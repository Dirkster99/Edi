namespace MiniUML.Model.ViewModels.Document
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using MiniUML.Framework;
    using MiniUML.Framework.Command;
    using MiniUML.Model.Events;
    using MiniUML.Model.Model;
    using MiniUML.Model.ViewModels.RubberBand;
    using MiniUML.Model.ViewModels.Shapes;
    using MiniUML.View.Views.RubberBand;
    using MsgBox;
    using MiniUML.Model.ViewModels.Interfaces;

    public class CanvasViewModel : BaseViewModel, IShapeParent
    {
        #region fields
        private bool _IsFocused;

        private SelectedItems _SelectedItem;

        private RelayCommand<object> _SelectCommand = null;

        private RelayCommand<object> _DeleteCommand = null;
        private RelayCommand<object> _CutCommand = null;
        private RelayCommand<object> _CopyCommand = null;
        private RelayCommand<object> _PasteCommand = null;

        private ICanvasViewMouseHandler _ICanvasViewMouseHandler = null;

        private RubberBandViewModel _RubberBand = null;
        private readonly IMessageBoxService _MsgBox;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="documentViewModel"></param>
        public CanvasViewModel(DocumentViewModel documentViewModel, IMessageBoxService msgBox)
            : this()
        {
            _MsgBox = msgBox;

            // Store a reference to the parent view model
            // (necessary to implement begin and end operation around undo).
            DocumentViewModel = documentViewModel;

            _SelectedItem = new SelectedItems();

            _IsFocused = false;
        }

        /// <summary>
        /// Hidden parameterless constructor
        /// </summary>
        protected CanvasViewModel()
        {

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
                return _IsFocused;
            }

            set
            {
                if (_IsFocused != value)
                {
                    _IsFocused = value;
                    NotifyPropertyChanged(() => this.IsFocused);
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
                return _SelectedItem;
            }
        }

        /// <summary>
        /// Viewmodel to the document represented by this canvas.
        /// </summary>
        public DocumentViewModel DocumentViewModel { get; }

        /// <summary>
        /// Get canvas view model mouse handler which is used to draw
        /// connections between items - using a class that is defined
        /// outside of the actual canvas viewmodel class.
        /// </summary>
        public ICanvasViewMouseHandler CanvasViewMouseHandler
        {
            get
            {
                return _ICanvasViewMouseHandler;
            }

            private set
            {
                if (_ICanvasViewMouseHandler != value)
                {
                    _ICanvasViewMouseHandler = value;
                }
            }
        }

        public RubberBandViewModel RubberBand
        {
            get
            {
                if (_RubberBand == null)
                    _RubberBand = new RubberBandViewModel();

                return _RubberBand;
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
                if (_SelectCommand == null)
                    _SelectCommand = new RelayCommand<object>(
                        (p) => this.OnSelectMode_Execute(),
                        (p) => this.OnSelectMode_CanExecute());

                return _SelectCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Delete a currently selected shape mode.
        /// </summary>
        public RelayCommand<object> DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                    _DeleteCommand = new RelayCommand<object>(
                        (p) => this.OnDeleteCommand_Executed(),
                        (p) => this.OnDeleteCutCopyCommand_CanExecute());

                return _DeleteCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Cut a currently selected shape into the windows clipboard.
        /// </summary>
        public RelayCommand<object> CutCommand
        {
            get
            {
                if (_CutCommand == null)
                    _CutCommand = new RelayCommand<object>(
                        (p) => this.OnCutCommand_Executed(),
                        (p) => this.OnDeleteCutCopyCommand_CanExecute());

                return _CutCommand;
            }
        }

        /// <summary>
        /// Get a command that can be used to Copy a currently selected shape from the windows clipboard.
        /// </summary>
        public RelayCommand<object> CopyCommand
        {
            get
            {
                if (_CopyCommand == null)
                    _CopyCommand = new RelayCommand<object>(
                        (p) => this.OnCopyCommand_Executed(),
                        (p) => this.OnDeleteCutCopyCommand_CanExecute());

                return _CopyCommand;
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
                if (_PasteCommand == null)
                    _PasteCommand = new RelayCommand<object>(
                        (p) => this.OnPasteCommand_Executed(),
                        (p) => this.OnPasteCommand_CanExecute());

                return _PasteCommand;
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
            this.DocumentViewModel.dm_DocumentDataModel.Remove(shape);
        }

        /// <summary>
        /// Brings the shape into front of the canvas view
        /// (moves shape on top of virtual Z-axis)
        /// </summary>
        /// <param name="obj"></param>
        void IShapeParent.BringToFront(ShapeViewModelBase shape)
        {
            this.DocumentViewModel.dm_DocumentDataModel.BringToFront(shape);
        }

        /// <summary>
        /// Brings the shape into the back of the canvas view
        /// (moves shape to the bottom of virtual Z-axis)
        /// </summary>
        /// <param name="obj"></param>
        void IShapeParent.SendToBack(ShapeViewModelBase shape)
        {
            this.DocumentViewModel.dm_DocumentDataModel.SendToBack(shape);
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
            foreach (var item in this.SelectedItem.Shapes)
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
            foreach (var item in this.SelectedItem.Shapes)
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

            foreach (var item in this.SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
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
                    this.SameWidth(shape);
                    break;

                case SameSize.SameHeight:
                    this.SameHeight(shape);
                    break;

                case SameSize.SameWidthandHeight:
                    this.SameWidthandHeight(shape);
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
                    this.DistributeHorizontal();
                    break;

                case Destribute.Vertically:
                    this.DistributeVertical();
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
            this.DocumentViewModel.dm_DocumentDataModel.AddShape(element);
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
            if (this.CanvasViewMouseHandler == value)  // no change
                return;

            if (this.CanvasViewMouseHandler != null)
                this.CancelCanvasViewMouseHandler();

            this.CanvasViewMouseHandler = value;

            if (this.CanvasViewMouseHandler != null)
                DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasViewMouseHandler session");
        }

        /// <summary>
        /// This method is called when the association drawing mode is cancelled
        /// (either via explicit command [click on Select Mode button] or
        /// because the source or target may be in-appropriate).
        /// </summary>
        public void CancelCanvasViewMouseHandler()
        {
            var handler = this.CanvasViewMouseHandler;
            this.CanvasViewMouseHandler = null;
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
                    this.SelectedItem.Clear();

                Rect rubberBand = new Rect(e.StartPoint, e.EndPoint);

                foreach (var item in this.DocumentViewModel.dm_DocumentDataModel.DocRoot.OfType<ShapeSizeViewModelBase>())
                {
                    Rect itemBounds = new Rect(item.Position, item.EndPosition);

                    bool contains = rubberBand.Contains(itemBounds);

                    if (contains == true)
                    {
                        this.SelectedItem.Add(item);
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
            this.CanvasViewMouseHandler = null;
            DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasViewMouseHandler session");
        }

        /// <summary>
        /// Destroy the current rubberband viewmodel to make way
        /// for a new rubber band selection based on a new viewmodel and view.
        /// </summary>
        public void ResetRubberBand()
        {
            if (_RubberBand != null)
                _RubberBand = null;
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
            return (this.CanvasViewMouseHandler != null);
        }

        /// <summary>
        /// Implements the select mode command which switches the viewmodel into the
        /// selection mode. The selection mode allows normal work with canvas elements
        /// by clicking on them.
        /// </summary>
        private void OnSelectMode_Execute()
        {
            this.CancelCanvasViewMouseHandler();
        }
        #endregion Selection mode command

        #region delete cut copy paste commands
        /// <summary>
        /// Determine whether a selected canvas element can be deleted, cut, or copied.
        /// </summary>
        /// <returns></returns>
        private bool OnDeleteCutCopyCommand_CanExecute()
        {
            return (this.DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready &&
                    _SelectedItem.Count > 0);
        }

        /// <summary>
        /// Determine whether a Paste command can be executed or not.
        /// </summary>
        /// <returns></returns>
        private bool OnPasteCommand_CanExecute()
        {
            return (this.DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready && Clipboard.ContainsText());
        }

        /// <summary>
        /// Delete a selected canvas element from the canvas.
        /// </summary>
        private void OnDeleteCommand_Executed()
        {
            try
            {
                DocumentViewModel.dm_DocumentDataModel.BeginOperation("DeleteCommandModel.OnExecute");

                DocumentViewModel.dm_DocumentDataModel.DeleteElements(_SelectedItem.Shapes);

                // Clear selection from selected elements in canvas viewmodel
                _SelectedItem.Clear();
            }
            finally
            {
                DocumentViewModel.dm_DocumentDataModel.EndOperation("DeleteCommandModel.OnExecute");
            }
        }

        /// <summary>
        /// Cut currently selected shape(s) into the clipboard.
        /// </summary>
        private void OnCutCommand_Executed()
        {
            this.DocumentViewModel.dm_DocumentDataModel.BeginOperation("CutCommandModel.OnExecute");

            string fragment = string.Empty;

            if (this.SelectedItem.Count > 0)
                fragment = this.DocumentViewModel.dm_DocumentDataModel.GetShapesAsXmlString(this.SelectedItem.Shapes);

            this.DocumentViewModel.dm_DocumentDataModel.DeleteElements(_SelectedItem.Shapes);

            _SelectedItem.Clear();
            Clipboard.SetText(fragment);

            this.DocumentViewModel.dm_DocumentDataModel.EndOperation("CutCommandModel.OnExecute");
        }

        /// <summary>
        /// Copy currently selected shape(s) into the clipboard.
        /// </summary>
        private void OnCopyCommand_Executed()
        {
            string fragment = string.Empty;

            if (this.SelectedItem.Count > 0)
                fragment = this.DocumentViewModel.dm_DocumentDataModel.GetShapesAsXmlString(this.SelectedItem.Shapes);

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

                if (string.IsNullOrEmpty(xmlDocument) == true)
                    return;

                // Look-up plugin model
                string plugin = this.DocumentViewModel.dm_DocumentDataModel.PluginModelName;
                PluginModelBase m = PluginManager.GetPluginModel(plugin);

                // Look-up shape converter
                UmlTypeToStringConverterBase conv = null;
                conv = m.ShapeConverter;

                List<ShapeViewModelBase> coll;
                // Convert Xml document into a list of shapes and page definition
                PageViewModelBase page = conv.ReadDocument(xmlDocument,
                                                   this.DocumentViewModel.vm_CanvasViewModel, out coll);

                if (coll == null)
                    return;

                if (coll.Count == 0)
                    return;

                this.DocumentViewModel.dm_DocumentDataModel.BeginOperation("PasteCommandModel.OnExecute");

                _SelectedItem.Clear();
                foreach (var shape in coll)
                {
                    this.DocumentViewModel.dm_DocumentDataModel.AddShape(shape);
                    _SelectedItem.Add(shape);
                }

                this.DocumentViewModel.dm_DocumentDataModel.EndOperation("PasteCommandModel.OnExecute");
            }
            catch
            {
                _MsgBox.Show(MiniUML.Framework.Local.Strings.STR_MSG_NoShapeInClipboard,
                            MiniUML.Framework.Local.Strings.STR_UnexpectedErrorCaption,
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

            foreach (var item in this.SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
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

            foreach (var item in this.SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
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

            foreach (var item in this.SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>())
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
            if (this.SelectedItem.Shapes.Count() <= 1)
                return;

            var selectedItems = from item in this.SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>()
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
            if (this.SelectedItem.Shapes.Count() <= 1)
                return;

            var selectedItems = from item in this.SelectedItem.Shapes.OfType<ShapeSizeViewModelBase>()
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
