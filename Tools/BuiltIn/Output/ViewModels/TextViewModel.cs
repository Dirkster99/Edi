namespace Output.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using Edi.Core.Interfaces;
	using Edi.Core.ViewModels;
	using Edi.Core.ViewModels.Command;
	using ICSharpCode.AvalonEdit.Document;
	using ICSharpCode.AvalonEdit.Edi.TextBoxControl;
	using ICSharpCode.AvalonEdit.Highlighting;
	using UnitComboLib.Unit;
	using UnitComboLib.Unit.Screen;
	using UnitComboLib.ViewModel;

	public class TextViewModel : BaseViewModel
	{
		#region fields
		private TextDocument mDocument;
		private IHighlightingDefinition mHighlightingDefinition;

		private object mLockThis = new object();

		private bool mIsReadOnly = false;
		private int mLine = 0;
		private int mColumn = 0;

		private TextBoxController mTxtControl = null;

		// These properties are used to save and restore the editor state when CTRL+TABing between documents
		private int mTextEditorCaretOffset = 0;
		private int mTextEditorSelectionStart = 0;
		private int mTextEditorSelectionLength = 0;
		private bool mTextEditorIsRectangularSelection = false;
		private double mTextEditorScrollOffsetX = 0;
		private double mTextEditorScrollOffsetY = 0;

		private bool mWordWrap = false;            // Toggle state command
		private bool mShowLineNumbers = true;     // Toggle state command

		private ICSharpCode.AvalonEdit.TextEditorOptions mTextOptions
						= new ICSharpCode.AvalonEdit.TextEditorOptions() { IndentationSize = 2, ConvertTabsToSpaces = true };

		private bool mIsDirty = false;
		#endregion fields

		#region constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="documentViewModel"></param>
		public TextViewModel()
		{
			this.SizeUnitLabel = new UnitViewModel(this.GenerateScreenUnitList(), new ScreenConverter(), 0);

			this.TxtControl = new TextBoxController();

			this.mDocument = new TextDocument();

			this.TextEditorSelectionStart = 0;
			this.TextEditorSelectionLength = 0;

			// Set XML Highlighting for XML split view part of the UML document viewer
			this.mHighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(".txt");
		}
		#endregion constructor

		#region properties
		#region AvalonEdit properties
		#region IsReadOnly
		public bool IsReadOnly
		{
			get
			{
				return this.mIsReadOnly;
			}

			protected set
			{
				if (this.mIsReadOnly != value)
				{
					this.mIsReadOnly = value;
					this.RaisePropertyChanged(() => this.IsReadOnly);
				}
			}
		}
		#endregion IsReadOnly

		#region TextContent
		/// <summary>
		/// This property wraps the document class provided by AvalonEdit. The actual text is inside
		/// the document and can be accessed at save, load or other processing times.
		/// 
		/// The Text property itself cannot be bound in AvalonEdit since binding this would mResult
		/// in updating the text (via binding) each time a user enters a key on the keyboard
		/// (which would be a design error resulting in huge performance problems)
		/// </summary>
		public TextDocument Document
		{
			get
			{
				return this.mDocument;
			}

			set
			{
				if (this.mDocument != value)
				{
					this.mDocument = value;
					this.RaisePropertyChanged(() => this.Document);
				}
			}
		}
		#endregion

		#region ScaleView
		/// <summary>
		/// Scale view of text in percentage of font size
		/// </summary>
		public UnitViewModel SizeUnitLabel { get; set; }
		#endregion ScaleView

		#region CaretPosition
		// These properties are used to display the current column/line
		// of the cursor in the user interface
		public int Line
		{
			get
			{
				return this.mLine;
			}

			set
			{
				if (this.mLine != value)
				{
					this.mLine = value;
					this.RaisePropertyChanged(() => this.Line);
				}
			}
		}

		public int Column
		{
			get
			{
				return this.mColumn;
			}

			set
			{
				if (this.mColumn != value)
				{
					this.mColumn = value;
					this.RaisePropertyChanged(() => this.Column);
				}
			}
		}
		#endregion CaretPosition

		#region TxtControl
		public TextBoxController TxtControl
		{
			get
			{
				return this.mTxtControl;
			}

			private set
			{
				if (this.mTxtControl != value)
				{
					this.mTxtControl = value;
					this.RaisePropertyChanged(() => this.TxtControl);
				}
			}
		}
		#endregion TxtControl

		#region EditorStateProperties
		/// <summary>
		/// Get/set editor carret position
		/// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
		/// </summary>
		public int TextEditorCaretOffset
		{
			get
			{
				return this.mTextEditorCaretOffset;
			}

			set
			{
				if (this.mTextEditorCaretOffset != value)
				{
					this.mTextEditorCaretOffset = value;
					this.RaisePropertyChanged(() => this.TextEditorCaretOffset);
				}
			}
		}

		/// <summary>
		/// Get/set editor start of selection
		/// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
		/// </summary>
		public int TextEditorSelectionStart
		{
			get
			{
				return this.mTextEditorSelectionStart;
			}

			set
			{
				if (this.mTextEditorSelectionStart != value)
				{
					this.mTextEditorSelectionStart = value;
					this.RaisePropertyChanged(() => this.TextEditorSelectionStart);
				}
			}
		}

		/// <summary>
		/// Get/set editor length of selection
		/// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
		/// </summary>
		public int TextEditorSelectionLength
		{
			get
			{
				return this.mTextEditorSelectionLength;
			}

			set
			{
				if (this.mTextEditorSelectionLength != value)
				{
					this.mTextEditorSelectionLength = value;
					this.RaisePropertyChanged(() => this.TextEditorSelectionLength);
				}
			}
		}

		public bool TextEditorIsRectangularSelection
		{
			get
			{
				return this.mTextEditorIsRectangularSelection;
			}

			set
			{
				if (this.mTextEditorIsRectangularSelection != value)
				{
					this.mTextEditorIsRectangularSelection = value;
					this.RaisePropertyChanged(() => this.TextEditorIsRectangularSelection);
				}
			}
		}

		#region EditorScrollOffsetXY
		/// <summary>
		/// Current editor view scroll X position
		/// </summary>
		public double TextEditorScrollOffsetX
		{
			get
			{
				return this.mTextEditorScrollOffsetX;
			}

			set
			{
				if (this.mTextEditorScrollOffsetX != value)
				{
					this.mTextEditorScrollOffsetX = value;
					this.RaisePropertyChanged(() => this.TextEditorScrollOffsetX);
				}
			}
		}

		/// <summary>
		/// Current editor view scroll Y position
		/// </summary>
		public double TextEditorScrollOffsetY
		{
			get
			{
				return this.mTextEditorScrollOffsetY;
			}

			set
			{
				if (this.mTextEditorScrollOffsetY != value)
				{
					this.mTextEditorScrollOffsetY = value;
					this.RaisePropertyChanged(() => this.TextEditorScrollOffsetY);
				}
			}
		}
		#endregion EditorScrollOffsetXY
		#endregion EditorStateProperties

		/// <summary>
		/// AvalonEdit exposes a Highlighting property that controls whether keywords,
		/// comments and other interesting text parts are colored or highlighted in any
		/// other visual way. This property exposes the highlighting information for the
		/// text file managed in this viewmodel class.
		/// </summary>
		public IHighlightingDefinition HighlightingDefinition
		{
			get
			{
				lock (this.mLockThis)
				{
					return this.mHighlightingDefinition;
				}
			}

			set
			{
				lock (this.mLockThis)
				{
					if (this.mHighlightingDefinition != value)
					{
						this.mHighlightingDefinition = value;

						this.RaisePropertyChanged(() => this.HighlightingDefinition);
					}
				}
			}
		}

		public bool WordWrap
		{
			get
			{
				return this.mWordWrap;
			}

			set
			{
				if (this.mWordWrap != value)
				{
					this.mWordWrap = value;
					this.RaisePropertyChanged(() => this.WordWrap);
				}
			}
		}

		public bool ShowLineNumbers
		{
			get
			{
				return this.mShowLineNumbers;
			}

			set
			{
				if (this.mShowLineNumbers != value)
				{
					this.mShowLineNumbers = value;
					this.RaisePropertyChanged(() => this.ShowLineNumbers);
				}
			}
		}

		public bool ShowEndOfLine               // Toggle state command
		{
			get
			{
				return this.TextOptions.ShowEndOfLine;
			}

			set
			{
				if (this.TextOptions.ShowEndOfLine != value)
				{
					this.TextOptions.ShowEndOfLine = value;
					this.RaisePropertyChanged(() => this.ShowEndOfLine);
				}
			}
		}

		public bool ShowSpaces               // Toggle state command
		{
			get
			{
				return this.TextOptions.ShowSpaces;
			}

			set
			{
				if (this.TextOptions.ShowSpaces != value)
				{
					this.TextOptions.ShowSpaces = value;
					this.RaisePropertyChanged(() => this.ShowSpaces);
				}
			}
		}

		public bool ShowTabs               // Toggle state command
		{
			get
			{
				return this.TextOptions.ShowTabs;
			}

			set
			{
				if (this.TextOptions.ShowTabs != value)
				{
					this.TextOptions.ShowTabs = value;
					this.RaisePropertyChanged(() => this.ShowTabs);
				}
			}
		}

		public ICSharpCode.AvalonEdit.TextEditorOptions TextOptions
		{
			get
			{
				return this.mTextOptions;
			}

			set
			{
				if (this.mTextOptions != value)
				{
					this.mTextOptions = value;
					this.RaisePropertyChanged(() => this.TextOptions);
				}
			}
		}
		#endregion AvalonEdit properties

		#region IsDirty
		/// <summary>
		/// Get whether the current information was edit and needs to be saved or not.
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return this.mIsDirty;
			}

			set
			{
				if (this.mIsDirty != value)
				{
					this.mIsDirty = value;
					this.RaisePropertyChanged(() => this.IsDirty);
				}
			}
		}
		#endregion IsDirty
		#endregion properties

		#region methods
		/// <summary>
		/// Method removes all text from the current viewmodel.
		/// </summary>
		public void Clear()
		{
			Application.Current.Dispatcher.BeginInvoke(
			new Action(
			delegate
			{
				this.mDocument.Text = string.Empty;
			}
			));
		}

		/// <summary>
		/// Method appends a new line of text into the current output.
		/// </summary>
		public void AppendLine(string text)
		{
			this.Append(text + Environment.NewLine);
		}

		/// <summary>
		/// Method appends text into the current output.
		/// </summary>
		public void Append(string text)
		{
			Application.Current.Dispatcher.BeginInvoke(
			new Action(
					delegate
					{
						if (text != null)
							this.mDocument.Insert(mDocument.TextLength, text);
					}
			));
		}

		/// <summary>
		/// Initialize scale view of content to indicated value and unit.
		/// </summary>
		/// <param name="unit"></param>
		/// <param name="defaultValue"></param>
		public void InitScaleView(int unit, double defaultValue)
		{
			this.SizeUnitLabel = new UnitViewModel(this.GenerateScreenUnitList(), new ScreenConverter(), unit, defaultValue);
		}

		/// <summary>
		/// Initialize Scale View with useful units in percent and font point size
		/// </summary>
		/// <returns></returns>
		private ObservableCollection<ListItem> GenerateScreenUnitList()
		{
			ObservableCollection<ListItem> unitList = new ObservableCollection<ListItem>();

			var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
			var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

			unitList.Add(new ListItem(Itemkey.ScreenPercent, "percent", "%", percentDefaults));
			unitList.Add(new ListItem(Itemkey.ScreenFontPoints, "point", "pt", pointsDefaults));

			return unitList;
		}
		#endregion methods
	}
}
