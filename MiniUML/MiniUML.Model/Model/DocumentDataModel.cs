namespace MiniUML.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using MiniUML.Framework;
    using MiniUML.Model.ViewModels;
    using MiniUML.Model.ViewModels.Document;
    using MiniUML.Model.ViewModels.Interfaces;
    using MiniUML.Model.ViewModels.Shapes;

    public class DocumentDataModel : DataModel
    {
        #region fields
        /// <summary>
        /// /name of plug-in model that is associated with this document
        /// </summary>
        private readonly string _PluginModelName = string.Empty;

        private PageViewModelBase _Root;
        private ObservableCollection<ShapeViewModelBase> _DocRoot;

        #region Undo
        private int _OperationLevel = 0;

        private bool _HasUnsavedData;

        /// <summary>
        /// Take a snapshot of all data to compare thise snapshot with changed data version.
        /// This is necessary to evaluate whether undo should be implemented (on changed data) or not.
        /// </summary>
        private UndoState mPendingUndoState;

        /// <summary>
        /// List of undo states to implement UNDO when user executes the undo command.
        /// </summary>
        private LinkedList<UndoState> _UndoList = new LinkedList<UndoState>();

        /// <summary>
        /// List of redo states to implement REDO when user executes the undo command.
        /// </summary>
        private LinkedList<UndoState> _RedoList = new LinkedList<UndoState>();
        #endregion Undo

        /// <summary>
        /// This field is incremented to generate unique ID from maximum + 1
        /// </summary>
        private long mMaxId = 0;
        #endregion fields

        #region constructor
        /// <summary>
        /// Constructor initializing the data model as an empty document.
        /// </summary>
        public DocumentDataModel(string pluginModelName)
        {
            _PluginModelName = pluginModelName;

            _Root = new PageViewModelBase();

            this.setDocRoot(_Root);
            ////this.setDocumentRoot(new XElement("invalid"));

            this.State = ModelState.Invalid;
        }
        #endregion constructor

        public enum InsertPosition
        {
            First = 0,
            Last = 1
        }

        #region properties
        public string PluginModelName
        {
            get
            {
                return _PluginModelName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an Undo operation is possible.
        /// An INotifyPropertyChanged-enabled property.
        /// </summary>
        public bool HasUndoData
        {
            get
            {
                VerifyAccess();
                return (_UndoList.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an Redo operation is possible.
        /// An INotifyPropertyChanged-enabled property.
        /// </summary>
        public bool HasRedoData
        {
            get
            {
                VerifyAccess();
                return (_RedoList.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an Redo operation is possible.
        /// An INotifyPropertyChanged-enabled property.
        /// </summary>
        public bool HasUnsavedData
        {
            get
            {
                this.VerifyAccess();
                return (_HasUnsavedData);
            }
        }

        /// <summary>
        /// Get the root of the collection of shapes visible on the canvas page.
        /// </summary>
        public ObservableCollection<ShapeViewModelBase> DocRoot
        {
            get
            {
                VerifyAccess();

                if (_DocRoot == null)
                    setCreateRoot();

                return _DocRoot;
            }
        }

        /// <summary>
        /// Get current size of canvas on which shapes can be positioned.
        /// </summary>
        public Size PageSize
        {
            get
            {
                this.VerifyAccess();

                return _Root.prop_PageSize;
            }

            protected set
            {
                this.VerifyAccess();

                if (_Root.prop_PageSize != value)
                {
                    _Root.prop_PageSize = value;

                    this.NotifyPropertyChanged(() => this.PageSize);
                }
            }
        }

        /// <summary>
        /// Get thickness of margin that is used to display the canvas in the dokument view.
        /// </summary>
        public Thickness PageMargins
        {
            get
            {
                this.VerifyAccess();

                return _Root.prop_PageMargins;
            }

            protected set
            {
                this.VerifyAccess();

                if (_Root.prop_PageMargins != value)
                {
                    _Root.prop_PageMargins = value;
                    this.NotifyPropertyChanged(() => this.PageMargins);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the maximum bounds of all items being displayed on the canvas.
        /// </summary>
        /// <param name="margin"></param>
        /// <returns></returns>
        public Rect GetMaxBounds(Rect margin = default(Rect))
        {
            Rect bounds = default(Rect); // Compute the maximum bounding rectangle

            foreach (var item in this.DocRoot)
            {
                if ((item is IShapeSizeViewModelBase) == false)
                {
                    Console.WriteLine("--> Not a IShapeSizeViewModelBase {0}", item);
                }
                else
                {
                    var szItem = item as IShapeSizeViewModelBase;

                    if (bounds == default(Rect))
                        bounds = new Rect(0, 0, szItem.Left + szItem.Width,
                                                szItem.Top + szItem.Height);
                    else
                    {
                        if (bounds.Width < (szItem.Left + szItem.Width))
                            bounds.Width = szItem.Left + szItem.Width;

                        if ((bounds.Height) < (szItem.Top + szItem.Height))
                            bounds.Height = szItem.Top + szItem.Height;
                    }
                }
            }

            if (margin != default(Rect))
            {
                bounds.X = bounds.X - margin.X;
                bounds.Y = bounds.Y - margin.Y;

                bounds.Width = bounds.Width + margin.Width;
                bounds.Height = bounds.Height + margin.Height;
            }

            return bounds;
        }

        public string GetShapesAsXmlString(IEnumerable<ShapeViewModelBase> coll)
        {
            return _Root.SaveDocument(coll);
        }

        /// <summary>
        /// Create a new document.
        /// </summary>
        public void New(PageViewModelBase root)
        {
            this.VerifyAccess();

            this.VerifyState(ModelState.Ready, ModelState.Invalid);

            _Root = root;
            this.NotifyPropertyChanged(() => this.PageSize);
            this.NotifyPropertyChanged(() => this.PageMargins);

            this.setDocRoot(root);

            this.ClearUndoRedo();
            this.mMaxId = 0;
            _HasUnsavedData = false;

            this.State = ModelState.Ready;

            this.SendPropertyChanged("HasUndoData",
                                     "HasRedoData",
                                     "HasUnsavedData");
        }

        /// <summary>
        /// Redefine document content based on given page definition and collection of objects.
        /// </summary>
        /// <param name="newDocumentRoot"></param>
        /// <param name="coll"></param>
        public void LoadFileFromCollection(PageViewModelBase newDocumentRoot,
                                           IEnumerable<ShapeViewModelBase> coll)
        {
            try
            {
                if (_DocRoot == null)
                    this.setCreateRoot();
                else
                    _DocRoot.Clear();

                if (newDocumentRoot != null)
                    this.SetDocRoot(newDocumentRoot, coll);

                this.ClearUndoRedo();
                _HasUnsavedData = false;
                this.mMaxId = 0;

                this.State = ModelState.Ready;
                this.SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        /// <summary>
        /// Save document to a file.
        /// </summary>
        public void Save(string filename)
        {
            VerifyAccess();
            VerifyState(ModelState.Ready);
            State = ModelState.Saving;

            try
            {
                PageViewModelBase docRoot = this.GetXmlElementDocRoot();
                docRoot.SaveDocument(filename, _DocRoot);

                _HasUnsavedData = false;
            }
            finally
            {
                State = ModelState.Ready;
            }

            NotifyPropertyChanged(() => HasUnsavedData);
        }

        /// <summary>
        /// Roll back to the previous state.
        /// </summary>
        /// <param name="parentOfShapes">Is necessary to create shapes with references to their parent.</param>
        public void Undo(IShapeParent parentOfShapes)
        {
            VerifyAccess();

            VerifyState(ModelState.Ready, ModelState.Invalid);

            if (HasUndoData == true)
            {
                UndoState undoState = _UndoList.First.Value;

                string fragment = string.Empty;

                if (_DocRoot.Count > 0)
                    fragment = this.GetShapesAsXmlString(_DocRoot);

                _RedoList.AddFirst(new UndoState(fragment, _HasUnsavedData));
                _UndoList.RemoveFirst();

                // Reload shape collection from this Xml formated (persistence) undo state
                RecreateShapeCollectionFromXml(parentOfShapes, undoState.DocRoot);

                _HasUnsavedData = undoState.HasUnsavedData;

                State = ModelState.Ready;

                NotifyPropertyChanged(() => HasUndoData);
                NotifyPropertyChanged(() => HasRedoData);
                NotifyPropertyChanged(() => HasUnsavedData);
            }
        }

        /// <summary>
        /// Roll forward to the next state.
        /// 
        /// Page definition properties are not in scope of undo/redo
        /// </summary>
        /// <param name="parentOfShapes">Is necessary to create shapes with references to their parent.</param>
        public void Redo(IShapeParent parentOfShapes)
        {
            VerifyAccess();

            VerifyState(ModelState.Ready, ModelState.Invalid);

            if (HasRedoData)
            {
                UndoState undoState = _RedoList.First.Value;

                string fragment = string.Empty;

                if (_DocRoot.Count > 0)
                    fragment = this.GetShapesAsXmlString(_DocRoot);

                _UndoList.AddFirst(new UndoState(fragment, _HasUnsavedData));
                _RedoList.RemoveFirst();

                // Reload shape collection from this Xml formated (persistence) undo state
                RecreateShapeCollectionFromXml(parentOfShapes, undoState.DocRoot);

                _HasUnsavedData = undoState.HasUnsavedData;

                State = ModelState.Ready;

                NotifyPropertyChanged(() => HasUndoData);
                NotifyPropertyChanged(() => HasRedoData);
                NotifyPropertyChanged(() => HasUnsavedData);
            }
        }

        /// <summary>
        /// Begins an "atomic" operation, during which the model state is Busy and no restore points are created.
        /// </summary>
        public void BeginOperation(string operationName)
        {
            VerifyAccess();

            //// Debug.WriteLine("Begin operation #" + (_operationLevel + 1) + ": " + operationName);

            if (_OperationLevel++ == 0)
            {
                VerifyState(ModelState.Ready, ModelState.Invalid);

                setPendingUndoState();
                State = ModelState.Busy;
            }
            else
                VerifyState(ModelState.Busy);
        }

        /// <summary>
        /// End an "atomic" operation, during which the model state is Busy and no restore points are created.
        /// </summary>
        /// <param name="operationName"></param>
        public void EndOperation(string operationName)
        {
            this.EndOperationWithoutCreatingUndoState(operationName);

            if (this.State == ModelState.Ready)
                this.applyPendingUndoState();
        }

        public void EndOperationWithoutCreatingUndoState(string operationName)
        {
            ////DebugUtilities.Assert(_operationLevel > 0, "Trying to end operation " + operationName + " that hasn't begun");

            //// Debug.WriteLine("End operation #" + _operationLevel + ": " + operationName);

            this.VerifyState(ModelState.Busy);

            if (--_OperationLevel == 0)
                this.State = ModelState.Ready;
        }

        /// <summary>
        /// Delete the listed elements in <paramref name="coll"/> from internal
        /// collection of canvas elements.
        /// </summary>
        /// <param name="coll"></param>
        public void DeleteElements(IEnumerable<ShapeViewModelBase> coll)
        {
            try
            {
                BeginOperation("Remove Shapes");

                foreach (var item in coll)
                {
                    _DocRoot.Remove(item);
                }
            }
            finally
            {
                EndOperation("Remove Shapes");
            }
        }

        /// <summary>
        /// Get the item with the <paramref name="id"/> and return it.
        /// It is important to return the actual item (and not a copy)
        /// since the itemcomtrol in the canvas view is using that
        /// reference to find the contentpresenter bound to it
        /// (via ShapeIdToControlConverter).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShapeViewModelBase GetShapeById(string id)
        {
            IEnumerable<ShapeViewModelBase> shapes = this.DocRoot.Where(c => c.ID == id);

            foreach (ShapeViewModelBase result in shapes)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Returns a unique (unused) Id. The returned ids are guaranteed not to repeat unless New or Load is called.
        /// </summary>
        public string GetUniqueId()
        {
            const string PREFIX = "auto_";

            foreach (ShapeViewModelBase element in this.DocRoot)
            {
                string elementIdString = element.ID;

                if (!elementIdString.StartsWith(PREFIX))
                    continue;

                long elementId;
                if (long.TryParse(elementIdString.Substring(PREFIX.Length), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out elementId) == false)
                    continue;

                if (this.mMaxId <= elementId)
                    this.mMaxId = elementId + 1;
            }

            return PREFIX + this.mMaxId.ToString("X", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Remove an element from the current collection of shape viewmodels.
        /// (Todo: Should be consolidated with this.Remove())
        /// </summary>
        /// <param name="element"></param>
        public void RemoveShape(ShapeViewModelBase element)
        {
            this.DocRoot.Remove(element);
        }

        public string SaveDocument()
        {
            VerifyAccess();

            return _Root.SaveDocument(_DocRoot);
        }

        public PageViewModelBase GetXmlElementDocRoot()
        {
            this.VerifyAccess();

            PageViewModelBase docRoot = new PageViewModelBase(_Root);

            if (_DocRoot != null)
            {
                foreach (var item in _DocRoot)
                {
                    docRoot.Add(item);
                }
            }

            return docRoot;
        }

        /// <summary>
        /// This method returns the created copy for further reference.
        /// Also assigns a unique ID to the shape, if the existing ID is taken.
        /// </summary>
        internal void AddShape(ShapeViewModelBase shape,
                               InsertPosition pos = InsertPosition.Last)
        {
            this.VerifyState(ModelState.Ready, ModelState.Busy);

            string idString = shape.ID;

            if (idString != string.Empty && this.GetShapeById(idString) != null)
                shape.ID = this.GetUniqueId();

            // Add new shape inside Undo/Redo pattern
            try
            {
                BeginOperation(string.Format("AddShape ID: {0}", idString));

                if (pos == InsertPosition.First)
                    this.DocRoot.Insert(0, shape);   // Insert shape at bottom of virtual Z-axis
                else
                    this.DocRoot.Add(shape);        // Insert shape at top of virtual Z-axis
            }
            finally
            {
                EndOperation(string.Format("AddShape ID: {0}", idString));
            }
        }

        #region IShapeParent methods (should be invoked through CanvasViewModel only)
        /// <summary>
        /// Removes the corresponding shape from the
        /// collection of shapes displayed on the canvas.
        /// </summary>
        /// <param name="obj"></param>
        internal void Remove(ShapeViewModelBase obj)
        {
            try
            {
                BeginOperation("SendToBack");
                _DocRoot.Remove(obj);
            }
            finally
            {
                EndOperation("SendToBack");
            }
        }

        /// <summary>
        /// Brings the shape into front of the canvas view
        /// (moves shape on top of virtual Z-axis)
        /// </summary>
        /// <param name="obj"></param>
        internal void BringToFront(ShapeViewModelBase obj)
        {
            try
            {
                BeginOperation("BringToFront");
                _DocRoot.Remove(obj);
                AddShape(obj, InsertPosition.Last);
            }
            finally
            {
                EndOperation("BringToFront");
            }
        }

        /// <summary>
        /// Brings the shape into the back of the canvas view
        /// (moves shape to the bottom of virtual Z-axis)
        /// </summary>
        /// <param name="obj"></param>
        internal void SendToBack(ShapeViewModelBase obj)
        {
            try
            {
                BeginOperation("SendToBack");
                _DocRoot.Remove(obj);
                AddShape(obj, InsertPosition.First);
            }
            finally
            {
                EndOperation("SendToBack");
            }
        }
        #endregion IShapeParent methods

        /// <summary>
        /// TODO XXX Dirkster not sure why this is needed
        /// </summary>
        /// <returns></returns>
        protected override bool IsValid()
        {
            return true;
        }

        private void setPendingUndoState()
        {
            this.VerifyAccess();

            this.VerifyState(ModelState.Ready, ModelState.Invalid);

            string fragment = string.Empty;

            if (_DocRoot.Count > 0)
                fragment = this.GetShapesAsXmlString(_DocRoot);

            this.mPendingUndoState = new UndoState(fragment, _HasUnsavedData);
        }

        /// <summary>
        /// Check if the last operation has changed something and apply undo if so.
        /// </summary>
        private void applyPendingUndoState()
        {
            this.VerifyAccess();

            this.VerifyState(ModelState.Ready, ModelState.Invalid);

            // Nothing has actually changed.
            if (this.mPendingUndoState.DocumentRootXml == this.SaveDocument())
                return;

            if (_UndoList.First == null || _UndoList.First.Value.DocumentRootXml != this.mPendingUndoState.DocumentRootXml)
            {
                _UndoList.AddFirst(this.mPendingUndoState);
                _RedoList.Clear();
                _HasUnsavedData = true;

                this.SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
            }
        }

        private void ClearUndoRedo()
        {
            _UndoList.Clear();
            _RedoList.Clear();
        }

        private void SetDocRoot(PageViewModelBase newDocumentRoot,
                                IEnumerable<ShapeViewModelBase> coll)
        {
            double pageWidth = newDocumentRoot.prop_PageSize.Width;

            double pageHeight = newDocumentRoot.prop_PageSize.Height;

            this.setDocRoot(new PageViewModelBase()
            {
                prop_PageSize = new Size(pageWidth, pageHeight),
                prop_PageMargins = newDocumentRoot.prop_PageMargins
            });

            this.ResetDocRoot(coll);
        }

        private void setDocRoot(PageViewModelBase root)
        {
            /*** TODO XXX Dirkster: Not sure why this is needed (maybe to set busy states?)
                  if (_documentRoot != null)
                  {
                    _documentRoot.Changed -= documentRoot_Changed;
                    _documentRoot.Changing -= documentRoot_Changing;
                  }

                  if (newDocumentRoot != null)
                  {
                    newDocumentRoot.Changed += documentRoot_Changed;
                    newDocumentRoot.Changing += documentRoot_Changing;
                  }
            ***/
            this.PageSize = root.prop_PageSize;
            this.PageMargins = root.prop_PageMargins;
        }

        /// <summary>
        /// (Re-)create the collection of canvas view elements.
        /// </summary>
        private void setCreateRoot()
        {
            if (_DocRoot != null)
            {
                _DocRoot.CollectionChanged -= this.mDocRoot_CollectionChanged;
                _DocRoot.Clear();
                _DocRoot = null;
            }

            _DocRoot = new ObservableCollection<ShapeViewModelBase>();

            _DocRoot.CollectionChanged += this.mDocRoot_CollectionChanged;
        }

        /// <summary>
        /// Resets all items in the collection of shapes with the items in the parameter collection.
        /// </summary>
        /// <param name="list"></param>
        private void ResetDocRoot(IEnumerable<ShapeViewModelBase> list)
        {
            _DocRoot.Clear();

            if (list != null)
            {
                foreach (var item in list)
                    _DocRoot.Add(item);
            }
        }

        /// <summary>
        /// Recreate document collection from Xml persistence in a string.
        /// </summary>
        /// <param name="parentOfShapes"></param>
        /// <param name="xmlText"></param>
        private void RecreateShapeCollectionFromXml(IShapeParent parentOfShapes, string xmlText)
        {
            // Look-up plugin model
            string plugin = this.PluginModelName;
            PluginModelBase m = PluginManager.GetPluginModel(plugin);

            // Look-up shape converter
            UmlTypeToStringConverterBase conv = null;
            conv = m.ShapeConverter;

            // Convert Xml document into a list of shapes and page definition
            List<ShapeViewModelBase> coll;
            PageViewModelBase page = conv.ReadDocument(xmlText, parentOfShapes, out coll);

            if (coll == null)
                return;

            if (coll.Count == 0)
                return;

            // Page definition properties are not in scope of undo/redo
            this.ResetDocRoot(coll);
        }

        /// <summary>
        /// An element in the colleciton of canvas elements has changed
        /// (Notifies recipients, such as, an XML Viewer to update their display).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDocRoot_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.VerifyAccess();
            /***
                  if (State != ModelState.Busy)
                  {
                    if (!IsValid())
                    {
                      if (State == ModelState.Ready) State = ModelState.Invalid;
                    }
                    else
                    {
                      if (State == ModelState.Invalid) State = ModelState.Ready;
                    }

                    applyPendingUndoState();
                  }
            ***/
            this.SendPropertyChanged("DocRoot");
            /*** Old setDocumentRoot code
            private void documentRoot_Changing(object sender, XObjectChangeEventArgs e)
            {
              if (State != ModelState.Busy)
                this.setPendingUndoState();
            }

            private void setDocumentRoot(XElement newDocumentRoot)
            {
              if (this.GetXmlElementDocRoot() != null)
              {
                _documentRoot.Changed -= documentRoot_Changed;
                _documentRoot.Changing -= documentRoot_Changing;
              }

              if (newDocumentRoot != null)
              {
                newDocumentRoot.Changed += documentRoot_Changed;
                newDocumentRoot.Changing += documentRoot_Changing;
              }

              _documentRoot = newDocumentRoot;
            }

            private void documentRoot_Changed(object sender, XObjectChangeEventArgs e)
            {
              if (State != ModelState.Busy)
              {
                if (!IsValid())
                {
                  if (State == ModelState.Ready) State = ModelState.Invalid;
                }
                else
                {
                  if (State == ModelState.Invalid) State = ModelState.Ready;
                }

                applyPendingUndoState();
              }
            }

        ***/
        }
        #endregion methods

        #region private struct UndoStruct
        private struct UndoState
        {
            private readonly string mDocumentXml;
            private readonly bool mHasUnsavedData;

            public UndoState(string fragment, bool hasUnsavedData)
            {
                this.mDocumentXml = fragment;
                this.mHasUnsavedData = hasUnsavedData;
            }

            public bool HasUnsavedData
            {
                get
                {
                    return this.mHasUnsavedData;
                }
            }

            public string DocRoot
            {
                get
                {
                    return this.mDocumentXml;
                }
            }

            public string DocumentRootXml
            {
                get
                {
                    return this.mDocumentXml;
                }
            }
        }
        #endregion private struct UndoStruct
    }
}
