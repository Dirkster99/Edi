namespace MiniUML.Model.Model
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using Framework;
  using ViewModels;
  using ViewModels.Document;
  using ViewModels.Shapes;

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
      mPluginModelName = pluginModelName;

      mRoot = new PageViewModelBase();

      setDocRoot(mRoot);
      ////this.setDocumentRoot(new XElement("invalid"));

      State = ModelState.Invalid;
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
        return mPluginModelName;
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
        return (mUndoList.Count > 0);
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
        return (mRedoList.Count > 0);
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
        VerifyAccess();
        return (mHasUnsavedData);
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

        if (mDocRoot == null)
          setCreateRoot();

        return mDocRoot;
      }
    }

    /// <summary>
    /// Get current size of canvas on which shapes can be positioned.
    /// </summary>
    public Size PageSize
    {
      get
      {
        VerifyAccess();

        return mRoot.prop_PageSize;
      }

      protected set
      {
        VerifyAccess();

        if (mRoot.prop_PageSize != value)
        {
          mRoot.prop_PageSize = value;

          NotifyPropertyChanged(() => PageSize);
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
        VerifyAccess();

        return mRoot.prop_PageMargins;
      }

      protected set
      {
        VerifyAccess();

        if (mRoot.prop_PageMargins != value)
        {
          mRoot.prop_PageMargins = value;
          NotifyPropertyChanged(() => PageMargins);
        }
      }
    }
    #endregion properties

    #region methods
    public string GetShapesAsXmlString(IEnumerable<ShapeViewModelBase> coll)
    {
      return mRoot.SaveDocument(coll);
    }

    /// <summary>
    /// Create a new document.
    /// </summary>
    public void New(PageViewModelBase root)
    {
      VerifyAccess();

      VerifyState(ModelState.Ready, ModelState.Invalid);

      mRoot = root;
      NotifyPropertyChanged(() => PageSize);
      NotifyPropertyChanged(() => PageMargins);

      setDocRoot(root);

      ClearUndoRedo();
      mMaxId = 0;
      mHasUnsavedData = false;

      State = ModelState.Ready;

      SendPropertyChanged("HasUndoData",
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
        if (mDocRoot == null)
          setCreateRoot();
        else
          mDocRoot.Clear();

        if (newDocumentRoot != null)
          SetDocRoot(newDocumentRoot, coll);

        ClearUndoRedo();
        mHasUnsavedData = false;
        mMaxId = 0;

        State = ModelState.Ready;
        SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
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
        PageViewModelBase docRoot = GetXmlElementDocRoot();

        docRoot.SaveDocument(filename, mDocRoot);

        mHasUnsavedData = false;
      }
      finally
      {
        State = ModelState.Ready;
      }

      SendPropertyChanged("HasUnsavedData");
    }

    /// <summary>
    /// Roll back to the previous state.
    /// </summary>
    /// <param name="parentOfShapes">Is necessary to create shapes with references to their parent.</param>
    public void Undo(IShapeParent parentOfShapes)
    {
      VerifyAccess();

      VerifyState(ModelState.Ready, ModelState.Invalid);

      if (HasUndoData)
      {
        UndoState undoState = mUndoList.First.Value;

        string fragment = string.Empty;

        if (mDocRoot.Count > 0)
          fragment = GetShapesAsXmlString(mDocRoot);

        mRedoList.AddFirst(new UndoState(fragment, mHasUnsavedData));
        mUndoList.RemoveFirst();

        // Reload shape collection from this Xml formated (persistence) undo state
        RecreateShapeCollectionFromXml(parentOfShapes, undoState.DocRoot);

        mHasUnsavedData = undoState.HasUnsavedData;

        State = ModelState.Ready;

        SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
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
        UndoState undoState = mRedoList.First.Value;

        string fragment = string.Empty;

        if (mDocRoot.Count > 0)
          fragment = GetShapesAsXmlString(mDocRoot);

        mUndoList.AddFirst(new UndoState(fragment, mHasUnsavedData));
        mRedoList.RemoveFirst();

        // Reload shape collection from this Xml formated (persistence) undo state
        RecreateShapeCollectionFromXml(parentOfShapes, undoState.DocRoot);

        mHasUnsavedData = undoState.HasUnsavedData;

        State = ModelState.Ready;

        SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
      }
    }

    /// <summary>
    /// Begins an "atomic" operation, during which the model state is Busy and no restore points are created.
    /// </summary>
    public void BeginOperation(string operationName)
    {
      VerifyAccess();

      //// Debug.WriteLine("Begin operation #" + (_operationLevel + 1) + ": " + operationName);

      if (mOperationLevel++ == 0)
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
      EndOperationWithoutCreatingUndoState(operationName);

      if (State == ModelState.Ready)
        applyPendingUndoState();
    }

    public void EndOperationWithoutCreatingUndoState(string operationName)
    {
      ////DebugUtilities.Assert(_operationLevel > 0, "Trying to end operation " + operationName + " that hasn't begun");

      //// Debug.WriteLine("End operation #" + _operationLevel + ": " + operationName);

      VerifyState(ModelState.Busy);

      if (--mOperationLevel == 0)
        State = ModelState.Ready;
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
          mDocRoot.Remove(item);
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
      IEnumerable<ShapeViewModelBase> shapes = DocRoot.Where(c => c.ID == id);

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

      foreach (ShapeViewModelBase element in DocRoot)
      {
        string elementIdString = element.ID;

        if (!elementIdString.StartsWith(PREFIX))
          continue;

        long elementId;
        if (long.TryParse(elementIdString.Substring(PREFIX.Length), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out elementId) == false)
          continue;

        if (mMaxId <= elementId)
          mMaxId = elementId + 1;
      }

      return PREFIX + mMaxId.ToString("X", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Remove an element from the current collection of shape viewmodels.
    /// (Todo: Should be consolidated with this.Remove())
    /// </summary>
    /// <param name="element"></param>
    public void RemoveShape(ShapeViewModelBase element)
    {
      DocRoot.Remove(element);
    }

    public string SaveDocument()
    {
      VerifyAccess();

      return mRoot.SaveDocument(mDocRoot);
    }

    public PageViewModelBase GetXmlElementDocRoot()
    {
      VerifyAccess();

      PageViewModelBase docRoot = new PageViewModelBase(mRoot);

      if (mDocRoot != null)
      {
        foreach (var item in mDocRoot)
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
      VerifyState(ModelState.Ready, ModelState.Busy);

      string idString = shape.ID;

      if (idString != string.Empty && GetShapeById(idString) != null)
        shape.ID = GetUniqueId();

      // Add new shape inside Undo/Redo pattern
      try
      {
        BeginOperation(string.Format("AddShape ID: {0}", idString));

        if (pos == InsertPosition.First)
          DocRoot.Insert(0, shape);   // Insert shape at bottom of virtual Z-axis
        else
          DocRoot.Add(shape);        // Insert shape at top of virtual Z-axis
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
      BeginOperation("SendToBack");
      mDocRoot.Remove(obj);
      EndOperation("SendToBack");
    }

    /// <summary>
    /// Brings the shape into front of the canvas view
    /// (moves shape on top of virtual Z-axis)
    /// </summary>
    /// <param name="obj"></param>
    internal void BringToFront(ShapeViewModelBase obj)
    {
      BeginOperation("BringToFront");
      mDocRoot.Remove(obj);
      AddShape(obj, InsertPosition.Last);
      EndOperation("BringToFront");
    }

    /// <summary>
    /// Brings the shape into the back of the canvas view
    /// (moves shape to the bottom of virtual Z-axis)
    /// </summary>
    /// <param name="obj"></param>
    internal void SendToBack(ShapeViewModelBase obj)
    {
      BeginOperation("SendToBack");
      mDocRoot.Remove(obj);
      AddShape(obj, InsertPosition.First);
      EndOperation("SendToBack");
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
      VerifyAccess();

      VerifyState(ModelState.Ready, ModelState.Invalid);

      string fragment = string.Empty;

      if (mDocRoot.Count > 0)
        fragment = GetShapesAsXmlString(mDocRoot);

      mPendingUndoState = new UndoState(fragment, mHasUnsavedData);
    }

    /// <summary>
    /// Check if the last operation has changed something and apply undo if so.
    /// </summary>
    private void applyPendingUndoState()
    {
      VerifyAccess();

      VerifyState(ModelState.Ready, ModelState.Invalid);

      // Nothing has actually changed.
      if (mPendingUndoState.DocumentRootXml == SaveDocument())
        return;

      if (mUndoList.First == null || mUndoList.First.Value.DocumentRootXml != mPendingUndoState.DocumentRootXml)
      {
        mUndoList.AddFirst(mPendingUndoState);
        mRedoList.Clear();
        mHasUnsavedData = true;

        SendPropertyChanged("HasUndoData", "HasRedoData", "HasUnsavedData");
      }
    }

    private void ClearUndoRedo()
    {
      mUndoList.Clear();
      mRedoList.Clear();
    }

    private void SetDocRoot(PageViewModelBase newDocumentRoot,
                            IEnumerable<ShapeViewModelBase> coll)
    {
      double pageWidth = newDocumentRoot.prop_PageSize.Width;

      double pageHeight = newDocumentRoot.prop_PageSize.Height;

      setDocRoot(new PageViewModelBase() { prop_PageSize = new Size(pageWidth, pageHeight),
                                                         prop_PageMargins = newDocumentRoot.prop_PageMargins
                                                       });

      ResetDocRoot(coll);
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
      PageSize = root.prop_PageSize;
      PageMargins = root.prop_PageMargins;
    }

    /// <summary>
    /// (Re-)create the collection of canvas view elements.
    /// </summary>
    private void setCreateRoot()
    {
      if (mDocRoot != null)
      {
        mDocRoot.CollectionChanged -= mDocRoot_CollectionChanged;
        mDocRoot.Clear();
        mDocRoot = null;
      }

      mDocRoot = new ObservableCollection<ShapeViewModelBase>();

      mDocRoot.CollectionChanged += mDocRoot_CollectionChanged;
    }

    /// <summary>
    /// Resets all items in the collection of shapes with the items in the parameter collection.
    /// </summary>
    /// <param name="list"></param>
    private void ResetDocRoot(IEnumerable<ShapeViewModelBase> list)
    {
      mDocRoot.Clear();

      if (list != null)
      {
        foreach (var item in list)
          mDocRoot.Add(item);
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
      string plugin = PluginModelName;
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
      ResetDocRoot(coll);
    }

    /// <summary>
    /// An element in the colleciton of canvas elements has changed
    /// (Notifies recipients, such as, an XML Viewer to update their display).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void mDocRoot_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      VerifyAccess();
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
      SendPropertyChanged("DocRoot");
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
        mDocumentXml = fragment;
        mHasUnsavedData = hasUnsavedData;
      }

      public bool HasUnsavedData
      {
        get
        {
          return mHasUnsavedData;
        }
      }

      public string DocRoot
      {
        get
        {
          return mDocumentXml;
        }
      }

      public string DocumentRootXml
      {
        get
        {
          return mDocumentXml;
        }
      }
    }
    #endregion private struct UndoStruct
  }
}
