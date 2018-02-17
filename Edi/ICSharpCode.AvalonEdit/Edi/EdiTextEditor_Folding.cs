namespace ICSharpCode.AvalonEdit.Edi
{
    using Folding;
    using AvalonEdit.Folding;
    using Highlighting;
    using System;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// This part of the AvalonEdit extension contains the code
    /// necessary to manage folding strategies for various types of text.
    /// </summary>
    public partial class EdiTextEditor : TextEditor
    {
        #region fields
        FoldingManager mFoldingManager = null;
        object mFoldingStrategy = null;

        private bool mInstallFoldingManager = false;
        private DispatcherTimer mFoldingUpdateTimer = null;
        #endregion fields

        #region Methods
        /// <summary>
        /// This method is executed via <seealso cref="TextEditor"/> class when the Highlighting for
        /// a text display is changed durring the live time of the control.
        /// </summary>
        /// <param name="newValue"></param>
        protected override void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
        {
            base.OnSyntaxHighlightingChanged(newValue);

            if (newValue != null)
                SetFolding(newValue);
        }

        /// <summary>
        /// Determine whether or not highlighting can be
        /// suppported by a particular folding strategy.
        /// </summary>
        /// <param name="syntaxHighlighting"></param>
        public void SetFolding(IHighlightingDefinition syntaxHighlighting)
        {
            if (syntaxHighlighting == null)
            {
                mFoldingStrategy = null;
            }
            else
            {
                switch (syntaxHighlighting.Name)
                {
                    case "XML":
                    case "HTML":
                        mFoldingStrategy = new XmlFoldingStrategy() { ShowAttributesWhenFolded = true };
                        TextArea.IndentationStrategy = new Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                        TextArea.IndentationStrategy = new Indentation.CSharp.CSharpIndentationStrategy(Options);
                        mFoldingStrategy = new CSharpBraceFoldingStrategy();
                        break;
                    case "C++":
                    case "PHP":
                    case "Java":
                        TextArea.IndentationStrategy = new Indentation.CSharp.CSharpIndentationStrategy(Options);
                        mFoldingStrategy = new CSharpBraceFoldingStrategy();
                        break;
                    case "VBNET":
                        TextArea.IndentationStrategy = new Indentation.CSharp.CSharpIndentationStrategy(Options);
                        mFoldingStrategy = new VBNetFoldingStrategy();
                        break;
                    default:
                        TextArea.IndentationStrategy = new Indentation.DefaultIndentationStrategy();
                        mFoldingStrategy = null;
                        break;
                }

                if (mFoldingStrategy != null)
                {
                    if (Document != null)
                    {
                        if (mFoldingManager == null)
                            mFoldingManager = FoldingManager.Install(TextArea);


                        if (mFoldingStrategy is AbstractFoldingStrategy)
                        {
                            AbstractFoldingStrategy abstractFolder = mFoldingStrategy as AbstractFoldingStrategy;
                            abstractFolder.UpdateFoldings(mFoldingManager, Document);
                        }
                    }
                    else
                        mInstallFoldingManager = true;
                }
                else
                {
                    if (mFoldingManager != null)
                    {
                        FoldingManager.Uninstall(mFoldingManager);
                        mFoldingManager = null;
                    }
                }
            }
        }

        /// <summary>
        /// Update the folding in the text editor when requested per call on this method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (IsVisible)
            {
                if (mInstallFoldingManager)
                {
                    if (Document != null)
                    {
                        if (mFoldingManager == null)
                        {
                            mFoldingManager = FoldingManager.Install(TextArea);

                            mInstallFoldingManager = false;
                        }
                    }
                    else
                        return;
                }

                if (mFoldingStrategy != null)
                {

                    if (mFoldingStrategy is AbstractFoldingStrategy)
                    {
                        AbstractFoldingStrategy abstractFolder = mFoldingStrategy as AbstractFoldingStrategy;
                        abstractFolder.UpdateFoldings(mFoldingManager, Document);
                    }
                }
            }
        }

        #region Fold Unfold Command
        /// <summary>
        /// Determines whether a folding command can be executed or not and sets correspondind
        /// <paramref name="e"/>.CanExecute property value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FoldsColapseExpandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;

            EdiTextEditor edi = sender as EdiTextEditor;

            if (edi == null)
                return;

            if (edi.mFoldingManager == null)
                return;

            if (edi.mFoldingManager.AllFoldings == null)
                return;

            e.CanExecute = true;
        }

        /// <summary>
        /// Executes the collapse all folds command (which folds all text foldings but the first).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FoldsCollapseAll(object sender, ExecutedRoutedEventArgs e)
        {
            EdiTextEditor edi = sender as EdiTextEditor;

            if (edi == null)
                return;

            edi.CollapseAllTextfoldings();
        }

        /// <summary>
        /// Executes the collapse all folds command (which folds all text foldings but the first).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FoldsExpandAll(object sender, ExecutedRoutedEventArgs e)
        {
            EdiTextEditor edi = sender as EdiTextEditor;

            if (edi == null)
                return;

            edi.ExpandAllTextFoldings();
        }

        /// <summary>
        /// Goes through all foldings in the displayed text and folds them
        /// so that users can explore the text in a top down manner.
        /// </summary>
        private void CollapseAllTextfoldings()
        {
            if (mFoldingManager == null)
                return;

            if (mFoldingManager.AllFoldings == null)
                return;

            foreach (var loFolding in mFoldingManager.AllFoldings)
            {
                loFolding.IsFolded = true;
            }

            // Unfold the first fold (if any) to give a useful overview on content
            FoldingSection foldSection = mFoldingManager.GetNextFolding(0);

            if (foldSection != null)
                foldSection.IsFolded = false;
        }

        /// <summary>
        /// Goes through all foldings in the displayed text and unfolds them
        /// so that users can see all text items (without having to play with folding).
        /// </summary>
        private void ExpandAllTextFoldings()
        {
            if (mFoldingManager == null)
                return;

            if (mFoldingManager.AllFoldings == null)
                return;

            foreach (var loFolding in mFoldingManager.AllFoldings)
            {
                loFolding.IsFolded = false;
            }
        }
        #endregion Fold Unfold Command
        #endregion
    }
}
