﻿namespace MiniUML.View.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using Framework;
    using Framework.interfaces;
    using Model.ViewModels.Document;
    using Model.ViewModels.RubberBand;
    using Model.ViewModels.Shapes;
    using Controls;
    using RubberBand;

    public delegate void LayoutUpdatedHandler();

    /// <summary>
    /// Interaction logic for CanvasView.xaml
    /// </summary>
    public partial class CanvasView : UserControl, ICanvasViewMouseHandler
    {
        #region fields
        #region commandbindings
        /// <summary>
        /// List of command bindings supported by this view
        /// </summary>
        private static readonly List<CommandBinding> CmdBindings = new List<CommandBinding>();
        ////static readonly List<InputBinding> InputBindings = new List<InputBinding>();
        #endregion commandbindings

        private static readonly DependencyProperty CustomDragProperty =
                               DependencyProperty.RegisterAttached("CustomDrag",
                                                                   typeof(bool), typeof(CanvasView),
                                                                   new FrameworkPropertyMetadata(false));

        private static BooleanToVisibilityConverter mBoolToVisConverter;

        private LeftMouseButton mGotMouseDown = LeftMouseButton.IsNotClicked;

        private ICanvasViewMouseHandler mCurrentMouseHandler;

        // start point of the rubberband drag operation
        ////private Point? mRubberbandSelectionStartPoint = null;
        private RubberbandAdorner mRubberbandAdorner;

        /// <summary>
        /// Mouse position at drag start / last drag update.
        /// </summary>
        private Point mDragStart;

        /// <summary>
        /// Shape under mouse at drag start.
        /// </summary>
        private ShapeViewModelBase mDragShape;

        private HashSet<LayoutUpdatedHandler> mLayoutUpdatedHandlers = new HashSet<LayoutUpdatedHandler>();

        private ItemsControl mPart_ItemsControl;
        #endregion fields

        #region constructor
        /// <summary>
        /// static class constructor
        /// </summary>
        static CanvasView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CanvasView), new FrameworkPropertyMetadata(typeof(CanvasView)));

            CmdBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCopy));
            CmdBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCut));
            CmdBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
            CmdBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete, CanDelete));
            CmdBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndo, CanUndo));
            CmdBindings.Add(new CommandBinding(ApplicationCommands.Redo, OnRedo, CanRedo));

            mBoolToVisConverter = new BooleanToVisibilityConverter();
        }

        /// <summary>
        /// class constructor
        /// </summary>
        public CanvasView()
        {
            // Copy static collection of commands to collection of commands of this instance
            CommandBindings.AddRange(CmdBindings);

            LayoutUpdated += canvas_LayoutUpdated;

            DataContextChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
            {
                if (CanvasViewModel != null)
                    CanvasViewModel.SelectedItem.SelectionChanged -= model_SelectionChanged;

                CanvasViewModel = (CanvasViewModel)DataContext;

                if (CanvasViewModel != null)
                    CanvasViewModel.SelectedItem.SelectionChanged += model_SelectionChanged;
            };
        }
        #endregion constructor

        /// <summary>
        /// Determine whether left mouse button is clicked or not.
        /// </summary>
        private enum LeftMouseButton
        {
            IsClicked,
            IsNotClicked
        }

        #region properties
        public CanvasViewModel CanvasViewModel { get; private set; }
        #endregion properties

        #region methods
        #region public methods
        #region CustomDrag dependency property
        /// <summary>
        /// Set property implementation for dependency property.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetCustomDrag(UIElement element, bool value)
        {
            element.SetValue(CustomDragProperty, value);
        }

        /// <summary>
        /// Get property implementation for dependency property.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetCustomDrag(UIElement element)
        {
            return (bool)element.GetValue(CustomDragProperty);
        }
        #endregion CustomDrag dependency property

        public static CanvasView GetCanvasView(DependencyObject obj)
        {
            while (obj != null)
            {
                if (obj is CanvasView)
                {
                    CanvasView cv = obj as CanvasView;
                    return cv;
                }

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }

        #region ICanvasViewMouseHandler Members
        /// <summary>
        /// Encapsulates functionality of shape selection by mouse
        /// The user can use the left or right ctrl key to toggle
        /// the selection of <paramref name="shape"/> on or off.
        /// </summary>
        /// <param name="shape"></param>
        void ICanvasViewMouseHandler.OnShapeClick(ShapeViewModelBase shape)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (shape == null)  // Click on canvas with CTRL key has no effect on seletion
                    return;

                // Toggle shape selection
                // equivalent expression is _CanvasViewModel.SelectedItem.Contains(shape)
                if (shape.IsSelected)
                    CanvasViewModel.SelectedItem.Remove(shape);
                else
                    CanvasViewModel.SelectedItem.Add(shape);
            }
            else
            {
                // Click on canvas without pressing a CTRL key down
                // Update currently selected shapes - removes all selections if shape == null
                CanvasViewModel.SelectedItem.SelectShape(shape);
            }
        }

        /// <summary>
        /// Is called if the user begins a drag operation on the canvas and there is no other
        /// mouse handler (eg, AssociationMouseHandler) currently activated.
        /// 
        /// Otherwise, the OnShapeDragBegin method of the other mouse handler is executed and
        /// this method is completely ignorred for the current drag operation.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="shape"></param>
        void ICanvasViewMouseHandler.OnShapeDragBegin(Point position, ShapeViewModelBase shape)
        {
            if (shape != null)
            {
                // Activate shape selection if user starts to drag a shape
                if (shape.IsSelected == false)
                    ((ICanvasViewMouseHandler)this).OnShapeClick(mDragShape);
            }
            else // user starts a drag operation on the canvas so we setup a rubber band adorner command
            {   // to implement multi object selection via rubber band
                if (mCurrentMouseHandler == this)
                {
                    // Clear current selection if no control key is pressed on the keyboard (which would allow adding selected items)
                    if ((Keyboard.IsKeyDown(Key.LeftCtrl) == false &&
                         Keyboard.IsKeyDown(Key.RightCtrl) == false))
                    {
                        CanvasViewModel.SelectedItem.SelectShape(null);
                    }

                    RubberBandViewModel vm = GetRubberBand(position);

                    // Create a new mouse handler command, attach event on completion, and hook it into the system
                    CreateRubberBandMouseHandler handler = new CreateRubberBandMouseHandler(vm, CanvasViewModel);
                    handler.RubberBandSelection += handler_RubberBandSelection;

                    // Hook up into frame work system
                    CanvasViewModel.BeginCanvasViewMouseHandler(handler);
                    BeginMouseOperation();
                    //this.mCurrentMouseHandler = this.CanvasViewModel.CanvasViewMouseHandler;
                }
            }
        }

        private void handler_RubberBandSelection(object sender, RubberBandSelectionEventArgs e)
        {

            if (mCurrentMouseHandler is CreateRubberBandMouseHandler)
            {
                CreateRubberBandMouseHandler handler = mCurrentMouseHandler as CreateRubberBandMouseHandler;
                handler.RubberBandSelection -= handler_RubberBandSelection;
            }

            CanvasViewModel.Handle_RubberBandSelection(e);

            EndMouseOperation();
            DestroyRubberband();
        }

        /// <summary>
        /// Method is executed if a shape is dragged across the canvas.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="delta"></param>
        void ICanvasViewMouseHandler.OnShapeDragUpdate(Point position, Vector delta)
        {
            if (mPart_ItemsControl == null)
                return;

            foreach (ShapeViewModelBase shape in CanvasViewModel.SelectedItem.Shapes)
            {
                FrameworkElement control = ControlFromElement(shape);

                if ((bool)control.GetValue(CustomDragProperty))
                    continue;

                // Get current position of shape
                Point origin = shape.Position;

                // Move shape by this delta in X and Y
                Point p = origin + delta;

                // Check whether the new position is legal and optimize based on the canvas view size
                if (p.X < 0)
                    p.X = 0;
                else
                {
                    if (p.X + control.ActualWidth > mPart_ItemsControl.ActualWidth)
                        p.X = mPart_ItemsControl.ActualWidth - control.ActualWidth;
                }

                if (p.Y < 0)
                    p.Y = 0;
                else
                {
                    if (p.Y + control.ActualHeight > mPart_ItemsControl.ActualHeight)
                        p.Y = mPart_ItemsControl.ActualHeight - control.ActualHeight;
                }

                // Set shape to new position
                shape.Position = p;

                // Invoke snap event if possible to let lines adjust correctly
                if (control is ISnapTarget)
                {
                    ISnapTarget ist = control as ISnapTarget;
                    ist.NotifySnapTargetUpdate(new SnapTargetUpdateEventArgs(p - origin));
                }
            }
        }

        void ICanvasViewMouseHandler.OnShapeDragEnd(Point position, ShapeViewModelBase element)
        {
            CanvasViewModel viewModel = DataContext as CanvasViewModel;
        }

        void ICanvasViewMouseHandler.OnCancelMouseHandler()
        {
            throw new NotImplementedException();
        }
        #endregion ICanvasViewMouseHandler interface

        #region Coercion
        public void NotifyOnLayoutUpdated(LayoutUpdatedHandler handler)
        {
            mLayoutUpdatedHandlers.Add(handler);
        }
        #endregion Coercion

        /// <summary>
        /// Standard method that is executed when a WPF template
        /// is applied to a lookless control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mPart_ItemsControl = GetTemplateChild("Part_ItemsControl") as ItemsControl;
        }

        #region Utility methods
        public ShapeViewModelBase GetShapeAt(Point p)
        {
            if (mPart_ItemsControl == null)
                return null;

            DependencyObject hitObject = (DependencyObject)mPart_ItemsControl.InputHitTest(p);

            // Workaround: For reasons unknown, InputHitTest sometimes return null when it clearly should not.
            // This appears to be a framework bug.
            if (hitObject == null)
                return null;

            // If hitObject is not a visual, we need to find the visual parent.
            // Thus we loop as long as we're dealing with a FrameworkContentElement.
            // Only FrameworkContentElements expose a Parent property, so we cast.
            // (If we find a generic ContentElement, something has gone horribly wrong.)
            while (hitObject is FrameworkContentElement)
                hitObject = ((FrameworkContentElement)hitObject).Parent;

            ContentPresenter presenter = null;

            do
            {
                if (hitObject is ContentPresenter)
                    presenter = (ContentPresenter)hitObject;

                hitObject = VisualTreeHelper.GetParent(hitObject);
            }
            while (hitObject != mPart_ItemsControl);

            // Something's wrong: We clicked a control not wrapped in a ContentPresenter... Never mind, then.
            if (presenter == null)
                return null;

            var element = mPart_ItemsControl.ItemContainerGenerator.ItemFromContainer(presenter);

            return (element == DependencyProperty.UnsetValue) ? null : (ShapeViewModelBase)element;
        }

        /// <summary>
        /// Get a shapes view in the CanvasView that is associated with a given shape viewmodel.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public UIElement PresenterFromElement(ShapeViewModelBase element)
        {
            if (element == null)
                return null;

            if (mPart_ItemsControl == null)
                return null;

            return (UIElement)mPart_ItemsControl.ItemContainerGenerator.ContainerFromItem(element);
        }

        public FrameworkElement ControlFromElement(ShapeViewModelBase element)
        {
            if (element == null)
                return null;

            if (mPart_ItemsControl == null)
                return null;

            // TODO XXX Dirkster Bug ???
            DependencyObject dob = mPart_ItemsControl.ItemContainerGenerator.ContainerFromItem(element);

            if (dob == null)
                return null;

            // Fix for exception:
            // VisualTreeHelper.GetChild
            // Message: Specified index is out of range or child at index is null. Do not call this method if
            //          VisualChildrenCount returns zero, indicating that the Visual has no children.
            //          (System.ArgumentOutOfRangeException)
            if (VisualTreeHelper.GetChildrenCount(dob) == 0)
                return null;

            return (FrameworkElement)VisualTreeHelper.GetChild(dob, 0);
        }

        public ShapeViewModelBase ElementFromControl(DependencyObject shape)
        {
            if (mPart_ItemsControl == null)
                return null;

            while (shape != null)
            {

                if (mPart_ItemsControl.ItemContainerGenerator.ItemFromContainer(shape) is ShapeViewModelBase)
                {
                    ShapeViewModelBase item = mPart_ItemsControl.ItemContainerGenerator.ItemFromContainer(shape) as ShapeViewModelBase;
                    return item;
                }

                shape = VisualTreeHelper.GetParent(shape);
            }

            return null;
        }
        #endregion
        #endregion public methods

        #region protected methods
        /// <summary>
        /// Handles the PreviewMouseDown event of the canvas.
        /// If CanvasViewMouseHandler is set, we handle all mouse button events. Otherwise, only left-clicks.
        /// </summary>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (mPart_ItemsControl == null)
                return;

            if (mGotMouseDown == LeftMouseButton.IsClicked ||
               (CanvasViewModel.CanvasViewMouseHandler == null && e.ChangedButton != MouseButton.Left))
            {
                base.OnPreviewMouseDown(e);
                return;
            }

            BeginMouseOperation();

            mDragStart = e.GetPosition(mPart_ItemsControl);
            mDragShape = GetShapeAt(mDragStart);

            e.Handled = (CanvasViewModel.CanvasViewMouseHandler != null) || (CanvasViewModel.SelectedItem.Count > 1);
        }

        /// <summary>
        /// Handles the PreviewMouseUp event of the canvas.
        /// </summary>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (mPart_ItemsControl == null)
                return;

            if (mGotMouseDown == LeftMouseButton.IsNotClicked)
                return;

            Point position = e.GetPosition(mPart_ItemsControl);
            Vector dragDelta = position - mDragStart;

            if (IsMouseCaptured == false)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                    return;                                      // We're not dragging anything.

                // This CanvasView is not responsible for the dragging.
                if (IsMouseCaptureWithin)
                    return;

                if (Math.Abs(dragDelta.X) < SystemParameters.MinimumHorizontalDragDistance &&
                    Math.Abs(dragDelta.Y) < SystemParameters.MinimumVerticalDragDistance)
                    return;

                mCurrentMouseHandler.OnShapeDragBegin(position, mDragShape);

                CaptureMouse();
            }

            mCurrentMouseHandler.OnShapeDragUpdate(position, dragDelta);

            // The new "drag start" is the current mouse position.
            mDragStart = position;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (mPart_ItemsControl == null)
                return;

            if (mGotMouseDown == LeftMouseButton.IsNotClicked)
                return;

            // This try block ends with an EndMouseOperation which chnages/checks the datamodel state
            // be careful when editing this part !!!
            try
            {
                if (mPart_ItemsControl != null)
                {
                    Point position = e.GetPosition(mPart_ItemsControl);

                    if (IsMouseCaptured == false)
                    {
                        mCurrentMouseHandler.OnShapeClick(mDragShape);
                        return;
                    }

                    ReleaseMouseCapture();

                    mCurrentMouseHandler.OnShapeDragUpdate(position, position - mDragStart);
                    mCurrentMouseHandler.OnShapeDragEnd(position, GetShapeAt(position));
                }
            }
            finally
            {
                EndMouseOperation();
            }

            /* HACK: Work-around for bug 4
            this._CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.Undo();
            this._CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.Redo();
             * */
        }

        /// <summary>
        /// Use mouse click event to get focus
        /// (ApplicationCommands.Copy, Cut, Paste will not work otherwise) 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Source == this)
            {
                if (IsFocused == false)
                    Focus();
            }
            else
            {
                // in case that this click is the start for a drag operation we cache the start point
                if (mCurrentMouseHandler != this && mCurrentMouseHandler != null)
                {
                    Point position = e.GetPosition(this);

                    mCurrentMouseHandler.OnShapeDragBegin(position, null);
                }
            }

            e.Handled = true;
        }

        #region Drag/Drop functionality
        /// <summary>
        /// Handles the Drop event of the canvas.
        /// When a IDragableCommand, which represents an item from a ribbon gallery, is dropped on the canvas, its OnDragDropExecute is called.
        /// </summary>
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (mPart_ItemsControl == null)
                return;

            if (e.Data.GetDataPresent(typeof(IDragableCommandModel)))
            {
                IDragableCommandModel cmd = (IDragableCommandModel)e.Data.GetData(typeof(IDragableCommandModel));

                cmd.OnDragDropExecute(e.GetPosition(mPart_ItemsControl));
                e.Handled = true;
                return;
            }

            if (e.Data.GetDataPresent(typeof(Framework.helpers.DragObject)))
            {
                try
                {
                    Framework.helpers.DragObject dragObject = (Framework.helpers.DragObject)e.Data.GetData(typeof(Framework.helpers.DragObject));

                    IDragableCommandModel c = dragObject.ObjectInstance as IDragableCommandModel;

                    if (c == null)
                        return;

                    Point p = e.GetPosition(mPart_ItemsControl);

                    if (p != null)
                        c.OnDragDropExecute(p);

                    return;
                }
                catch (Exception ex)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(ex, "Drag & Drop operation aborted on error",
                                    "An erro occurred", MsgBoxButtons.OK);
                }
            }

            string fileName = IsSingleFile(e);
            if (fileName != null)
            {
                // Check if the datamodel is ready
                if (!(CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
                      CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid))
                    return;

                Application.Current.MainWindow.Activate();

                if (CanvasViewModel.DocumentViewModel.QuerySaveChanges() == false)
                    return;

                try
                {
                    // Open the document.
                    CanvasViewModel.DocumentViewModel.LoadFile(fileName);
                }
                catch (Exception ex)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(ex, string.Format(Framework.Local.Strings.STR_OpenFILE_MSG, fileName),
                                Framework.Local.Strings.STR_OpenFILE_MSG_CAPTION, MsgBoxButtons.OK);
                }

                return;
            }
        }

        /// <summary>
        /// Method is executed when the user drags an item over the canvas.
        /// The <seealso cref="DragDropEffects"/> enumeration is used to signal
        /// wheter drag & drop operation can continue or not (system shows a plus
        /// sign or stop sign on drag over)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(IDragableCommandModel)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                // the user dragged a toolbox item over wrapped in a DragObject over the canvas
                if (e.Data.GetDataPresent(typeof(Framework.helpers.DragObject)))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else if (IsSingleFile(e) != null)
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                    Console.WriteLine(e.Data.GetType().ToString());
            }

            e.Handled = true;
        }
        #endregion protected mthods
        #endregion

        #region private methods
        #region Clipboard and standard appliaction commands
        #region static methods
        private static void CanCopy(object target, CanExecuteRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandCopy_CanExecute(target, args);
        }

        private static void CanCut(object target, CanExecuteRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandCut_CanExecute(target, args);
        }

        private static void CanPaste(object target, CanExecuteRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandPaste_CanExecute(target, args);
        }

        private static void CanDelete(object target, CanExecuteRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandDelete_CanExecute(target, args);
        }

        private static void CanUndo(object target, CanExecuteRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandUndo_CanExecute(target, args);
        }

        private static void CanRedo(object target, CanExecuteRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandRedo_CanExecute(target, args);
        }

        private static void OnCopy(object target, ExecutedRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandCopy_Executed(target, args);
            args.Handled = true;
        }

        private static void OnCut(object target, ExecutedRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandCut_Executed(target, args);
            args.Handled = true;
        }

        private static void OnPaste(object target, ExecutedRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandPaste_Executed(target, args);
            args.Handled = true;
        }

        private static void OnUndo(object target, ExecutedRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandUndo_Executed(target, args);
            args.Handled = true;
        }

        private static void OnRedo(object target, ExecutedRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandRedo_Executed(target, args);
            args.Handled = true;
        }

        private static void OnDelete(object target, ExecutedRoutedEventArgs args)
        {
            CanvasView cv = target as CanvasView;

            if (cv == null)
                return;

            cv.CommandDelete_Executed(target, args);
            args.Handled = true;
        }
        #endregion static methods

        private void BeginMouseOperation()
        {
            //// DebugUtilities.Assert(_gotMouseDown == false, "beginMouseOperation called when already in mouse operation");
            mGotMouseDown = LeftMouseButton.IsClicked;

            // Use the handler specified on Model, if not null. Otherwise, use ourself
            // Default is the ICanvasViewMouseHandler interface implementation of the CanvasView
            mCurrentMouseHandler = CanvasViewModel.CanvasViewMouseHandler != null ?
                                        CanvasViewModel.CanvasViewMouseHandler : this;

            // Don't create undo states at every drag update.
            CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasView mouse operation");
        }

        private void EndMouseOperation()
        {
            mGotMouseDown = LeftMouseButton.IsNotClicked;
            mCurrentMouseHandler = null;

            // Re-enable the data model.
            CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasView mouse operation");
        }

        #region Delete Command
        private void CommandDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (CanvasViewModel != null)
                e.CanExecute = CanvasViewModel.DeleteCommand.CanExecute(e.Parameter);

            e.Handled = true;
        }

        private void CommandDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanvasViewModel != null)
                CanvasViewModel.DeleteCommand.Execute(e.Parameter);

            e.Handled = true;
        }
        #endregion Delete Command

        #region Copy Command
        private void CommandCopy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (CanvasViewModel != null)
                e.CanExecute = CanvasViewModel.CopyCommand.CanExecute(e.Parameter);

            e.Handled = true;
        }

        private void CommandCopy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanvasViewModel != null)
                CanvasViewModel.CopyCommand.Execute(e.Parameter);

            e.Handled = true;
        }
        #endregion Copy Command

        #region Cut Command
        private void CommandCut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (CanvasViewModel != null)
                e.CanExecute = CanvasViewModel.CutCommand.CanExecute(e.Parameter);

            e.Handled = true;
        }

        private void CommandCut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanvasViewModel != null)
                CanvasViewModel.CutCommand.Execute(e.Parameter);

            e.Handled = true;
        }
        #endregion Cut Command

        #region Paste Command
        private void CommandPaste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (CanvasViewModel != null)
                e.CanExecute = CanvasViewModel.PasteCommand.CanExecute(e.Parameter);

            e.Handled = true;
        }

        private void CommandPaste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanvasViewModel != null)
                CanvasViewModel.PasteCommand.Execute(e.Parameter);

            e.Handled = true;
        }
        #endregion Paste Command

        #region Undo Command
        private void CommandUndo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (CanvasViewModel == null)
                return;

            if (CanvasViewModel.DocumentViewModel == null)
                return;

            CanvasViewModel.DocumentViewModel.cmd_Undo.OnQueryEnabled(sender, e);
            e.Handled = true;
        }

        private void CommandUndo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanvasViewModel == null)
                return;

            if (CanvasViewModel.DocumentViewModel == null)
                return;

            CanvasViewModel.DocumentViewModel.cmd_Undo.OnExecute(sender, e);
        }
        #endregion Undo Command

        #region Redo Command
        private void CommandRedo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (CanvasViewModel == null)
                return;

            if (CanvasViewModel.DocumentViewModel == null)
                return;

            CanvasViewModel.DocumentViewModel.cmd_Redo.OnQueryEnabled(sender, e);
            e.Handled = true;
        }

        private void CommandRedo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanvasViewModel == null)
                return;

            if (CanvasViewModel.DocumentViewModel == null)
                return;

            CanvasViewModel.DocumentViewModel.cmd_Redo.OnExecute(sender, e);
        }
        #endregion Redo Command
        #endregion Clipboard and standard appliaction commands

        #region Coercion
        private void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            if (CanvasViewModel == null)
                return;

            HashSet<LayoutUpdatedHandler> set = mLayoutUpdatedHandlers;

            if (set.Count == 0)
                return;

            mLayoutUpdatedHandlers = new HashSet<LayoutUpdatedHandler>();

            try
            {
                CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasView.canvas_LayoutUpdated");

                foreach (LayoutUpdatedHandler handler in set)
                    handler();
            }
            finally
            {
                CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.EndOperationWithoutCreatingUndoState("CanvasView.canvas_LayoutUpdated");
            }
        }
        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the view model.
        /// When the collection of selected shapes changes, the Selector.IsSelectedProperty
        /// is updated for all elements on the canvas.
        /// </summary>
        private void model_SelectionChanged(object sender, EventArgs e)
        {
            if (mPart_ItemsControl == null)
                return;

            foreach (ShapeViewModelBase shape in mPart_ItemsControl.Items)
            {
                mPart_ItemsControl.ItemContainerGenerator.ContainerFromItem(shape).SetValue(
                                           Selector.IsSelectedProperty, CanvasViewModel.SelectedItem.Contains(shape));
            }
        }

        /// <summary>
        /// Standard method to check the drag data; found in documentation.
        /// If the data object in args is a single file, this method will return the filename.
        /// Otherwise, it returns null.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string IsSingleFile(DragEventArgs args)
        {
            // Check for files in the hovering data object.
            if (args.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];

                // Check fo a single file or folder.
                if (fileNames.Length == 1)
                {
                    // Check for a file (a directory will return false).
                    if (File.Exists(fileNames[0]))
                    {
                        // At this point we know there is a single file.
                        return fileNames[0];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Destroy rubber band adorner resources
        /// </summary>
        private void DestroyRubberband()
        {
            if (mRubberbandAdorner != null)
            {
                try
                {
                    mRubberbandAdorner.Visibility = Visibility.Hidden;

                    CanvasViewModel.ResetRubberBand();

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                    if (adornerLayer != null)
                    {
                        adornerLayer.Remove(mRubberbandAdorner);
                    }
                }
                finally
                {
                    mRubberbandAdorner = null;
                }
            }
        }

        /// <summary>
        /// Create a new rubber band adorner along with its viewmodel in this canvas viewmodel.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private RubberBandViewModel GetRubberBand(Point position)
        {
            DestroyRubberband();

            RubberBandViewModel rbViewModel = CanvasViewModel.RubberBand;
            rbViewModel.IsVisible = false;
            rbViewModel.Position = new Point(position.X, position.Y);
            rbViewModel.EndPosition = new Point(position.X, position.Y);

            if (mRubberbandAdorner == null)
            {
                mRubberbandAdorner = new RubberbandAdorner(this, new Point(position.X, position.Y))
                {
                    Visibility = Visibility.Hidden
                };
                mRubberbandAdorner.Width = mRubberbandAdorner.Height = 0;

                // EndPosition binding with converter
                {
                    var endPosBinding = new Binding("EndPosition")
                    {
                        Source = rbViewModel
                    };
                    BindingOperations.SetBinding(mRubberbandAdorner, RubberbandAdorner.EndPointProperty, endPosBinding);
                }

                // Visibility binding with converter
                {
                    var visiblityBinding = new Binding("IsVisible")
                    {
                        Source = rbViewModel,
                        Converter = mBoolToVisConverter
                    };
                    BindingOperations.SetBinding(mRubberbandAdorner, VisibilityProperty, visiblityBinding);
                }

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null)
                    adornerLayer.Add(mRubberbandAdorner);
            }

            rbViewModel.IsVisible = true;

            return rbViewModel;
        }
        #endregion private methods
        #endregion methods
    }
}
