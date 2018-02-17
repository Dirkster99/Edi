// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public class CompletionWindow : CompletionWindowBase
	{
	    ToolTip toolTip = new ToolTip();
		
		/// <summary>
		/// Gets the completion list used in this completion window.
		/// </summary>
		public CompletionList CompletionList { get; } = new CompletionList();

	    /// <summary>
		/// Creates a new code completion window.
		/// </summary>
		public CompletionWindow(TextArea textArea) : base(textArea)
		{
			// keep height automatic
			CloseAutomatically = true;
			SizeToContent = SizeToContent.Height;
			MaxHeight = 300;
			Width = 175;
			Content = CompletionList;
			// prevent user from resizing window to 0x0
			MinHeight = 15;
			MinWidth = 30;
            Background = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00)); //Dirkster99

            toolTip.PlacementTarget = this;
			toolTip.Placement = PlacementMode.Right;
			toolTip.Closed += toolTip_Closed;
			
			AttachEvents();
		}
		
		#region ToolTip handling
		void toolTip_Closed(object sender, RoutedEventArgs e)
		{
			// Clear content after tooltip is closed.
			// We cannot clear is immediately when setting IsOpen=false
			// because the tooltip uses an animation for closing.
			if (toolTip != null)
				toolTip.Content = null;
		}
		
		void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = CompletionList.SelectedItem;
			if (item == null)
				return;
			object description = item.Description;
			if (description != null) {
                if (description is string)
                {
                    string descriptionText = description as string;
                    toolTip.Content = new TextBlock
                    {
                        Text = descriptionText,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    toolTip.Content = description;
                }
                toolTip.IsOpen = true;
			} else {
				toolTip.IsOpen = false;
			}
		}
		#endregion
		
		void completionList_InsertionRequested(object sender, EventArgs e)
		{
			Close();
			// The window must close before Complete() is called.
			// If the Complete callback pushes stacked input handlers, we don't want to pop those when the CC window closes.
			var item = CompletionList.SelectedItem;
		    item?.Complete(TextArea, new AnchorSegment(TextArea.Document, StartOffset, EndOffset - StartOffset), e);
		}
		
		void AttachEvents()
		{
			CompletionList.InsertionRequested += completionList_InsertionRequested;
			CompletionList.SelectionChanged += completionList_SelectionChanged;
			TextArea.Caret.PositionChanged += CaretPositionChanged;
			TextArea.MouseWheel += textArea_MouseWheel;
			TextArea.PreviewTextInput += textArea_PreviewTextInput;
		}
		
		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			CompletionList.InsertionRequested -= completionList_InsertionRequested;
			CompletionList.SelectionChanged -= completionList_SelectionChanged;
			TextArea.Caret.PositionChanged -= CaretPositionChanged;
			TextArea.MouseWheel -= textArea_MouseWheel;
			TextArea.PreviewTextInput -= textArea_PreviewTextInput;
			base.DetachEvents();
		}
		
		/// <inheritdoc/>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (toolTip != null) {
				toolTip.IsOpen = false;
				toolTip = null;
			}
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled) {
				CompletionList.HandleKey(e);
			}
		}
		
		void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
			                           new TextCompositionEventArgs(e.Device, e.TextComposition));
		}
		
		void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = RaiseEventPair(GetScrollEventTarget(),
			                           PreviewMouseWheelEvent, MouseWheelEvent,
			                           new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
		}
		
		UIElement GetScrollEventTarget()
		{
			if (CompletionList == null)
				return this;
			return CompletionList.ScrollViewer ?? CompletionList.ListBox ?? (UIElement)CompletionList;
		}
		
		/// <summary>
		/// Gets/Sets whether the completion window should close automatically.
		/// The default value is true.
		/// </summary>
		public bool CloseAutomatically { get; set; }
		
		/// <inheritdoc/>
		protected override bool CloseOnFocusLost => CloseAutomatically;

	    /// <summary>
		/// When this flag is set, code completion closes if the caret moves to the
		/// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
		/// but not in dot-completion.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		public bool CloseWhenCaretAtBeginning { get; set; }
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			int offset = TextArea.Caret.Offset;
			if (offset == StartOffset) {
				if (CloseAutomatically && CloseWhenCaretAtBeginning) {
					Close();
				} else {
					CompletionList.SelectItem(string.Empty);
				}
				return;
			}
			if (offset < StartOffset || offset > EndOffset) {
				if (CloseAutomatically) {
					Close();
				}
			} else {
				TextDocument document = TextArea.Document;
				if (document != null) {
					CompletionList.SelectItem(document.GetText(StartOffset, offset - StartOffset));
				}
			}
		}
	}
}
