namespace EdiDialogs.FindReplace.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Windows.Input;
	using Edi.Core.ViewModels.Base;
	using Edi.Core.ViewModels.Command;
	using MsgBox;
	using Settings;

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
		private EdiDialogs.FindReplace.SearchScope mSearchIn = EdiDialogs.FindReplace.SearchScope.CurrentDocument;
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
			this.mListFindHistory = settingsManager.SessionData.FindHistoryList;
			this.mListReplaceHistory = settingsManager.SessionData.ReplaceHistoryList;
		}

		/// <summary>
		/// Hidden standard class constructor.
		/// </summary>
		/// <param name="settingsManager"></param>
		protected FindReplaceViewModel()
			: base()
		{
			this.CurrentEditor = null;

			// load the find/replace history from user profile
			this.mListFindHistory = null;
			this.mListReplaceHistory = null;
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
				return this.mTextToFind;
			}

			set
			{
				if (this.mTextToFind != value)
				{
					this.mTextToFind = value;

					this.RaisePropertyChanged(() => this.TextToFind);
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
				return this.mReplacementText;
			}

			set
			{
				if (this.mReplacementText != value)
				{
					this.mReplacementText = value;

					this.RaisePropertyChanged(() => this.ReplacementText);
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
				return this.mSearchUp;
			}

			set
			{
				if (this.mSearchUp != value)
				{
					this.mSearchUp = value;

					this.RaisePropertyChanged(() => this.SearchUp);
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
				return this.mUseWildcards;
			}

			set
			{
				if (this.mUseWildcards != value)
				{
					this.mUseWildcards = value;

					this.RaisePropertyChanged(() => this.UseWildcards);
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
				return this.mCaseSensitive;
			}

			set
			{
				if (this.mCaseSensitive != value)
				{
					this.mCaseSensitive = value;

					this.RaisePropertyChanged(() => this.CaseSensitive);
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
				return this.mUseRegEx;
			}

			set
			{
				if (this.mUseRegEx != value)
				{
					this.mUseRegEx = value;

					this.RaisePropertyChanged(() => this.UseRegEx);
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
				return this.mWholeWord;
			}

			set
			{
				if (this.mWholeWord != value)
				{
					this.mWholeWord = value;

					this.RaisePropertyChanged(() => this.WholeWord);
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
				return this.mAcceptsReturn;
			}

			set
			{
				if (this.mAcceptsReturn != value)
				{
					this.mAcceptsReturn = value;

					this.RaisePropertyChanged(() => this.AcceptsReturn);
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
				return this.mAllowReplace;
			}

			set
			{
				if (this.mAllowReplace != value)
				{
					this.mAllowReplace = value;

					this.RaisePropertyChanged(() => this.AllowReplace);
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
				return this.mShowAsFind;
			}

			set
			{
				if (this.mShowAsFind != value)
				{
					this.mShowAsFind = value;

					if (value == true)            // Focus textbox on find tab or find/replace tab
					{
						this.IsTextToFindFocused = true;
						this.IsTextToFindInReplaceFocused = false;
					}
					else
					{
						this.IsTextToFindFocused = false;
						this.IsTextToFindInReplaceFocused = true;
					}

					this.RaisePropertyChanged(() => this.ShowAsFind);
				}
			}
		}

		/// <summary>
		/// </summary>
		public bool IsTextToFindFocused
		{
			get
			{
				return this.mIsTextToFindFocused;
			}

			set
			{
				if (this.mIsTextToFindFocused != value)
				{
					this.mIsTextToFindFocused = value;

					this.RaisePropertyChanged(() => this.IsTextToFindFocused);
				}
			}
		}

		/// <summary>
		/// </summary>
		public bool IsTextToFindInReplaceFocused
		{
			get
			{
				return this.mIsTextToFindInReplaceFocused;
			}

			set
			{
				if (this.mIsTextToFindInReplaceFocused != value)
				{
					this.mIsTextToFindInReplaceFocused = value;

					this.RaisePropertyChanged(() => this.IsTextToFindInReplaceFocused);
				}
			}
		}

		/// <summary>
		/// Get/set whether to search in current document, all open documents etc.
		/// </summary>
		public EdiDialogs.FindReplace.SearchScope SearchIn
		{
			get
			{
				return this.mSearchIn;
			}

			set
			{
				if (this.mSearchIn != value)
				{
					this.mSearchIn = value;

					this.RaisePropertyChanged(() => this.SearchIn);
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
				return this.mShowSearchIn;
			}

			private set
			{
				if (this.mShowSearchIn != value)
				{
					this.mShowSearchIn = value;

					this.RaisePropertyChanged(() => this.ShowSearchIn);
				}
			}
		}

		public IEditor CurrentEditor { get; set; }

		public Func<FindReplaceViewModel, bool, bool> FindNext { get; set; }

		public ICommand FindCommand
		{
			get
			{
				if (this.mFindCommand == null)
					this.mFindCommand = new RelayCommand<object>((p) =>
					{
						if (this.FindNext != null)
						{
							// remember the searched text into search history
							if (this.TextToFind.Length > 0)
							{
								this.FindHistory.Remove(this.TextToFind);
								this.FindHistory.Insert(0, this.TextToFind);
								if (this.FindHistory.Count > MaxCountFindReplaceHistory)
								{
									this.FindHistory.RemoveRange(MaxCountFindReplaceHistory, this.FindHistory.Count - MaxCountFindReplaceHistory);
								}
							}

							this.FindNext(this, this.mSearchUp);
						}
					});

				return this.mFindCommand;
			}
		}

		public ICommand ReplaceCommand
		{
			get
			{
				if (this.mReplaceCommand == null)
					this.mReplaceCommand = new RelayCommand<object>((p) =>
					{
						addReplaceHistory();
						this.Replace();
					});

				return this.mReplaceCommand;
			}
		}

		public ICommand ReplaceAllCommand
		{
			get
			{
				if (this.mReplaceAllCommand == null)
					this.mReplaceAllCommand = new RelayCommand<object>((p) =>
					{
						addReplaceHistory();
						this.ReplaceAll();
					});

				return this.mReplaceAllCommand;
			}
		}

		public List<string> FindHistory
		{
			get
			{
				return this.mListFindHistory;
			}
		}

		public List<string> ReplaceHistory
		{
			get
			{
				return this.mListReplaceHistory;
			}
		}

		/// <summary>
		/// Helper function to add replace history
		/// </summary>
		private void addReplaceHistory()
		{
			// remember the replacement text into replace history
			if (this.ReplacementText.Length > 0)
			{
				this.ReplaceHistory.Remove(this.ReplacementText);
				this.ReplaceHistory.Insert(0, this.ReplacementText);
				if (this.ReplaceHistory.Count > MaxCountFindReplaceHistory)
				{
					this.ReplaceHistory.RemoveRange(MaxCountFindReplaceHistory, this.ReplaceHistory.Count - MaxCountFindReplaceHistory);
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
			if (this.FindNext != null)
				this.FindNext(this, true);
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

			if (this.FindNext != null)
				this.FindNext(this, false);
		}

		public void ReplaceAll(bool AskBefore = true)
		{
			IEditor CE = GetCurrentEditor();
			if (CE == null) return;

			if (!AskBefore || MsgBox.Msg.Show(string.Format(Util.Local.Strings.STR_FINDREPLACE_ASK_REALLY_REPLACEEVERYTHING, TextToFind, ReplacementText),
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
