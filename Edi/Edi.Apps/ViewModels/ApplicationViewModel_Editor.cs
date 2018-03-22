namespace Edi.Apps.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Threading;
    using Edi.Dialogs.FindReplace.ViewModel;
    using ICSharpCode.AvalonEdit.Document;
    using MsgBox;
    using Edi.Documents.ViewModels.EdiDoc;

    public partial class ApplicationViewModel
    {
        private FindReplaceViewModel _mFindReplaceVm = null;
        public FindReplaceViewModel FindReplaceVm
        {
            get
            {
                return this._mFindReplaceVm;
            }

            protected set
            {
                if (this._mFindReplaceVm != value)
                {
                    this._mFindReplaceVm = value;
                    this.RaisePropertyChanged(() => this.FindReplaceVm);
                }
            }
        }

        private IEditor GetNextEditor(FindReplaceViewModel f,
                                                                    bool previous = false
                                                                    )
        {
            // There is no next open document if there is none or only one open
            if (this.Files.Count <= 1)
                return f.GetCurrentEditor();

            // There is no next open document If the user wants to search the current document only
            if (f.SearchIn == Edi.Dialogs.FindReplace.SearchScope.CurrentDocument)
                return f.GetCurrentEditor();

            var l = new List<object>(this.Files.Cast<object>());

            int idxStart = l.IndexOf(f.CurrentEditor);
            int i = idxStart;

            if (i >= 0)
            {
                Match m = null;

                bool textSearchSuccess = false;
                do
                {
                    if (previous == true)                  // Get next/previous document
                        i = (i < 1 ? l.Count - 1 : i - 1);
                    else
                        i = (i >= l.Count - 1 ? 0 : i + 1);

                    //// i = (i + (previous ? l.Count - 1 : +1)) % l.Count;

                    // Search text in document
                    if (l[i] is EdiViewModel)
                    {
                        EdiViewModel fTmp = l[i] as EdiViewModel;

                        Regex r;
                        m = this.FindNextMatchInText(0, 0, false, fTmp.Text, ref f, out r);

                        textSearchSuccess = m.Success;
                    }
                }
                while (i != idxStart && textSearchSuccess != true);

                // Found a match so activate the corresponding document and select the text with scroll into view
                if (textSearchSuccess == true && m != null)
                {
                    var doc = l[i] as EdiViewModel;

                    if (doc != null)
                        this.ActiveDocument = doc;

                    // Ensure that no pending calls are in the dispatcher queue
                    // This makes sure that we are blocked until bindings are re-established
                    // Bindings are required to scroll a selection into view
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
                    {
                        if (this.ActiveDocument != null && doc != null)
                        {
                            doc.TextEditorSelectionStart = m.Index;
                            doc.TextEditorSelectionLength = m.Length;

                            // Reset cursor position to make sure we search a document from its beginning
                            doc.TxtControl.SelectText(m.Index, m.Length);

                            f.CurrentEditor = l[i] as IEditor;

                            IEditor edi = f.GetCurrentEditor();

                            if (edi != null)
                                edi.Select(m.Index, m.Length);

                        }
                    });

                    return f.GetCurrentEditor();
                }
            }

            return null;
        }

        /// <summary>
        /// Find a match in a given peace of string
        /// </summary>
        /// <param name="selectionStart"></param>
        /// <param name="selectionLength"></param>
        /// <param name="invertLeftRight"></param>
        /// <param name="text"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        Match FindNextMatchInText(int selectionStart,             // CE.SelectionStart
                                                            int selectionLength,           // CE.SelectionLength
                                                            bool invertLeftRight,         // CE.Text
                                                            string text,                 // InvertLeftRight
                                                            ref FindReplaceViewModel f,
                                                            out Regex r)
        {
            if (invertLeftRight)
            {
                f.SearchUp = !f.SearchUp;
                r = f.GetRegEx();
                f.SearchUp = !f.SearchUp;
            }
            else
                r = f.GetRegEx();

            return r.Match(text, r.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart : selectionStart + selectionLength);
        }

        private bool FindNext(FindReplaceViewModel f,
                                                    bool invertLeftRight = false)
        {
            IEditor ce = f.GetCurrentEditor();

            if (ce == null)
                return false;

            Regex r;
            Match m = this.FindNextMatchInText(ce.SelectionStart, ce.SelectionLength,
                                                                                 invertLeftRight, ce.Text, ref f, out r);

            if (m.Success)
            {
                ce.Select(m.Index, m.Length);

                return true;
            }
            else
            {
                if (f.SearchIn == Edi.Dialogs.FindReplace.SearchScope.CurrentDocument)
                {
                    _msgBox.Show(Util.Local.Strings.STR_MSG_FIND_NO_MORE_ITEMS_FOUND);

                    return false;
                }

                // we have reached the end of the document
                // start again from the beginning/end,
                object oldEditor = f.CurrentEditor;
                do
                {
                    if (f.SearchIn == Edi.Dialogs.FindReplace.SearchScope.AllDocuments)
                    {
                        ce = this.GetNextEditor(f, r.Options.HasFlag(RegexOptions.RightToLeft));

                        if (ce == null)
                            return false;

                        f.CurrentEditor = ce;

                        return true;
                    }

                    if (r.Options.HasFlag(RegexOptions.RightToLeft))
                        m = r.Match(ce.Text, ce.Text.Length - 1);
                    else
                        m = r.Match(ce.Text, 0);

                    if (m.Success)
                    {
                        ce.Select(m.Index, m.Length);
                        break;
                    }
                    else
                    {
                        _msgBox.Show(Util.Local.Strings.STR_MSG_FIND_NO_MORE_ITEMS_FOUND2,
                                     Util.Local.Strings.STR_MSG_FIND_Caption);
                    }
                } while (f.CurrentEditor != oldEditor);
            }

            return false;
        }

        /// <summary>
        /// Gets the current line in which the cursor is currently located
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static int GetCurrentEditorLine(EdiViewModel f)
        {
            int iCurrLine = 0;

            try
            {
                int start, length;
                bool isRectangularSelection = false;

                f.TxtControl.CurrentSelection(out start, out length, out isRectangularSelection);

                iCurrLine = f.Document.GetLineByOffset(start).LineNumber;
            }
            catch (Exception)
            {
            }
            return iCurrLine;
        }

        private void ShowGotoLineDialog()
        {

            if (this.ActiveDocument is EdiViewModel)
            {
                EdiViewModel f = this.ActiveDocument as EdiViewModel;

                Window dlg = null;
                Edi.Dialogs.GotoLine.GotoLineViewModel dlgVm = null;

                try
                {
                    int iCurrLine = ApplicationViewModel.GetCurrentEditorLine(f);

                    dlgVm = new Edi.Dialogs.GotoLine.GotoLineViewModel(1, f.Document.LineCount, iCurrLine);
                    dlg = ViewSelector.GetDialogView((object)dlgVm, Application.Current.MainWindow);

                    dlg.Closing += dlgVm.OnClosing;

                    dlg.ShowDialog();

                    // Copy input if user OK'ed it. This could also be done by a method, equality operator, or copy constructor
                    if (dlgVm.WindowCloseResult == true)
                    {
                        DocumentLine line = f.Document.GetLineByNumber(dlgVm.LineNumber);

                        f.TxtControl.SelectText(line.Offset, 0);      // Select text with length 0 and scroll to where
                        f.TxtControl.ScrollToLine(dlgVm.LineNumber); // we are supposed to be at
                    }
                }
                catch (Exception exc)
                {
                    _msgBox.Show(exc, Edi.Util.Local.Strings.STR_MSG_FIND_UNEXPECTED_ERROR,
                                 MsgBoxButtons.OK, MsgBoxImage.Error);
                }
                finally
                {
                    if (dlg != null)
                    {
                        dlg.Closing -= dlgVm.OnClosing;
                        dlg.Close();
                    }
                }
            }
        }

        private void ShowFindReplaceDialog(bool showFind = true)
        {

            if (this.ActiveDocument is EdiViewModel)
            {
                EdiViewModel f = this.ActiveDocument as EdiViewModel;
                Window dlg = null;

                try
                {
                    if (this.FindReplaceVm == null)
                    {
                        this.FindReplaceVm = new Edi.Dialogs.FindReplace.ViewModel.FindReplaceViewModel(this._mSettingsManager);
                    }

                    this.FindReplaceVm.FindNext = this.FindNext;

                    // determine whether Find or Find/Replace is to be executed
                    this.FindReplaceVm.ShowAsFind = showFind;

                    if (f.TxtControl != null)      // Search by default for currently selected text (if any)
                    {
                        string textToFind;
                        f.TxtControl.GetSelectedText(out textToFind);

                        if (textToFind.Length > 0)
                            this.FindReplaceVm.TextToFind = textToFind;
                    }

                    this.FindReplaceVm.CurrentEditor = f;

                    dlg = ViewSelector.GetDialogView((object)this.FindReplaceVm, Application.Current.MainWindow);

                    dlg.Closing += this.FindReplaceVm.OnClosing;

                    dlg.ShowDialog();
                }
                catch (Exception exc)
                {
                    _msgBox.Show(exc, Edi.Util.Local.Strings.STR_MSG_FIND_UNEXPECTED_ERROR,
                                 MsgBoxButtons.OK, MsgBoxImage.Error);
                }
                finally
                {
                    if (dlg != null)
                    {
                        dlg.Closing -= this.FindReplaceVm.OnClosing;
                        dlg.Close();
                    }
                }
            }
        }
    }
}
