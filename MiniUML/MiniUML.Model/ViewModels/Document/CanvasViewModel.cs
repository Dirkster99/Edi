namespace MiniUML.Model.ViewModels.Document
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Framework;
    using Framework.Command;
    using Events;
    using Model;
    using RubberBand;
    using Shapes;
    using View.Views.RubberBand;

    /// <summary>
    /// Interface to define interaction for drag and drop
    /// mouse gestures when adding new connection lines,
    /// selecting and resizing shapes, and so for.
    /// </summary>
    public interface ICanvasViewMouseHandler
    {
        void OnShapeClick(ShapeViewModelBase shape);

        void OnShapeDragBegin(Point position, ShapeViewModelBase shape);

        void OnShapeDragUpdate(Point position, Vector delta);

        void OnShapeDragEnd(Point position, ShapeViewModelBase shape);

        void OnCancelMouseHandler();
    }

    public class CanvasViewModel : BaseViewModel, IShapeParent
    {
        #region fields
        private bool mIsFocused;

        private SelectedItems mSelectedItem;

        private RelayCommand<object> mSelectCommand = null;

        private RelayCommand<object> mDeleteCommand = null;
        private RelayCommand<object> mCutCommand = null;
        private RelayCommand<object> mCopyCommand = null;
        private RelayCommand<object> mPasteCommand = null;

        private ICanvasViewMouseHandler mICanvasViewMouseHandler = null;

        private RubberBandViewModel mRubberBand = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="documentViewModel"></param>
        public CanvasViewModel(DocumentViewModel documentViewModel)
        {
            // Store a reference to the parent view model
            // (necessary to implement begin and end operation around undo).
            DocumentViewModel = documentViewModel;

            mSelectedItem = new SelectedItems();

            mIsFocused = false;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// This property can be bound to a focus behaviour in a view
        /// to transfer focus when the viewmodel deems this to be appropriate.
        /// </summary>
        public bool IsFocused
        {
            get
            {
                return mIsFocused;
            }

            set
            {
                if (mIsFocused != value)
                {
                    mIsFocused = value;
                    NotifyPropertyChanged(() => IsFocused);
                }
            }
        }

        /// <summary>
        /// Property to expose selected items collection.
        /// </summary>
        public SelectedItems SelectedItem
        {
            get
            {
                return mSelectedItem;
            }
        }

        /// <summary>
        /// Viewmodel to the document represented by this canvas.
        /// </summary>
        public DocumentViewModel DocumentViewModel { get; private set; }

        /// <summary>
        /// Get canvas view model mouse handler which is used to draw
        /// connections between items - using a class that is defined
        /// outside of the actual canvas viewmodel class.
        /// </summary>
        public ICanvasViewMouseHandler CanvasViewMouseHandler
        {
            get
            {
                return mICanvasViewMouseHandler;
            }

            private set
            {
                if (mICanvasViewMouseHandler != value)
                {
                    mICanvasViewMouseHandler = value;
                }
            }
        }

        public RubberBandViewModel RubberBand
        {
            get
            {
                if (mRubberBand == null)
                    mRubberBand = new RubberBandViewModel();

                return mRubberBand;
            }
        }

        #region Commands
        /// <summary>
        /// Get a command that can be used to active the mouse select shape mode.
        /// </summary>
        public RelayCommand<object> SelectCommand
        {
            get
            {
                if (mSelectCommand == null)
                    mSelectCommand = new RelayCommand<object>((p) => OnSelectMode_Execute(),
                                                                   (p) => OnSelectMode_CanExecute());
                return mSelectCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Delete a currently selected shape mode.
        /// </summary>
        public RelayCommand<object> DeleteCommand
        {
            get
            {
                if (mDeleteCommand == null)
                    mDeleteCommand = new RelayCommand<object>((p) => OnDeleteCommand_Executed(),
                                                                   (p) => OnDeleteCutCopyCommand_CanExecute());
                return mDeleteCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Cut a currently selected shape into the windows clipboard.
        /// </summary>
        public RelayCommand<object> CutCommand
        {
            get
            {
                if (mCutCommand == null)
                    mCutCommand = new RelayCommand<object>((p) => OnCutCommand_Executed(),
                                                                (p) => OnDeleteCutCopyCommand_CanExecute());

                return mCutCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Copy a currently selected shape from the windows clipboard.
        /// </summary>
        public RelayCommand<object> CopyCommand
        {
            get
            {
                if (mCopyCommand == null)
                    mCopyCommand = new RelayCommand<object>((p) => OnCopyCommand_Executed(),
                                                                 (p) => OnDeleteCutCopyCommand_CanExecute());

                return mCopyCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Paste a shape from the windows clipboard
        /// into the collection of currently selected shapes on to the canvas.
        /// </summary>
        public RelayCommand<object> PasteCommand
        {
            get
            {
                if (mPasteCommand == null)
                    mPasteCommand = new RelayCommand<object>((p) => OnPasteCommand_Executed(),
                                                                  (p) => OnPasteCommand_CanExecute());

                return mPasteCommand;
            }
        }
        #endregion
        #endregion properties

        #region methods
        #region IShapeParent methods
        /// <summary>
        /// Removes the corresponding shape from the
        /// collection of shapes displayed on the canvas.
        /// </summary>
        /// <param name="obj"></param>
        void IShapeParent.Remove(ShapeViewModelBase shape)
        {
            DocumentViewModel.dm_DocumentDataModel.Remove(shape);
        }

        /// <summary>
        /// Brings the shape into front of the canvas view
        /// (moves shape on top of virtual Z-axis)
        /// </summary>
        /// <param name="obj"></param>
        void IShapeParent.BringToFront(ShapeViewModelBase shape)
        {
            DocumentViewModel.dm_DocumentDataModel.BringToFront(shape);
        }

        /// <summary>
        /// Brings the shape into the back of the canvas view
        /// (moves shape to the bottom of virtual Z-axis)
        /// </summary>
        /// <param name="obj"></param>
        void IShapeParent.SendToBack(ShapeViewModelBase shape)
        {
            DocumentViewModel.dm_DocumentDataModel.SendToBack(shape);
        }

        /// <summary>
        /// The resize shape function resizes all currently selected
        /// shapes in accordance to the delta contained in the supplied
        /// <paramref name="e"/> parameter.
        /// 
        /// Method is based on resize method in
        /// http://www.codeproject.com/Articles/23871/WPF-Diagram-Designer-Part-3
        /// </summary>
        /// <param name="e"></param>
        void IShapeParent.ResizeSelectedShapes(DragDeltaThumbEvent e)
        {
            if (SelectedItem.Shapes.Count == 0)
                return;

            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;
            double minDeltaHorizontal = double.MaxValue;
            double minDeltaVertical = double.MaxValue;
            double dragDeltaVertical, dragDeltaHorizontal;

            // filter for those items that contain a height property and
            // find the min of their min (Height, Width) properties
            foreach (var item in SelectedItem.Shapes)
            {
                ShapeSizeViewModelBase shape = item as ShapeSizeViewModelBase;

                if (shape == null) // filter for those items that have a height or width
                    continue;

                minLeft = Math.Min(shape.Left, minLeft);
                minTop = Math.Min(shape.Top, minTop);

                minDeltaVertical = Math.Min(minDeltaVertical, shape.Height - shape.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, shape.Width - shape.MinWidth);
            }

            // Resize currently selected items with regard to min height and width determined before
            foreach (var item in SelectedItem.Shapes)
            {
                ShapeSizeViewModelBase shape = item as ShapeSizeViewModelBase;

                if (shape == null) // filter for those items that have a height or width
                    continue;

                switch (e.VerticalAlignment)
                {
                    // Changing an element at its bottom changes the its height only
                    case VerticalAlignment.Bottom:
                        dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                        shape.Height = shape.Height - dragDeltaVertical;
                        break;

                    // Changing an element at its top changes the Y position and the height
                    case VerticalAlignment.Top:
                        dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                        shape.Top = shape.Top + dragDeltaVertical;
                        shape.Height = shape.Height - dragDeltaVertical;
                        break;
                }

                switch (e.HorizontalAlignment)
                {
                    // Changing an element at its left side changes the x position and its width
                    case HorizontalAlignment.Left:
                        dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                        shape.Left = shape.Left + dragDeltaHorizontal;
                        shape.Width = shape.Width - dragDeltaHorizontal;
                        break;

                    // Changing an element at its right side changes the its width only
                    case HorizontalAlignment.Right:
                        dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                        shape.Width = shape.Width - dragDeltaHorizontal;
                        break;
                }
            }
        }

        /// <summary>
        /// Align all selected shapes (if any) to a given shape <paramref name="shape"/>.
        /// The actual alignment operation performed is defined by the <paramref name="alignmentOption"/> parameter.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="alignmentOption"></param>
        void IShapeParent.AlignShapes(ShapeSizeViewModelBase shape, AlignShapes alignmentOption)
        {
            if (shape == null)
                return;

            double YShapeCenter = shape.Top + (shape.Height / 2);
            double XShapeCenter = shape.Left + (shape.Width / 2);
            double shapeRight = shape.Position.X + shape.Width;

            foreach (var item in SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
            {
                if (shape == item) // Adjust shape to itself is a superflous operation
                    continue;

                switch (alignmentOption)
                {
                    case AlignShapes.Bottom:
                        item.MoveEndPosition(new Point(item.EndPosition.X, shape.EndPosition.Y));
                        break;

                    case AlignShapes.CenteredHorizontal:
                        item.MovePosition(new Point(item.Position.X, YShapeCenter - (item.Height / 2)));
                        break;

                    case AlignShapes.CenteredVertical:
                        item.MovePosition(new Point(XShapeCenter - (item.Width / 2), item.Position.Y));
                        break;

                    case AlignShapes.Left:
                        item.MovePosition(new Point(shape.Position.X, item.Position.Y));
                        break;

                    case AlignShapes.Right:
                        item.MovePosition(new Point(shapeRight - item.Width, item.Position.Y));
                        break;

                    case AlignShapes.Top:
                        item.MovePosition(new Point(item.Position.X, shape.Top));
                        break;

                    default:
                        throw new NotImplementedException(alignmentOption.ToString());
                }
            }
        }

        /// <summary>
        /// Adjusts width, height, or both, of all selected shapes (if any)
        /// such that they are sized equally to the given <paramref name="shape"/>.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="option"></param>
        void IShapeParent.AdjustShapesToSameSize(ShapeSizeViewModelBase shape, SameSize option)
        {
            switch (option)
            {
                case SameSize.SameWidth:
                    SameWidth(shape);
                    break;

                case SameSize.SameHeight:
                    SameHeight(shape);
                    break;

                case SameSize.SameWidthandHeight:
                    SameWidthandHeight(shape);
                    break;

                default:
                    throw new NotImplementedException(option.ToString());
            }
        }

        /// <summary>
        /// Destribute all selected shapes (if any) over X or Y space evenly.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="distribOption"></param>
        void IShapeParent.DistributeShapes(Destribute distribOption)
        {
            switch (distribOption)
            {
                case Destribute.Horizontally:
                    DistributeHorizontal();
                    break;

                case Destribute.Vertically:
                    DistributeVertical();
                    break;

                default:
                    throw new NotImplementedException(distribOption.ToString());
            }
        }
        #endregion IShapeParent methods

        /// <summary>
        /// Add a new shape in the data model to make it visible on the canvas.
        /// </summary>
        /// <param name="element"></param>
        public void AddShape(ShapeViewModelBase element)
        {
            DocumentViewModel.dm_DocumentDataModel.AddShape(element);
        }

        #region Mouse handling (ICanvasViewMouseHandler)
        /// <summary>
        /// Method is executed when the canvas switches into the 'draw association'
        /// mode when the user clicks a draw 'connection' button. The method is called
        /// directly by the corresponding CommandModel that creates an association like shape.
        /// 
        /// The mouse handler supplied in <paramref name="value"/> is installed by this method
        /// which includes cancelling a previously installed mouse handler (if any) and beginning
        /// a new undo operation.
        /// </summary>
        /// <param name="value"></param>
        public void BeginCanvasViewMouseHandler(ICanvasViewMouseHandler value)
        {
            if (CanvasViewMouseHandler == value)  // no change
                return;

            if (CanvasViewMouseHandler != null)
                CancelCanvasViewMouseHandler();

            CanvasViewMouseHandler = value;

            if (CanvasViewMouseHandler != null)
                DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasViewMouseHandler session");
        }

        /// <summary>
        /// This method is called when the association drawing mode is cancelled
        /// (either via explicit command [click on Select Mode button] or
        /// because the source or target may be in-appropriate).
        /// </summary>
        public void CancelCanvasViewMouseHandler()
        {
            var handler = CanvasViewMouseHandler;
            CanvasViewMouseHandler = null;
            try
            {
                handler.OnCancelMouseHandler();

                CreateRubberBandMouseHandler rbh = handler as CreateRubberBandMouseHandler;

                // Make sure event completion handler is detached when mouse command handler is finished
                //// if (rbh != null)
                ////   rbh.RubberBandSelection -= this.handle_RubberBandSelection;
            }
            finally
            {
                DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasViewMouseHandler session");
            }
        }

        public void Handle_RubberBandSelection(RubberBandSelectionEventArgs e)
        {
            if (e != null)
            {
                if (e.Select == MouseSelection.CancelSelection)
                    return;

                // Clear existing selection since multiselection with addition is not what we want
                if (e.Select == MouseSelection.ReducedToNewSelection)
                    SelectedItem.Clear();

                Rect rubberBand = new Rect(e.StartPoint, e.EndPoint);

                foreach (var item in DocumentViewModel.dm_DocumentDataModel.DocRoot.OfType<ShapeSizeViewModelBase>())
                {
                    Rect itemBounds = new Rect(item.Position, item.EndPosition);

                    bool contains = rubberBand.Contains(itemBounds);

                    if (contains)
                    {
                        SelectedItem.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// This method is called when the draw association mode ends
        /// successfully with a new connection drawn on the canvas.
        /// </summary>
        public void FinishCanvasViewMouseHandler()
        {
            CanvasViewMouseHandler = null;
            DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasViewMouseHandler session");
        }

        /// <summary>
        /// Destroy the current rubberband viewmodel to make way
        /// for a new rubber band selection based on a new viewmodel and view.
        /// </summary>
        public void ResetRubberBand()
        {
            if (mRubberBand != null)
                mRubberBand = null;
        }
        #endregion Mouse handling ICanvasViewMouseHandler

        #region CommndImplementation
        #region Selection mode command
        /// <summary>
        /// Determine whether canvas can switch to selection mode or not.
        /// </summary>
        /// <returns></returns>
        private bool OnSelectMode_CanExecute()
        {
            return (CanvasViewMouseHandler != null);
        }

        /// <summary>
        /// Implements the select mode command which switches the viewmodel into the
        /// selection mode. The selection mode allows normal work with canvas elements
        /// by clicking on them.
        /// </summary>
        private void OnSelectMode_Execute()
        {
            CancelCanvasViewMouseHandler();
        }
        #endregion Selection mode command

        #region delete cut copy paste commands
        /// <summary>
        /// Determine whether a selected canvas element can be deleted, cut, or copied.
        /// </summary>
        /// <returns></returns>
        private bool OnDeleteCutCopyCommand_CanExecute()
        {
            return (DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready &&
                    mSelectedItem.Count > 0);
        }

        /// <summary>
        /// Determine whether a Paste command can be executed or not.
        /// </summary>
        /// <returns></returns>
        private bool OnPasteCommand_CanExecute()
        {
            return (DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready && Clipboard.ContainsText());
        }

        /// <summary>
        /// Delete a selected canvas element from the canvas.
        /// </summary>
        private void OnDeleteCommand_Executed()
        {
            DocumentViewModel.dm_DocumentDataModel.BeginOperation("DeleteCommandModel.OnExecute");

            DocumentViewModel.dm_DocumentDataModel.DeleteElements(mSelectedItem.Shapes);

            // Clear selection from selected elements in canvas viewmodel
            mSelectedItem.Clear();

            DocumentViewModel.dm_DocumentDataModel.EndOperation("DeleteCommandModel.OnExecute");
        }

        /// <summary>
        /// Cut currently selected shape(s) into the clipboard.
        /// </summary>
        private void OnCutCommand_Executed()
        {
            DocumentViewModel.dm_DocumentDataModel.BeginOperation("CutCommandModel.OnExecute");

            string fragment = string.Empty;

            if (SelectedItem.Count > 0)
                fragment = DocumentViewModel.dm_DocumentDataModel.GetShapesAsXmlString(SelectedItem.Shapes);

            DocumentViewModel.dm_DocumentDataModel.DeleteElements(mSelectedItem.Shapes);

            mSelectedItem.Clear();
            Clipboard.SetText(fragment);

            DocumentViewModel.dm_DocumentDataModel.EndOperation("CutCommandModel.OnExecute");
        }

        /// <summary>
        /// Copy currently selected shape(s) into the clipboard.
        /// </summary>
        private void OnCopyCommand_Executed()
        {
            string fragment = string.Empty;

            if (SelectedItem.Count > 0)
                fragment = DocumentViewModel.dm_DocumentDataModel.GetShapesAsXmlString(SelectedItem.Shapes);

            Clipboard.SetText(fragment);
        }

        /// <summary>
        /// Paste element from windows clipboard into current selection on to the canvas.
        /// </summary>
        private void OnPasteCommand_Executed()
        {
            try
            {
                string xmlDocument = Clipboard.GetText();

                if (string.IsNullOrEmpty(xmlDocument))
                    return;

                // Look-up plugin model
                string plugin = DocumentViewModel.dm_DocumentDataModel.PluginModelName;
                PluginModelBase m = PluginManager.GetPluginModel(plugin);

                // Look-up shape converter
                UmlTypeToStringConverterBase conv = null;
                conv = m.ShapeConverter;

                List<ShapeViewModelBase> coll;
                // Convert Xml document into a list of shapes and page definition
                PageViewModelBase page = conv.ReadDocument(xmlDocument,
                                                   DocumentViewModel.vm_CanvasViewModel, out coll);

                if (coll == null)
                    return;

                if (coll.Count == 0)
                    return;

                DocumentViewModel.dm_DocumentDataModel.BeginOperation("PasteCommandModel.OnExecute");

                mSelectedItem.Clear();
                foreach (var shape in coll)
                {
                    DocumentViewModel.dm_DocumentDataModel.AddShape(shape);
                    mSelectedItem.Add(shape);
                }

                DocumentViewModel.dm_DocumentDataModel.EndOperation("PasteCommandModel.OnExecute");
            }
            catch
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();

                msgBox.Show(Framework.Local.Strings.STR_MSG_NoShapeInClipboard,
                            Framework.Local.Strings.STR_UnexpectedErrorCaption,
                            MsgBoxButtons.OK, MsgBoxImage.Warning);
            }
        }
        #endregion delete command
        #endregion CommndImplementation

        #region private SameSize methods
        private void SameWidth(ShapeSizeViewModelBase shape)
        {
            if (shape == null)
                return;

            foreach (var item in SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
            {
                if (shape == item) // Adjust shape to itself is a superflous operation
                    continue;

                item.Width = shape.Width;
            }
        }

        private void SameHeight(ShapeSizeViewModelBase shape)
        {
            if (shape == null)
                return;

            foreach (var item in SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
            {
                if (shape == item) // Adjust shape to itself is a superflous operation
                    continue;

                item.Height = shape.Height;
            }
        }

        private void SameWidthandHeight(ShapeSizeViewModelBase shape)
        {
            if (shape == null)
                return;

            foreach (var item in SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
            {
                if (shape == item) // Adjust shape to itself is a superflous operation
                    continue;

                item.Width = shape.Width;
                item.Height = shape.Height;
            }
        }
        #endregion private SameSize methods

        #region private Shape Distribution methods
        /// <summary>
        /// Destribute all selected shapes (if any) over X space evenly.
        /// 
        /// Method is based on:
        /// http://www.codeproject.com/Articles/24681/WPF-Diagram-Designer-Part-4
        /// 
        /// DesignerCanvas.Commands.cs (DistributeHorizontal_Executed method)
        /// </summary>
        /// <param name="shape"></param>
        private void DistributeHorizontal()
        {
            if (SelectedItem.Shapes.Count() <= 1)
                return;

            var selectedItems = from item in SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>()
                                let itemLeft = item.Left
                                orderby itemLeft
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Double.MaxValue;
                double right = Double.MinValue;
                double sumWidth = 0;

                // Compute min(left), max(right), and sum(width) for all selected items
                foreach (var item in selectedItems)
                {
                    left = Math.Min(left, item.Left);
                    right = Math.Max(right, item.Left + item.Width);

                    sumWidth += item.Width;
                }

                double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
                double offset = selectedItems.First().Left;

                foreach (var item in selectedItems)
                {
                    double delta = offset - item.Left;

                    item.Left = item.Left + delta;

                    offset = offset + item.Width + distance;
                }
            }
        }

        /// <summary>
        /// Destribute all selected shapes (if any) over Y space evenly.
        /// 
        /// Method is based on:
        /// http://www.codeproject.com/Articles/24681/WPF-Diagram-Designer-Part-4
        /// 
        /// DesignerCanvas.Commands.cs (DistributeVertical_Executed method)
        /// </summary>
        /// <param name="shape"></param>
        private void DistributeVertical()
        {
            if (SelectedItem.Shapes.Count() <= 1)
                return;

            var selectedItems = from item in SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>()
                                let itemTop = item.Top
                                orderby itemTop
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Double.MaxValue;
                double bottom = Double.MinValue;
                double sumHeight = 0;

                // Compute min(top), max(bottom), and sum(Height) for all selected items
                foreach (var item in selectedItems)
                {
                    top = Math.Min(top, item.Top);
                    bottom = Math.Max(bottom, item.Top + item.Height);
                    sumHeight += item.Height;
                }

                double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
                double offset = selectedItems.First().Top;

                foreach (var item in selectedItems)
                {
                    double delta = offset - item.Top;

                    item.Top = item.Top + delta;

                    offset = offset + item.Height + distance;
                }
            }
        }
        #endregion private Shape Distribution methods
        #endregion methods
    }
}
