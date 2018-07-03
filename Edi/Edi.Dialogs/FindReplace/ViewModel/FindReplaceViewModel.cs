using Edi.Settings.Interfaces;

namespace Edi.Dialogs.FindReplace.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Windows.Input;
	using Core.ViewModels.Base;
	using Core.ViewModels.Command;
	using MsgBox;
	using CommonServiceLocator;

	public class FindReplaceViewModel : DialogViewModelBase
	{
		#region fields
		/// <summary>
		/// Maximum of find/replace history list
		/// </summary>
		private const int MaxCountFindReplaceHistory = 10;

		private RelayCommand<object> _mFindCommand;
		private RelayCommand<object> _mReplaceCommand;
		private RelayCommand<object> _mReplaceAllCommand;

		private string _mTextToFind = string.Empty;
		private string _mReplacementText = string.Empty;
		private bool _mSearchUp = false;
		private bool _mUseWildcards = false;
		private bool _mCaseSensitive = false;
		private bool _mUseRegEx = false;
		private bool _mWholeWord = false;
		private bool _mAcceptsReturn = false;
		private bool _mAllowReplace = true;

		private bool _mShowAsFind = true;
		private bool _mIsTextToFindFocused = true;
		private bool _mIsTextToFindInReplaceFocused = true;
		private SearchScope _mSearchIn = SearchScope.CurrentDocument;
		private bool _mShowSearchIn = true;

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
			FindHistory = settingsManager.SessionData.FindHistoryList;
			ReplaceHistory = settingsManager.SessionData.ReplaceHistoryList;
		}

		/// <summary>
		/// Hidden standard class constructor.
		/// </summary>
		protected FindReplaceViewModel()
		{
			CurrentEditor = null;

			// load the find/replace history from user profile
			FindHistory = null;
			ReplaceHistory = null;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the title string of the view - to be displayed in the associated view
		/// (e.g. as dialog title)
		/// </summary>
		public string WindowTitle => Util.Local.Strings.STR_FIND_REPLACE_CAPTION;

		/// <summary>
		/// Get/set text to find via find/replace
		/// </summary>
		public string TextToFind
		{
			get { return _mTextToFind; }

			set
			{
				if (_mTextToFind != value)
				{
					_mTextToFind = value;

					RaisePropertyChanged(() => TextToFind);
				}
			}
		}

		/// <summary>
		/// Get/set text to replace via find/replace
		/// </summary>
		public string ReplacementText
        {
            get { return _mReplacementText; }

			set
			{
				if (_mReplacementText != value)
				{
					_mReplacementText = value;

					RaisePropertyChanged(() => ReplacementText);
				}
			}
		}

		/// <summary>
		/// Get/set whether the search is upward in text or downwards.
		/// </summary>
		public bool SearchUp
        {
            get { return _mSearchUp; }

			set
			{
				if (_mSearchUp != value)
				{
					_mSearchUp = value;

					RaisePropertyChanged(() => SearchUp);
				}
			}
		}

		/// <summary>
		/// Get/set whether to do a wildcard based search or not.
		/// </summary>
		public bool UseWildcards
        {
            get { return _mUseWildcards; }

			set
			{
				if (_mUseWildcards != value)
				{
					_mUseWildcards = value;

					RaisePropertyChanged(() => UseWildcards);
				}
			}
		}

		/// <summary>
		/// Get/set whether to search case sensitive or not.
		/// </summary>
		public bool CaseSensitive
        {
            get { return _mCaseSensitive; }

			set
			{
				if (_mCaseSensitive != value)
				{
					_mCaseSensitive = value;

					RaisePropertyChanged(() => CaseSensitive);
				}
			}
		}

		/// <summary>
		/// Get/set whether to search text with regular expressions or not.
		/// </summary>
		public bool UseRegEx
        {
            get { return _mUseRegEx; }

			set
			{
				if (_mUseRegEx != value)
				{
					_mUseRegEx = value;

					RaisePropertyChanged(() => UseRegEx);
				}
			}
		}

		/// <summary>
		/// Get/set whether to search for a whole word occurrance or not.
		/// </summary>
		public bool WholeWord
        {
            get { return _mWholeWord; }

			set
			{
				if (_mWholeWord != value)
				{
					_mWholeWord = value;

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
            get { return _mAcceptsReturn; }

			set
			{
				if (_mAcceptsReturn != value)
				{
					_mAcceptsReturn = value;

					RaisePropertyChanged(() => AcceptsReturn);
				}
			}
		}

		/// <summary>
		/// Get/set whether replace is to be displayed and used in view or not.
		/// </summary>
		public bool AllowReplace
        {
            get { return _mAllowReplace; }

			set
			{
				if (_mAllowReplace != value)
				{
					_mAllowReplace = value;

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
            get { return _mShowAsFind; }

			set
			{
				if (_mShowAsFind != value)
				{
					_mShowAsFind = value;

					if (value == true)            // Focus textbox on find tab or find/replace tab
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
            get { return _mIsTextToFindFocused; }

			set
			{
				if (_mIsTextToFindFocused != value)
				{
					_mIsTextToFindFocused = value;

					RaisePropertyChanged(() => IsTextToFindFocused);
				}
			}
		}

		/// <summary>
		/// </summary>
		public bool IsTextToFindInReplaceFocused
        {
            get { return _mIsTextToFindInReplaceFocused; }

			set
			{
				if (_mIsTextToFindInReplaceFocused != value)
				{
					_mIsTextToFindInReplaceFocused = value;

					RaisePropertyChanged(() => IsTextToFindInReplaceFocused);
				}
			}
		}

		/// <summary>
		/// Get/set whether to search in current document, all open documents etc.
		/// </summary>
		public SearchScope SearchIn
        {
            get { return _mSearchIn; }

			set
			{
				if (_mSearchIn != value)
				{
					_mSearchIn = value;

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
            get { return _mShowSearchIn; }

			private set
			{
				if (_mShowSearchIn != value)
				{
					_mShowSearchIn = value;

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
				return _mFindCommand ?? (_mFindCommand = new RelayCommand<object>((p) =>
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

						FindNext(this, _mSearchUp);
					}
				}));
			}
		}

		public ICommand ReplaceCommand
		{
			get
			{
				return _mReplaceCommand ?? (_mReplaceCommand = new RelayCommand<object>((p) =>
				{
					AddReplaceHistory();
					Replace();
				}));
			}
		}

		public ICommand ReplaceAllCommand
		{
			get
			{
				return _mReplaceAllCommand ?? (_mReplaceAllCommand = new RelayCommand<object>((p) =>
				{
					AddReplaceHistory();
					ReplaceAll();
				}));
			}
		}

		public List<string> FindHistory { get; }

		public List<string> ReplaceHistory { get; }

		/// <summary>
		/// Helper function to add replace history
		/// </summary>
		private void AddReplaceHistory()
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
		/// <param name="forceLeftToRight"></param>
		/// <returns>The regular expression.</returns>
		public Regex GetRegEx(bool forceLeftToRight = false)
		{
			// Multiline option is required to support matching start and end of line in regex
			// http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regexoptions.aspx
			// http://stackoverflow.com/questions/6596060/how-to-match-begin-or-end-of-line-using-c-regex
			Regex r;
			RegexOptions o = RegexOptions.Multiline;

			if (SearchUp && !forceLeftToRight)
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
			FindNext?.Invoke(this, true);
		}

		public void Replace()
		{
			IEditor ce = GetCurrentEditor();
			if (ce == null) return;

			// if currently selected text matches -> replace; anyways, find the next match
			Regex r = GetRegEx();
			string s = ce.Text.Substring(ce.SelectionStart, ce.SelectionLength); // CE.SelectedText;
			Match m = r.Match(s);
			if (m.Success && m.Index == 0 && m.Length == s.Length)
			{
				ce.Replace(ce.SelectionStart, ce.SelectionLength, ReplacementText);
				//CE.SelectedText = ReplacementText;
			}

			FindNext?.Invoke(this, false);
		}

		public void ReplaceAll(bool askBefore = true)
		{
			IEditor ce = GetCurrentEditor();
			if (ce == null) return;

			var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();

			if (!askBefore || msgBox.Show(string.Format(Util.Local.Strings.STR_FINDREPLACE_ASK_REALLY_REPLACEEVERYTHING, TextToFind, ReplacementText),
										  Util.Local.Strings.STR_FINDREPLACE_ReplaceAll_Caption,
										  MsgBoxButtons.YesNoCancel, MsgBoxImage.Alert) == MsgBoxResult.Yes)
			{
				object initialEditor = CurrentEditor;
				// loop through all editors, until we are back at the starting editor                
				do
				{
					Regex r = GetRegEx(true);   // force left to right, otherwise indices are screwed up
					int offset = 0;
					ce.BeginChange();
					foreach (Match m in r.Matches(ce.Text))
					{
						ce.Replace(offset + m.Index, m.Length, ReplacementText);
						offset += ReplacementText.Length - m.Length;
					}
					ce.EndChange();

					// XXX TODO CE = GetNextEditor();
				} while (CurrentEditor != initialEditor);
			}
		}

		public IEditor GetCurrentEditor()
		{
			if (CurrentEditor == null)
				return null;

			return CurrentEditor as IEditor;

			////      if (InterfaceConverter == null)
			////        return null;
			////
			////      return InterfaceConverter.Convert(CurrentEditor, typeof(IEditor), null, CultureInfo.CurrentCulture) as IEditor;
		}
		#endregion methods
	}
}
