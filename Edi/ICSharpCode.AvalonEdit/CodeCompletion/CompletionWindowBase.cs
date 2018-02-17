﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// Base class for completion windows. Handles positioning the window at the caret.
	/// </summary>
	public class CompletionWindowBase : Window
	{
		static CompletionWindowBase()
		{
			WindowStyleProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(WindowStyle.None));
			ShowActivatedProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
			ShowInTaskbarProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));

            //Dirkster99 BugFix to get rid of window border on completion drop-down list
            AllowsTransparencyProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Gets the parent TextArea.
        /// </summary>
        public TextArea TextArea { get; private set; }
		
		Window parentWindow;
		TextDocument document;
		
		/// <summary>
		/// Gets/Sets the start of the text range in which the completion window stays open.
		/// This text portion is used to determine the text used to select an entry in the completion list by typing.
		/// </summary>
		public int StartOffset { get; set; }
		
		/// <summary>
		/// Gets/Sets the end of the text range in which the completion window stays open.
		/// This text portion is used to determine the text used to select an entry in the completion list by typing.
		/// </summary>
		public int EndOffset { get; set; }
		
		/// <summary>
		/// Gets whether the window was opened above the current line.
		/// </summary>
		protected bool IsUp { get; private set; }
		
		/// <summary>
		/// Creates a new CompletionWindowBase.
		/// </summary>
		public CompletionWindowBase(TextArea textArea)
		{
		    TextArea = textArea ?? throw new ArgumentNullException(nameof(textArea));
			parentWindow = GetWindow(textArea);
			Owner = parentWindow;
			AddHandler(MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);
			
			StartOffset = EndOffset = TextArea.Caret.Offset;
			
			AttachEvents();
		}
		
		#region Event Handlers
		void AttachEvents()
		{
			document = TextArea.Document;
			if (document != null) {
				document.Changing += textArea_Document_Changing;
			}
			// LostKeyboardFocus seems to be more reliable than PreviewLostKeyboardFocus - see SD-1729
			TextArea.LostKeyboardFocus += TextAreaLostFocus;
			TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
			TextArea.DocumentChanged += TextAreaDocumentChanged;
			if (parentWindow != null) {
				parentWindow.LocationChanged += parentWindow_LocationChanged;
			}
			
			// close previous completion windows of same type
			foreach (InputHandler x in TextArea.StackedInputHandlers.OfType<InputHandler>()) {
				if (x.window.GetType() == GetType())
					TextArea.PopStackedInputHandler(x);
			}
			
			myInputHandler = new InputHandler(this);
			TextArea.PushStackedInputHandler(myInputHandler);
		}
		
		/// <summary>
		/// Detaches events from the text area.
		/// </summary>
		protected virtual void DetachEvents()
		{
			if (document != null) {
				document.Changing -= textArea_Document_Changing;
			}
			TextArea.LostKeyboardFocus -= TextAreaLostFocus;
			TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
			TextArea.DocumentChanged -= TextAreaDocumentChanged;
			if (parentWindow != null) {
				parentWindow.LocationChanged -= parentWindow_LocationChanged;
			}
			TextArea.PopStackedInputHandler(myInputHandler);
		}
		
		#region InputHandler
		InputHandler myInputHandler;
		
		/// <summary>
		/// A dummy input handler (that justs invokes the default input handler).
		/// This is used to ensure the completion window closes when any other input handler
		/// becomes active.
		/// </summary>
		sealed class InputHandler : TextAreaStackedInputHandler
		{
			internal readonly CompletionWindowBase window;
			
			public InputHandler(CompletionWindowBase window)
				: base(window.TextArea)
			{
				Debug.Assert(window != null);
				this.window = window;
			}
			
			public override void Detach()
			{
				base.Detach();
				window.Close();
			}
			
			const Key KeyDeadCharProcessed = (Key)0xac; // Key.DeadCharProcessed; // new in .NET 4
			
			public override void OnPreviewKeyDown(KeyEventArgs e)
			{
				// prevents crash when typing deadchar while CC window is open
				if (e.Key == KeyDeadCharProcessed)
					return;
				e.Handled = RaiseEventPair(window, PreviewKeyDownEvent, KeyDownEvent,
				                           new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
			}
			
			public override void OnPreviewKeyUp(KeyEventArgs e)
			{
				if (e.Key == KeyDeadCharProcessed)
					return;
				e.Handled = RaiseEventPair(window, PreviewKeyUpEvent, KeyUpEvent,
				                           new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
			}
		}
		#endregion
		
		void TextViewScrollOffsetChanged(object sender, EventArgs e)
		{
			// Workaround for crash #1580 (reproduction steps unknown):
			// NullReferenceException in System.Windows.Window.CreateSourceWindow()
			if (!sourceIsInitialized)
				return;
			
			IScrollInfo scrollInfo = TextArea.TextView;
			Rect visibleRect = new Rect(scrollInfo.HorizontalOffset, scrollInfo.VerticalOffset, scrollInfo.ViewportWidth, scrollInfo.ViewportHeight);
			// close completion window when the user scrolls so far that the anchor position is leaving the visible area
			if (visibleRect.Contains(visualLocation) || visibleRect.Contains(visualLocationTop))
				UpdatePosition();
			else
				Close();
		}
		
		void TextAreaDocumentChanged(object sender, EventArgs e)
		{
			Close();
		}
		
		void TextAreaLostFocus(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
		}
		
		void parentWindow_LocationChanged(object sender, EventArgs e)
		{
			UpdatePosition();
		}
		
		/// <inheritdoc/>
		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
		}
		#endregion
		
		/// <summary>
		/// Raises a tunnel/bubble event pair for a WPF control.
		/// </summary>
		/// <param name="target">The WPF control for which the event should be raised.</param>
		/// <param name="previewEvent">The tunneling event.</param>
		/// <param name="event">The bubbling event.</param>
		/// <param name="args">The event args to use.</param>
		/// <returns>The <see cref="RoutedEventArgs.Handled"/> value of the event args.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		protected static bool RaiseEventPair(UIElement target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
            if (args == null)
				throw new ArgumentNullException(nameof(args));
			args.RoutedEvent = previewEvent ?? throw new ArgumentNullException(nameof(previewEvent));
			target.RaiseEvent(args);
			args.RoutedEvent = @event ?? throw new ArgumentNullException(nameof(@event));
			target.RaiseEvent(args);
			return args.Handled;
		}
		
		// Special handler: handledEventsToo
		void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			ActivateParentWindow();
		}
		
		/// <summary>
		/// Activates the parent window.
		/// </summary>
		protected virtual void ActivateParentWindow()
		{
		    parentWindow?.Activate();
		}
		
		void CloseIfFocusLost()
		{
			if (CloseOnFocusLost) {
				Debug.WriteLine("CloseIfFocusLost: this.IsActive=" + IsActive + " IsTextAreaFocused=" + IsTextAreaFocused);
				if (!IsActive && !IsTextAreaFocused) {
					Close();
				}
			}
		}
		
		/// <summary>
		/// Gets whether the completion window should automatically close when the text editor looses focus.
		/// </summary>
		protected virtual bool CloseOnFocusLost => true;

	    bool IsTextAreaFocused {
			get {
				if (parentWindow != null && !parentWindow.IsActive)
					return false;
				return TextArea.IsKeyboardFocused;
			}
		}
		
		bool sourceIsInitialized;
		
		/// <inheritdoc/>
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			
			if (document != null && StartOffset != TextArea.Caret.Offset) {
				SetPosition(new TextViewPosition(document.GetLocation(StartOffset)));
			} else {
				SetPosition(TextArea.Caret.Position);
			}
			sourceIsInitialized = true;
		}
		
		/// <inheritdoc/>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			DetachEvents();
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && e.Key == Key.Escape) {
				e.Handled = true;
				Close();
			}
		}
		
		Point visualLocation, visualLocationTop;
		
		/// <summary>
		/// Positions the completion window at the specified position.
		/// </summary>
		protected void SetPosition(TextViewPosition position)
		{
			TextView textView = TextArea.TextView;
			
			visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
			visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
			UpdatePosition();
		}
		
		/// <summary>
		/// Updates the position of the CompletionWindow based on the parent TextView position and the screen working area.
		/// It ensures that the CompletionWindow is completely visible on the screen.
		/// </summary>
		protected void UpdatePosition()
		{
			TextView textView = TextArea.TextView;
			// PointToScreen returns device dependent units (physical pixels)
			Point location = textView.PointToScreen(visualLocation - textView.ScrollOffset);
			Point locationTop = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);
			
			// Let's use device dependent units for everything
			Size completionWindowSize = new Size(ActualWidth, ActualHeight).TransformToDevice(textView);
			Rect bounds = new Rect(location, completionWindowSize);
			Rect workingScreen = System.Windows.Forms.Screen.GetWorkingArea(location.ToSystemDrawing()).ToWpf();
			if (!workingScreen.Contains(bounds)) {
				if (bounds.Left < workingScreen.Left) {
					bounds.X = workingScreen.Left;
				} else if (bounds.Right > workingScreen.Right) {
					bounds.X = workingScreen.Right - bounds.Width;
				}
				if (bounds.Bottom > workingScreen.Bottom) {
					bounds.Y = locationTop.Y - bounds.Height;
					IsUp = true;
				} else {
					IsUp = false;
				}
				if (bounds.Y < workingScreen.Top) {
					bounds.Y = workingScreen.Top;
				}
			}
			// Convert the window bounds to device independent units
			bounds = bounds.TransformFromDevice(textView);
			Left = bounds.X;
			Top = bounds.Y;
		}
		
		/// <inheritdoc/>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			if (sizeInfo.HeightChanged && IsUp) {
				Top += sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height;
			}
		}
		
		/// <summary>
		/// Gets/sets whether the completion window should expect text insertion at the start offset,
		/// which not go into the completion region, but before it.
		/// </summary>
		/// <remarks>This property allows only a single insertion, it is reset to false
		/// when that insertion has occurred.</remarks>
		public bool ExpectInsertionBeforeStart { get; set; }
		
		void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
		{
			if (e.Offset + e.RemovalLength == StartOffset && e.RemovalLength > 0) {
				Close(); // removal immediately in front of completion segment: close the window
				// this is necessary when pressing backspace after dot-completion
			}
			if (e.Offset == StartOffset && e.RemovalLength == 0 && ExpectInsertionBeforeStart) {
				StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.AfterInsertion);
				ExpectInsertionBeforeStart = false;
			} else {
				StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.BeforeInsertion);
			}
			EndOffset = e.GetNewOffset(EndOffset, AnchorMovementType.AfterInsertion);
		}
	}
}
