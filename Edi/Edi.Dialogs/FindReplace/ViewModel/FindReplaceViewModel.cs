namespace Edi.Dialogs.FindReplace.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Core.ViewModels.Base;
    using Core.ViewModels.Command;

    public class FindReplaceViewModel : DialogViewModelBase
    {
        #region fields
        /// <summary>
        /// Maximum of find/replace history list
        /// </summary>
        private const int MaxCountFindReplaceHistory = 10;

        private RelayCommand<object> mFindCommand;
        private RelayCommand<object> mReplaceCommand;
        private RelayCommand<object> mReplaceAllCommand;

        private string mTextToFind = string.Empty;
        private string mReplacementText = string.Empty;
        private bool mSearchUp = false;
        private bool mUseWildcards = false;
        private bool mCaseSensitive = false;
        private bool mUseRegEx = false;
        private bool mWholeWord = false;
        private bool mAcceptsReturn = false;
        private bool mAllowReplace = true;

        private bool mShowAsFind = true;
        private bool mIsTextToFindFocused = true;
        private bool mIsTextToFindInReplaceFocused = true;
        private SearchScope mSearchIn = SearchScope.CurrentDocument;
        private bool mShowSearchIn = true;
        private List<string> mListFindHistory;
        private List<string> mListReplaceHistory;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor from <seealso cref="ISettingsManager"/> manager instance to reade & write
        /// strings that were searched and replaced during user session.
        /// </summary>
        /// <param name="settingsManager"></param>
        public FindReplaceViewModel(Settings.Interfaces.ISettingsManager settingsManager)
            : this()
        {
            // load the find/replace history from user profile
            mListFindHistory = settingsManager.SessionData.FindHistoryList;
            mListReplaceHistory = settingsManager.SessionData.ReplaceHistoryList;
        }

        /// <summary>
        /// Hidden standard class constructor.
        /// </summary>
        /// <param name="settingsManager"></param>
        protected FindReplaceViewModel()
            : base()
        {
            CurrentEditor = null;

            // load the find/replace history from user profile
            mListFindHistory = null;
            mListReplaceHistory = null;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get the title string of the view - to be displayed in the associated view
        /// (e.g. as dialog title)
        /// </summary>
        public string WindowTitle
        {
            get
            {
                return Util.Local.Strings.STR_FIND_REPLACE_CAPTION;
            }
        }

        /// <summary>
        /// Get/set text to find via find/replace
        /// </summary>
        public string TextToFind
        {
            get
            {
                return mTextToFind;
            }

            set
            {
                if (mTextToFind != value)
                {
                    mTextToFind = value;

                    RaisePropertyChanged(() => TextToFind);
                }
            }
        }

        /// <summary>
        /// Get/set text to replace via find/replace
        /// </summary>
        public string ReplacementText
        {
            get
            {
                return mReplacementText;
            }

            set
            {
                if (mReplacementText != value)
                {
                    mReplacementText = value;

                    RaisePropertyChanged(() => ReplacementText);
                }
            }
        }

        /// <summary>
        /// Get/set whether the search is upward in text or downwards.
        /// </summary>
        public bool SearchUp
        {
            get
            {
                return mSearchUp;
            }

            set
            {
                if (mSearchUp != value)
                {
                    mSearchUp = value;

                    RaisePropertyChanged(() => SearchUp);
                }
            }
        }

        /// <summary>
        /// Get/set whether to do a wildcard based search or not.
        /// </summary>
        public bool UseWildcards
        {
            get
            {
                return mUseWildcards;
            }

            set
            {
                if (mUseWildcards != value)
                {
                    mUseWildcards = value;

                    RaisePropertyChanged(() => UseWildcards);
                }
            }
        }

        /// <summary>
        /// Get/set whether to search case sensitive or not.
        /// </summary>
        public bool CaseSensitive
        {
            get
            {
                return mCaseSensitive;
            }

            set
            {
                if (mCaseSensitive != value)
                {
                    mCaseSensitive = value;

                    RaisePropertyChanged(() => CaseSensitive);
                }
            }
        }

        /// <summary>
        /// Get/set whether to search text with regular expressions or not.
        /// </summary>
        public bool UseRegEx
        {
            get
            {
                return mUseRegEx;
            }

            set
            {
                if (mUseRegEx != value)
                {
                    mUseRegEx = value;

                    RaisePropertyChanged(() => UseRegEx);
                }
            }
        }

        /// <summary>
        /// Get/set whether to search for a whole word occurrance or not.
        /// </summary>
        public bool WholeWord
        {
            get
            {
                return mWholeWord;
            }

            set
            {
                if (mWholeWord != value)
                {
                    mWholeWord = value;

                    RaisePropertyChanged(() => WholeWord);
                }
            }
        }

        /// <summary>
        /// Get/set whether Find and Replace textbox in view
        /// support multiline input or not.
        /// </summary>
        public bool AcceptsReturn
        {
            get
            {
                return mAcceptsReturn;
            }

            set
            {
                if (mAcceptsReturn != value)
                {
                    mAcceptsReturn = value;

                    RaisePropertyChanged(() => AcceptsReturn);
                }
            }
        }

        /// <summary>
        /// Get/set whether replace is to be displayed and used in view or not.
        /// </summary>
        public bool AllowReplace
        {
            get
            {
                return mAllowReplace;
            }

            set
            {
                if (mAllowReplace != value)
                {
                    mAllowReplace = value;

                    RaisePropertyChanged(() => AllowReplace);
                }
            }
        }

        /// <summary>
        /// Get/set property to determine whether dialog should show Find UI (true)
        /// or whether it should show Find/Replace UI elements (false).
        /// </summary>
        public bool ShowAsFind
        {
            get
            {
                return mShowAsFind;
            }

            set
            {
                if (mShowAsFind != value)
                {
                    mShowAsFind = value;

                    if (value)            // Focus textbox on find tab or find/replace tab
                    {
                        IsTextToFindFocused = true;
                        IsTextToFindInReplaceFocused = false;
                    }
                    else
                    {
                        IsTextToFindFocused = false;
                        IsTextToFindInReplaceFocused = true;
                    }

                    RaisePropertyChanged(() => ShowAsFind);
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool IsTextToFindFocused
        {
            get
            {
                return mIsTextToFindFocused;
            }

            set
            {
                if (mIsTextToFindFocused != value)
                {
                    mIsTextToFindFocused = value;

                    RaisePropertyChanged(() => IsTextToFindFocused);
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool IsTextToFindInReplaceFocused
        {
            get
            {
                return mIsTextToFindInReplaceFocused;
            }

            set
            {
                if (mIsTextToFindInReplaceFocused != value)
                {
                    mIsTextToFindInReplaceFocused = value;

                    RaisePropertyChanged(() => IsTextToFindInReplaceFocused);
                }
            }
        }

        /// <summary>
        /// Get/set whether to search in current document, all open documents etc.
        /// </summary>
        public SearchScope SearchIn
        {
            get
            {
                return mSearchIn;
            }

            set
            {
                if (mSearchIn != value)
                {
                    mSearchIn = value;

                    RaisePropertyChanged(() => SearchIn);
                }
            }
        }

        /// <summary>
        /// Determines whether to display the Search in choice or not.
        /// 
        /// The user can use this choice to select whether results are search/replaced:
        /// 1> Only in the current document
        /// 2> In all open documents
        /// </summary>
        public bool ShowSearchIn
        {
            get
            {
                return mShowSearchIn;
            }

            private set
            {
                if (mShowSearchIn != value)
                {
                    mShowSearchIn = value;

                    RaisePropertyChanged(() => ShowSearchIn);
                }
            }
        }

        public IEditor CurrentEditor { get; set; }

        public Func<FindReplaceViewModel, bool, bool> FindNext { get; set; }

        public ICommand FindCommand
        {
            get
            {
                if (mFindCommand == null)
                    mFindCommand = new RelayCommand<object>((p) =>
                    {
                        if (FindNext != null)
                        {
                            // remember the searched text into search history
                            if (TextToFind.Length > 0)
                            {
                                FindHistory.Remove(TextToFind);
                                FindHistory.Insert(0, TextToFind);
                                if (FindHistory.Count > MaxCountFindReplaceHistory)
                                {
                                    FindHistory.RemoveRange(MaxCountFindReplaceHistory, FindHistory.Count - MaxCountFindReplaceHistory);
                                }
                            }

                            FindNext(this, mSearchUp);
                        }
                    });

                return mFindCommand;
            }
        }

        public ICommand ReplaceCommand
        {
            get
            {
                if (mReplaceCommand == null)
                    mReplaceCommand = new RelayCommand<object>((p) =>
                    {
                        addReplaceHistory();
                        Replace();
                    });

                return mReplaceCommand;
            }
        }

        public ICommand ReplaceAllCommand
        {
            get
            {
                if (mReplaceAllCommand == null)
                    mReplaceAllCommand = new RelayCommand<object>((p) =>
                    {
                        addReplaceHistory();
                        ReplaceAll();
                    });

                return mReplaceAllCommand;
            }
        }

        public List<string> FindHistory
        {
            get
            {
                return mListFindHistory;
            }
        }

        public List<string> ReplaceHistory
        {
            get
            {
                return mListReplaceHistory;
            }
        }

        /// <summary>
        /// Helper function to add replace history
        /// </summary>
        private void addReplaceHistory()
        {
            // remember the replacement text into replace history
            if (ReplacementText.Length > 0)
            {
                ReplaceHistory.Remove(ReplacementText);
                ReplaceHistory.Insert(0, ReplacementText);
                if (ReplaceHistory.Count > MaxCountFindReplaceHistory)
                {
                    ReplaceHistory.RemoveRange(MaxCountFindReplaceHistory, ReplaceHistory.Count - MaxCountFindReplaceHistory);
                }
            }
        }

        #endregion properties

        #region methods
        /// <summary>
        /// Constructs a regular expression according to the currently selected search parameters.
        /// </summary>
        /// <param name="ForceLeftToRight"></param>
        /// <returns>The regular expression.</returns>
        public Regex GetRegEx(bool ForceLeftToRight = false)
        {
            // Multiline option is required to support matching start and end of line in regex
            // http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regexoptions.aspx
            // http://stackoverflow.com/questions/6596060/how-to-match-begin-or-end-of-line-using-c-regex
            Regex r;
            RegexOptions o = RegexOptions.Multiline;

            if (SearchUp && !ForceLeftToRight)
                o = o | RegexOptions.RightToLeft;

            if (!CaseSensitive)
                o = o | RegexOptions.IgnoreCase;

            if (UseRegEx)
                r = new Regex(TextToFind, o);
            else
            {
                string s = Regex.Escape(TextToFind);

                if (UseWildcards)
                    s = s.Replace("\\*", ".*").Replace("\\?", ".");

                if (WholeWord)
                    s = "\\W" + s + "\\W";

                r = new Regex(s, o);
            }

            return r;
        }

        public void FindPrevious()
        {
            if (FindNext != null)
                FindNext(this, true);
        }

        public void Replace()
        {
            IEditor CE = GetCurrentEditor();
            if (CE == null) return;

            // if currently selected text matches -> replace; anyways, find the next match
            Regex r = GetRegEx();
            string s = CE.Text.Substring(CE.SelectionStart, CE.SelectionLength); // CE.SelectedText;
            Match m = r.Match(s);
            if (m.Success && m.Index == 0 && m.Length == s.Length)
            {
                CE.Replace(CE.SelectionStart, CE.SelectionLength, ReplacementText);
                //CE.SelectedText = ReplacementText;
            }

            if (FindNext != null)
                FindNext(this, false);
        }

        public void ReplaceAll(bool AskBefore = true)
        {
            IEditor CE = GetCurrentEditor();
            if (CE == null) return;

            var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();

            if (!AskBefore || msgBox.Show(string.Format(Util.Local.Strings.STR_FINDREPLACE_ASK_REALLY_REPLACEEVERYTHING, TextToFind, ReplacementText),
                                          Util.Local.Strings.STR_FINDREPLACE_ReplaceAll_Caption,
                                          MsgBoxButtons.YesNoCancel, MsgBoxImage.Alert) == MsgBoxResult.Yes)
            {
                object InitialEditor = CurrentEditor;
                // loop through all editors, until we are back at the starting editor                
                do
                {
                    Regex r = GetRegEx(true);   // force left to right, otherwise indices are screwed up
                    int offset = 0;
                    CE.BeginChange();
                    foreach (Match m in r.Matches(CE.Text))
                    {
                        CE.Replace(offset + m.Index, m.Length, ReplacementText);
                        offset += ReplacementText.Length - m.Length;
                    }
                    CE.EndChange();

                    // XXX TODO CE = GetNextEditor();
                } while (CurrentEditor != InitialEditor);
            }
        }

        public IEditor GetCurrentEditor()
        {
            if (CurrentEditor == null)
                return null;

            if (CurrentEditor is IEditor)
                return CurrentEditor as IEditor;

            ////      if (InterfaceConverter == null)
            ////        return null;
            ////
            ////      return InterfaceConverter.Convert(CurrentEditor, typeof(IEditor), null, CultureInfo.CurrentCulture) as IEditor;

            return null;
        }
        #endregion methods
    }
}
