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
  using MiniUML.Model.ViewModels.Shapes;

  public class DocumentDataModel : DataModel
  {
    #region fields
    // name of plug-in model that is associated with this document
    private readonly string mPluginModelName = string.Empty;

    private PageViewModelBase mRoot;
    private ObservableCollection<ShapeViewModelBase> mDocRoot;

    #region Undo
    private int mOperationLevel = 0;

    private bool mHasUnsavedData;

    /// <summary>
    /// Take a snapshot of all data to compare thise snapshot with changed data version.
    /// This is necessary to evaluate whether undo should be implemented (on changed data) or not.
    /// </summary>
    private UndoState mPendingUndoState;

    /// <summary>
    /// List of undo states to implement UNDO when user executes the undo command.
    /// </summary>
    private LinkedList<UndoState> mUndoList = new LinkedList<UndoState>();

    /// <summary>
    /// List of redo states to implement REDO when user executes the undo command.
    /// </summary>
    private LinkedList<UndoState> mRedoList = new LinkedList<UndoState>();
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
      this.mPluginModelName = pluginModelName;

      this.mRoot = new PageViewModelBase();

      this.setDocRoot(this.mRoot);
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
        return this.mPluginModelName;
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
        this.VerifyAccess();
        return (this.mUndoList.Count > 0);
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
        this.VerifyAccess();
        return (this.mRedoList.Count > 0);
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
        return (this.mHasUnsavedData);
      }
    }

    /// <summary>
    /// Get the root of the collection of shapes visible on the canvas page.
    /// </summary>
    public ObservableCollection<ShapeViewModelBase> DocRoot
    {
      get
      {
        this.VerifyAccess();

        if (this.mDocRoot == null)
          this.setCreateRoot();

        return this.mDocRoot;
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

        return this.mRoot.prop_PageSize;
      }

      protected set
      {
        this.VerifyAccess();

        if (this.mRoot.prop_PageSize != value)
        {
          this.mRoot.prop_PageSize = value;

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

        return this.mRoot.prop_PageMargins;
      }

      protected set
      {
        this.VerifyAccess();

        if (this.mRoot.prop_PageMargins != value)
        {
          this.mRoot.prop_PageMargins = value;
          this.NotifyPropertyChanged(() => this.PageMargins);
        }
      }
    }
    #endregion properties

    #region methods
    public string GetShapesAsXmlString(IEnumerable<ShapeViewModelBase> coll)
    {
      return this.mRoot.SaveDocument(coll);
    }

    /// <summary>
    /// Create a new document.
    /// </summary>
    public void New(PageViewModelBase root)
    {
      this.VerifyAccess();

      this.VerifyState(ModelState.Ready, ModelState.Invalid);

      this.mRoot = root;
      this.NotifyPropertyChanged(() => this.PageSize);
      this.NotifyPropertyChanged(() => this.PageMargins);

      this.setDocRoot(root);

      this.ClearUndoRedo();
      this.mMaxId = 0;
      this.mHasUnsavedData = false;

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
        if (this.mDocRoot == null)
          this.setCreateRoot();
        else
          this.mDocRoot.Clear();

        if (newDocumentRoot != null)
          this.SetDocRoot(newDocumentRoot, coll);

        this.ClearUndoRedo();
        this.mHasUnsavedData = false;
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
      this.VerifyAccess();

      this.VerifyState(ModelState.Ready);

      this.State = ModelState.Saving;

      try
      {
        PageViewModelBase docRoot = this.GetXmlElementDocRoot();

        docRoot.SaveDocument(filename, this.mDocRoot);

        this.mHasUnsavedData = false;
      }
      finally
      {
        this.State = ModelState.Ready;
      }

      this.SendPropertyChanged("HasUnsavedData");
    }

    /// <summary>
    /// Roll back to the previous state.
    /// </summary>
    /// <param name="parentOfShapes">Is necessary to create shapes with references to their parent.</param>
    public void Undo(IShapeParent parentOfShapes)
    {
      this.VerifyAccess();

      this.VerifyState(ModelState.Ready, ModelState.Invalid);

      if (this.HasUndoData == true)
      {
        UndoState undoState = this.mUndoList.First.Value;

        string fragment = string.Empty;

        if (this.mDocRoot.Count > 0)
          fragment = this.GetShapesAsXmlString(this.mDocRoot);

        this.mRedoList.AddFirst(new UndoState(fragment, this.mHasUnsavedData));
        this.mUndoList.RemoveFirst();

        // Reload shape collection from this Xml formated (persistence) undo state
        this.RecreateShapeCollectionFromXml(parentOfShapes, undoState.DocRoot);

        this.mHasUnsavedData = undoState.HasUnsavedData;

        this.State = ModelState.Ready;

        this.SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
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
      this.VerifyAccess();

      this.VerifyState(ModelState.Ready, ModelState.Invalid);

      if (this.HasRedoData)
      {
        UndoState undoState = this.mRedoList.First.Value;

        string fragment = string.Empty;

        if (this.mDocRoot.Count > 0)
          fragment = this.GetShapesAsXmlString(this.mDocRoot);

        this.mUndoList.AddFirst(new UndoState(fragment, this.mHasUnsavedData));
        this.mRedoList.RemoveFirst();

        // Reload shape collection from this Xml formated (persistence) undo state
        this.RecreateShapeCollectionFromXml(parentOfShapes, undoState.DocRoot);

        this.mHasUnsavedData = undoState.HasUnsavedData;

        this.State = ModelState.Ready;

        this.SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
      }
    }

    /// <summary>
    /// Begins an "atomic" operation, during which the model state is Busy and no restore points are created.
    /// </summary>
    public void BeginOperation(string operationName)
    {
      this.VerifyAccess();

      //// Debug.WriteLine("Begin operation #" + (_operationLevel + 1) + ": " + operationName);

      if (this.mOperationLevel++ == 0)
      {
        this.VerifyState(ModelState.Ready, ModelState.Invalid);

        this.setPendingUndoState();
        this.State = ModelState.Busy;
      }
      else
        this.VerifyState(ModelState.Busy);
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

      if (--this.mOperationLevel == 0)
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
        this.BeginOperation("Remove Shapes");

        foreach (var item in coll)
        {
          this.mDocRoot.Remove(item);
        }
      }
      finally
      {
        this.EndOperation("Remove Shapes");
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
      this.VerifyAccess();

      return this.mRoot.SaveDocument(this.mDocRoot);
    }

    public PageViewModelBase GetXmlElementDocRoot()
    {
      this.VerifyAccess();

      PageViewModelBase docRoot = new PageViewModelBase(this.mRoot);

      if (this.mDocRoot != null)
      {
        foreach (var item in this.mDocRoot)
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
        this.BeginOperation(string.Format("AddShape ID: {0}", idString));

        if (pos == InsertPosition.First)
          this.DocRoot.Insert(0, shape);   // Insert shape at bottom of virtual Z-axis
        else
          this.DocRoot.Add(shape);        // Insert shape at top of virtual Z-axis
      }
      finally
      {
        this.EndOperation(string.Format("AddShape ID: {0}", idString));
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
      this.BeginOperation("SendToBack");
      this.mDocRoot.Remove(obj);
      this.EndOperation("SendToBack");
    }

    /// <summary>
    /// Brings the shape into front of the canvas view
    /// (moves shape on top of virtual Z-axis)
    /// </summary>
    /// <param name="obj"></param>
    internal void BringToFront(ShapeViewModelBase obj)
    {
      this.BeginOperation("BringToFront");
      this.mDocRoot.Remove(obj);
      this.AddShape(obj, InsertPosition.Last);
      this.EndOperation("BringToFront");
    }

    /// <summary>
    /// Brings the shape into the back of the canvas view
    /// (moves shape to the bottom of virtual Z-axis)
    /// </summary>
    /// <param name="obj"></param>
    internal void SendToBack(ShapeViewModelBase obj)
    {
      this.BeginOperation("SendToBack");
      this.mDocRoot.Remove(obj);
      this.AddShape(obj, InsertPosition.First);
      this.EndOperation("SendToBack");
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

      if (this.mDocRoot.Count > 0)
        fragment = this.GetShapesAsXmlString(this.mDocRoot);

      this.mPendingUndoState = new UndoState(fragment, this.mHasUnsavedData);
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

      if (this.mUndoList.First == null || this.mUndoList.First.Value.DocumentRootXml != this.mPendingUndoState.DocumentRootXml)
      {
        this.mUndoList.AddFirst(this.mPendingUndoState);
        this.mRedoList.Clear();
        this.mHasUnsavedData = true;

        this.SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
      }
    }

    private void ClearUndoRedo()
    {
      this.mUndoList.Clear();
      this.mRedoList.Clear();
    }

    private void SetDocRoot(PageViewModelBase newDocumentRoot,
                            IEnumerable<ShapeViewModelBase> coll)
    {
      double pageWidth = newDocumentRoot.prop_PageSize.Width;

      double pageHeight = newDocumentRoot.prop_PageSize.Height;

      this.setDocRoot(new PageViewModelBase() { prop_PageSize = new Size(pageWidth, pageHeight),
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
      if (this.mDocRoot != null)
      {
        this.mDocRoot.CollectionChanged -= this.mDocRoot_CollectionChanged;
        this.mDocRoot.Clear();
        this.mDocRoot = null;
      }

      this.mDocRoot = new ObservableCollection<ShapeViewModelBase>();

      this.mDocRoot.CollectionChanged += this.mDocRoot_CollectionChanged;
    }

    /// <summary>
    /// Resets all items in the collection of shapes with the items in the parameter collection.
    /// </summary>
    /// <param name="list"></param>
    private void ResetDocRoot(IEnumerable<ShapeViewModelBase> list)
    {
      this.mDocRoot.Clear();

      if (list != null)
      {
        foreach (var item in list)
          this.mDocRoot.Add(item);
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
