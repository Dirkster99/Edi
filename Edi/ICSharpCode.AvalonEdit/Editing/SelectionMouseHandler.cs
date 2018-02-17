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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Handles selection of text using the mouse.
	/// </summary>
	sealed class SelectionMouseHandler : ITextAreaInputHandler
	{
		#region enum SelectionMode
		enum SelectionMode
		{
			/// <summary>
			/// no selection (no mouse button down)
			/// </summary>
			None,
			/// <summary>
			/// left mouse button down on selection, might be normal click
			/// or might be drag'n'drop
			/// </summary>
			PossibleDragStart,
			/// <summary>
			/// dragging text
			/// </summary>
			Drag,
			/// <summary>
			/// normal selection (click+drag)
			/// </summary>
			Normal,
			/// <summary>
			/// whole-word selection (double click+drag or ctrl+click+drag)
			/// </summary>
			WholeWord,
			/// <summary>
			/// whole-line selection (triple click+drag)
			/// </summary>
			WholeLine,
			/// <summary>
			/// rectangular selection (alt+click+drag)
			/// </summary>
			Rectangular
		}
		#endregion

	    SelectionMode mode;
		AnchorSegment startWord;
		Point possibleDragStartMousePos;
		
		#region Constructor + Attach + Detach
		public SelectionMouseHandler(TextArea textArea)
		{
            this.TextArea = textArea ?? throw new ArgumentNullException(nameof(textArea));
		}
		
		public TextArea TextArea { get; }

	    public void Attach()
		{
			TextArea.MouseLeftButtonDown += textArea_MouseLeftButtonDown;
			TextArea.MouseMove += textArea_MouseMove;
			TextArea.MouseLeftButtonUp += textArea_MouseLeftButtonUp;
			TextArea.QueryCursor += textArea_QueryCursor;
			TextArea.OptionChanged += textArea_OptionChanged;
			
			enableTextDragDrop = TextArea.Options.EnableTextDragDrop;
			if (enableTextDragDrop) {
				AttachDragDrop();
			}
		}
		
		public void Detach()
		{
			mode = SelectionMode.None;
			TextArea.MouseLeftButtonDown -= textArea_MouseLeftButtonDown;
			TextArea.MouseMove -= textArea_MouseMove;
			TextArea.MouseLeftButtonUp -= textArea_MouseLeftButtonUp;
			TextArea.QueryCursor -= textArea_QueryCursor;
			TextArea.OptionChanged -= textArea_OptionChanged;
			if (enableTextDragDrop) {
				DetachDragDrop();
			}
		}
		
		void AttachDragDrop()
		{
			TextArea.AllowDrop = true;
			TextArea.GiveFeedback += textArea_GiveFeedback;
			TextArea.QueryContinueDrag += textArea_QueryContinueDrag;
			TextArea.DragEnter += textArea_DragEnter;
			TextArea.DragOver += textArea_DragOver;
			TextArea.DragLeave += textArea_DragLeave;
			TextArea.Drop += textArea_Drop;
		}
		
		void DetachDragDrop()
		{
			TextArea.AllowDrop = false;
			TextArea.GiveFeedback -= textArea_GiveFeedback;
			TextArea.QueryContinueDrag -= textArea_QueryContinueDrag;
			TextArea.DragEnter -= textArea_DragEnter;
			TextArea.DragOver -= textArea_DragOver;
			TextArea.DragLeave -= textArea_DragLeave;
			TextArea.Drop -= textArea_Drop;
		}
		
		bool enableTextDragDrop;
		
		void textArea_OptionChanged(object sender, PropertyChangedEventArgs e)
		{
            // Dirkster99 BugFix for binding options in VS2010
            if (TextArea.Options == null)
                return;

            bool newEnableTextDragDrop = TextArea.Options.EnableTextDragDrop;
			if (newEnableTextDragDrop != enableTextDragDrop) {
				enableTextDragDrop = newEnableTextDragDrop;
				if (newEnableTextDragDrop)
					AttachDragDrop();
				else
					DetachDragDrop();
			}
		}
		#endregion
		
		#region Dropping text
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void textArea_DragEnter(object sender, DragEventArgs e)
		{
			try {
				e.Effects = GetEffect(e);
				TextArea.Caret.Show();
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void textArea_DragOver(object sender, DragEventArgs e)
		{
			try {
				e.Effects = GetEffect(e);
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		
		DragDropEffects GetEffect(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.UnicodeText, true)) {
				e.Handled = true;
				int visualColumn;
				bool isAtEndOfLine;
				int offset = GetOffsetFromMousePosition(e.GetPosition(TextArea.TextView), out visualColumn, out isAtEndOfLine);
				if (offset >= 0) {
					TextArea.Caret.Position = new TextViewPosition(TextArea.Document.GetLocation(offset), visualColumn) { IsAtEndOfLine = isAtEndOfLine };
					TextArea.Caret.DesiredXPos = double.NaN;
					if (TextArea.ReadOnlySectionProvider.CanInsert(offset)) {
						if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move
						    && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey)
						{
							return DragDropEffects.Move;
						} else {
							return e.AllowedEffects & DragDropEffects.Copy;
						}
					}
				}
			}
			return DragDropEffects.None;
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void textArea_DragLeave(object sender, DragEventArgs e)
		{
			try {
				e.Handled = true;
				if (!TextArea.IsKeyboardFocusWithin)
					TextArea.Caret.Hide();
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void textArea_Drop(object sender, DragEventArgs e)
		{
			try {
				DragDropEffects effect = GetEffect(e);
				e.Effects = effect;
				if (effect != DragDropEffects.None) {
					int start = TextArea.Caret.Offset;
					if (mode == SelectionMode.Drag && TextArea.Selection.Contains(start)) {
						Debug.WriteLine("Drop: did not drop: drop target is inside selection");
						e.Effects = DragDropEffects.None;
					} else {
						Debug.WriteLine("Drop: insert at " + start);
						
						var pastingEventArgs = new DataObjectPastingEventArgs(e.Data, true, DataFormats.UnicodeText);
						TextArea.RaiseEvent(pastingEventArgs);
						if (pastingEventArgs.CommandCancelled)
							return;
						
						string text = EditingCommandHandler.GetTextToPaste(pastingEventArgs, TextArea);
						if (text == null)
							return;
						bool rectangular = pastingEventArgs.DataObject.GetDataPresent(RectangleSelection.RectangularSelectionDataType);
						
						// Mark the undo group with the currentDragDescriptor, if the drag
						// is originating from the same control. This allows combining
						// the undo groups when text is moved.
						TextArea.Document.UndoStack.StartUndoGroup(currentDragDescriptor);
						try {
							if (rectangular && RectangleSelection.PerformRectangularPaste(TextArea, TextArea.Caret.Position, text, true)) {
								
							} else {
								TextArea.Document.Insert(start, text);
								TextArea.Selection = Selection.Create(TextArea, start, start + text.Length);
							}
						} finally {
							TextArea.Document.UndoStack.EndUndoGroup();
						}
					}
					e.Handled = true;
				}
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		
		void OnDragException(Exception ex)
		{
			// WPF swallows exceptions during drag'n'drop or reports them incorrectly, so
			// we re-throw them later to allow the application's unhandled exception handler
			// to catch them
			TextArea.Dispatcher.BeginInvoke(
				DispatcherPriority.Send,
				new Action(delegate {
				           	throw new DragDropException("Exception during drag'n'drop", ex);
				           }));
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void textArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			try {
				e.UseDefaultCursors = true;
				e.Handled = true;
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void textArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			try {
				if (e.EscapePressed) {
					e.Action = DragAction.Cancel;
				} else if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.LeftMouseButton) {
					e.Action = DragAction.Drop;
				} else {
					e.Action = DragAction.Continue;
				}
				e.Handled = true;
			} catch (Exception ex) {
				OnDragException(ex);
			}
		}
		#endregion
		
		#region Start Drag
		object currentDragDescriptor;
		
		void StartDrag()
		{
			// prevent nested StartDrag calls
			mode = SelectionMode.Drag;
			
			// mouse capture and Drag'n'Drop doesn't mix
			TextArea.ReleaseMouseCapture();
			
			DataObject dataObject = TextArea.Selection.CreateDataObject(TextArea);
			
			DragDropEffects allowedEffects = DragDropEffects.All;
			var deleteOnMove = TextArea.Selection.Segments.Select(s => new AnchorSegment(TextArea.Document, s)).ToList();
			foreach (ISegment s in deleteOnMove) {
				ISegment[] result = TextArea.GetDeletableSegments(s);
				if (result.Length != 1 || result[0].Offset != s.Offset || result[0].EndOffset != s.EndOffset) {
					allowedEffects &= ~DragDropEffects.Move;
				}
			}
			
			var copyingEventArgs = new DataObjectCopyingEventArgs(dataObject, true);
			TextArea.RaiseEvent(copyingEventArgs);
			if (copyingEventArgs.CommandCancelled)
				return;
			
			object dragDescriptor = new object();
			currentDragDescriptor = dragDescriptor;
			
			DragDropEffects resultEffect;
			using (TextArea.AllowCaretOutsideSelection()) {
				var oldCaretPosition = TextArea.Caret.Position;
				try {
					Debug.WriteLine("DoDragDrop with allowedEffects=" + allowedEffects);
					resultEffect = DragDrop.DoDragDrop(TextArea, dataObject, allowedEffects);
					Debug.WriteLine("DoDragDrop done, resultEffect=" + resultEffect);
				} catch (COMException ex) {
					// ignore COM errors - don't crash on badly implemented drop targets
					Debug.WriteLine("DoDragDrop failed: " + ex.ToString());
					return;
				}
				if (resultEffect == DragDropEffects.None) {
					// reset caret if drag was aborted
					TextArea.Caret.Position = oldCaretPosition;
				}
			}
			
			currentDragDescriptor = null;
			
			if (deleteOnMove != null && resultEffect == DragDropEffects.Move && (allowedEffects & DragDropEffects.Move) == DragDropEffects.Move) {
				bool draggedInsideSingleDocument = (dragDescriptor == TextArea.Document.UndoStack.LastGroupDescriptor);
				if (draggedInsideSingleDocument)
					TextArea.Document.UndoStack.StartContinuedUndoGroup(null);
				TextArea.Document.BeginUpdate();
				try {
					foreach (ISegment s in deleteOnMove) {
						TextArea.Document.Remove(s.Offset, s.Length);
					}
				} finally {
					TextArea.Document.EndUpdate();
					if (draggedInsideSingleDocument)
						TextArea.Document.UndoStack.EndUndoGroup();
				}
			}
		}
		#endregion
		
		#region QueryCursor
		// provide the IBeam Cursor for the text area
		void textArea_QueryCursor(object sender, QueryCursorEventArgs e)
		{
			if (!e.Handled) {
				if (mode != SelectionMode.None || !enableTextDragDrop) {
					e.Cursor = Cursors.IBeam;
					e.Handled = true;
				} else if (TextArea.TextView.VisualLinesValid) {
					// Only query the cursor if the visual lines are valid.
					// If they are invalid, the cursor will get re-queried when the visual lines
					// get refreshed.
					Point p = e.GetPosition(TextArea.TextView);
					if (p.X >= 0 && p.Y >= 0 && p.X <= TextArea.TextView.ActualWidth && p.Y <= TextArea.TextView.ActualHeight) {
						int visualColumn;
						bool isAtEndOfLine;
						int offset = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
						if (TextArea.Selection.Contains(offset))
							e.Cursor = Cursors.Arrow;
						else
							e.Cursor = Cursors.IBeam;
						e.Handled = true;
					}
				}
			}
		}
		#endregion
		
		#region LeftButtonDown
		void textArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			mode = SelectionMode.None;
			if (!e.Handled && e.ChangedButton == MouseButton.Left) {
				ModifierKeys modifiers = Keyboard.Modifiers;
				bool shift = (modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
				if (enableTextDragDrop && e.ClickCount == 1 && !shift) {
					int visualColumn;
					bool isAtEndOfLine;
					int offset = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
					if (TextArea.Selection.Contains(offset)) {
						if (TextArea.CaptureMouse()) {
							mode = SelectionMode.PossibleDragStart;
							possibleDragStartMousePos = e.GetPosition(TextArea);
						}
						e.Handled = true;
						return;
					}
				}
				
				var oldPosition = TextArea.Caret.Position;
				SetCaretOffsetToMousePosition(e);
				
				
				if (!shift) {
					TextArea.ClearSelection();
				}
				if (TextArea.CaptureMouse()) {
					if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && TextArea.Options.EnableRectangularSelection) {
						mode = SelectionMode.Rectangular;
						if (shift && TextArea.Selection is RectangleSelection) {
							TextArea.Selection = TextArea.Selection.StartSelectionOrSetEndpoint(oldPosition, TextArea.Caret.Position);
						}
					} else if (e.ClickCount == 1 && ((modifiers & ModifierKeys.Control) == 0)) {
						mode = SelectionMode.Normal;
						if (shift && !(TextArea.Selection is RectangleSelection)) {
							TextArea.Selection = TextArea.Selection.StartSelectionOrSetEndpoint(oldPosition, TextArea.Caret.Position);
						}
					} else {
						SimpleSegment startWord;
						if (e.ClickCount == 3) {
							mode = SelectionMode.WholeLine;
							startWord = GetLineAtMousePosition(e);
						} else {
							mode = SelectionMode.WholeWord;
							startWord = GetWordAtMousePosition(e);
						}
						if (startWord == SimpleSegment.Invalid) {
							mode = SelectionMode.None;
							TextArea.ReleaseMouseCapture();
							return;
						}
						if (shift && !TextArea.Selection.IsEmpty) {
							if (startWord.Offset < TextArea.Selection.SurroundingSegment.Offset) {
								TextArea.Selection = TextArea.Selection.SetEndpoint(new TextViewPosition(TextArea.Document.GetLocation(startWord.Offset)));
							} else if (startWord.EndOffset > TextArea.Selection.SurroundingSegment.EndOffset) {
								TextArea.Selection = TextArea.Selection.SetEndpoint(new TextViewPosition(TextArea.Document.GetLocation(startWord.EndOffset)));
							}
							this.startWord = new AnchorSegment(TextArea.Document, TextArea.Selection.SurroundingSegment);
						} else {
							TextArea.Selection = Selection.Create(TextArea, startWord.Offset, startWord.EndOffset);
							this.startWord = new AnchorSegment(TextArea.Document, startWord.Offset, startWord.Length);
						}
					}
				}
			}
			e.Handled = true;
		}
		#endregion
		
		#region Mouse Position <-> Text coordinates
		SimpleSegment GetWordAtMousePosition(MouseEventArgs e)
		{
			TextView textView = TextArea.TextView;
			if (textView == null) return SimpleSegment.Invalid;
			Point pos = e.GetPosition(textView);
			if (pos.Y < 0)
				pos.Y = 0;
			if (pos.Y > textView.ActualHeight)
				pos.Y = textView.ActualHeight;
			pos += textView.ScrollOffset;
			VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
			if (line != null) {
				int visualColumn = line.GetVisualColumn(pos, TextArea.Selection.EnableVirtualSpace);
				int wordStartVC = line.GetNextCaretPosition(visualColumn + 1, LogicalDirection.Backward, CaretPositioningMode.WordStartOrSymbol, TextArea.Selection.EnableVirtualSpace);
				if (wordStartVC == -1)
					wordStartVC = 0;
				int wordEndVC = line.GetNextCaretPosition(wordStartVC, LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol, TextArea.Selection.EnableVirtualSpace);
				if (wordEndVC == -1)
					wordEndVC = line.VisualLength;
				int relOffset = line.FirstDocumentLine.Offset;
				int wordStartOffset = line.GetRelativeOffset(wordStartVC) + relOffset;
				int wordEndOffset = line.GetRelativeOffset(wordEndVC) + relOffset;
				return new SimpleSegment(wordStartOffset, wordEndOffset - wordStartOffset);
			} else {
				return SimpleSegment.Invalid;
			}
		}
		
		SimpleSegment GetLineAtMousePosition(MouseEventArgs e)
		{
			TextView textView = TextArea.TextView;
			if (textView == null) return SimpleSegment.Invalid;
			Point pos = e.GetPosition(textView);
			if (pos.Y < 0)
				pos.Y = 0;
			if (pos.Y > textView.ActualHeight)
				pos.Y = textView.ActualHeight;
			pos += textView.ScrollOffset;
			VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
			if (line != null) {
				return new SimpleSegment(line.StartOffset, line.LastDocumentLine.EndOffset - line.StartOffset);
			} else {
				return SimpleSegment.Invalid;
			}
		}
		
		int GetOffsetFromMousePosition(MouseEventArgs e, out int visualColumn, out bool isAtEndOfLine)
		{
			return GetOffsetFromMousePosition(e.GetPosition(TextArea.TextView), out visualColumn, out isAtEndOfLine);
		}
		
		int GetOffsetFromMousePosition(Point positionRelativeToTextView, out int visualColumn, out bool isAtEndOfLine)
		{
			visualColumn = 0;
			TextView textView = TextArea.TextView;
			Point pos = positionRelativeToTextView;
			if (pos.Y < 0)
				pos.Y = 0;
			if (pos.Y > textView.ActualHeight)
				pos.Y = textView.ActualHeight;
			pos += textView.ScrollOffset;
			if (pos.Y > textView.DocumentHeight)
				pos.Y = textView.DocumentHeight - ExtensionMethods.Epsilon;
			VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
			if (line != null) {
				visualColumn = line.GetVisualColumn(pos, TextArea.Selection.EnableVirtualSpace, out isAtEndOfLine);
				return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
			}
			isAtEndOfLine = false;
			return -1;
		}
		
		int GetOffsetFromMousePositionFirstTextLineOnly(Point positionRelativeToTextView, out int visualColumn)
		{
			visualColumn = 0;
			TextView textView = TextArea.TextView;
			Point pos = positionRelativeToTextView;
			if (pos.Y < 0)
				pos.Y = 0;
			if (pos.Y > textView.ActualHeight)
				pos.Y = textView.ActualHeight;
			pos += textView.ScrollOffset;
			if (pos.Y > textView.DocumentHeight)
				pos.Y = textView.DocumentHeight - ExtensionMethods.Epsilon;
			VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
			if (line != null) {
				visualColumn = line.GetVisualColumn(line.TextLines.First(), pos.X, TextArea.Selection.EnableVirtualSpace);
				return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
			}
			return -1;
		}
		#endregion
		
		#region MouseMove
		void textArea_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Handled)
				return;
			if (mode == SelectionMode.Normal || mode == SelectionMode.WholeWord || mode == SelectionMode.WholeLine || mode == SelectionMode.Rectangular) {
				e.Handled = true;
				if (TextArea.TextView.VisualLinesValid) {
					// If the visual lines are not valid, don't extend the selection.
					// Extending the selection forces a VisualLine refresh, and it is sufficient
					// to do that on MouseUp, we don't have to do it every MouseMove.
					ExtendSelectionToMouse(e);
				}
			} else if (mode == SelectionMode.PossibleDragStart) {
				e.Handled = true;
				Vector mouseMovement = e.GetPosition(TextArea) - possibleDragStartMousePos;
				if (Math.Abs(mouseMovement.X) > SystemParameters.MinimumHorizontalDragDistance
				    || Math.Abs(mouseMovement.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartDrag();
				}
			}
		}
		#endregion
		
		#region ExtendSelection
		void SetCaretOffsetToMousePosition(MouseEventArgs e)
		{
			SetCaretOffsetToMousePosition(e, null);
		}
		
		void SetCaretOffsetToMousePosition(MouseEventArgs e, ISegment allowedSegment)
		{
			int visualColumn;
			bool isAtEndOfLine;
			int offset;
			if (mode == SelectionMode.Rectangular) {
				offset = GetOffsetFromMousePositionFirstTextLineOnly(e.GetPosition(TextArea.TextView), out visualColumn);
				isAtEndOfLine = true;
			} else {
				offset = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
			}
			if (allowedSegment != null) {
				offset = offset.CoerceValue(allowedSegment.Offset, allowedSegment.EndOffset);
			}
			if (offset >= 0) {
				TextArea.Caret.Position = new TextViewPosition(TextArea.Document.GetLocation(offset), visualColumn) { IsAtEndOfLine = isAtEndOfLine };
				TextArea.Caret.DesiredXPos = double.NaN;
			}
		}
		
		void ExtendSelectionToMouse(MouseEventArgs e)
		{
			TextViewPosition oldPosition = TextArea.Caret.Position;
			if (mode == SelectionMode.Normal || mode == SelectionMode.Rectangular) {
				SetCaretOffsetToMousePosition(e);
				if (mode == SelectionMode.Normal && TextArea.Selection is RectangleSelection)
					TextArea.Selection = new SimpleSelection(TextArea, oldPosition, TextArea.Caret.Position);
				else if (mode == SelectionMode.Rectangular && !(TextArea.Selection is RectangleSelection))
					TextArea.Selection = new RectangleSelection(TextArea, oldPosition, TextArea.Caret.Position);
				else
					TextArea.Selection = TextArea.Selection.StartSelectionOrSetEndpoint(oldPosition, TextArea.Caret.Position);
			} else if (mode == SelectionMode.WholeWord || mode == SelectionMode.WholeLine) {
				var newWord = (mode == SelectionMode.WholeLine) ? GetLineAtMousePosition(e) : GetWordAtMousePosition(e);
				if (newWord != SimpleSegment.Invalid) {
					TextArea.Selection = Selection.Create(TextArea,
					                                      Math.Min(newWord.Offset, startWord.Offset),
					                                      Math.Max(newWord.EndOffset, startWord.EndOffset));
					// moves caret to start or end of selection
					if( newWord.Offset < startWord.Offset) 
						TextArea.Caret.Offset = newWord.Offset;
					else 
						TextArea.Caret.Offset = Math.Max(newWord.EndOffset, startWord.EndOffset);
				}
			}
			TextArea.Caret.BringCaretToView(5.0);
		}
		#endregion
		
		#region MouseLeftButtonUp
		void textArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (mode == SelectionMode.None || e.Handled)
				return;
			e.Handled = true;
			if (mode == SelectionMode.PossibleDragStart) {
				// -> this was not a drag start (mouse didn't move after mousedown)
				SetCaretOffsetToMousePosition(e);
				TextArea.ClearSelection();
			} else if (mode == SelectionMode.Normal || mode == SelectionMode.WholeWord || mode == SelectionMode.WholeLine || mode == SelectionMode.Rectangular) {
				ExtendSelectionToMouse(e);
			}
			mode = SelectionMode.None;
			TextArea.ReleaseMouseCapture();
		}
		#endregion
	}
}
