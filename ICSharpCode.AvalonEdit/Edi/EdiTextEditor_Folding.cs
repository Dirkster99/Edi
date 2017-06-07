namespace ICSharpCode.AvalonEdit.Edi
{
    using ICSharpCode.AvalonEdit.Edi.Folding;
    using ICSharpCode.AvalonEdit.Folding;
    using ICSharpCode.AvalonEdit.Highlighting;
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
                this.SetFolding(newValue);
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
                this.mFoldingStrategy = null;
            }
            else
            {
                switch (syntaxHighlighting.Name)
                {
                    case "XML":
                    case "HTML":
                        mFoldingStrategy = new XmlFoldingStrategy() { ShowAttributesWhenFolded = true };
                        this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                        this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
                        mFoldingStrategy = new CSharpBraceFoldingStrategy();
                        break;
                    case "C++":
                    case "PHP":
                    case "Java":
                        this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
                        mFoldingStrategy = new CSharpBraceFoldingStrategy();
                        break;
                    case "VBNET":
                        this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
                        mFoldingStrategy = new VBNetFoldingStrategy();
                        break;
                    default:
                        this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        mFoldingStrategy = null;
                        break;
                }

                if (mFoldingStrategy != null)
                {
                    if (this.Document != null)
                    {
                        if (mFoldingManager == null)
                            mFoldingManager = FoldingManager.Install(this.TextArea);


                        if (this.mFoldingStrategy is AbstractFoldingStrategy abstractFolder)
                            abstractFolder.UpdateFoldings(mFoldingManager, this.Document);
                    }
                    else
                        this.mInstallFoldingManager = true;
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
            if (this.IsVisible == true)
            {
                if (mInstallFoldingManager == true)
                {
                    if (this.Document != null)
                    {
                        if (mFoldingManager == null)
                        {
                            this.mFoldingManager = FoldingManager.Install(this.TextArea);

                            mInstallFoldingManager = false;
                        }
                    }
                    else
                        return;
                }

                if (mFoldingStrategy != null)
                {

                    if (this.mFoldingStrategy is AbstractFoldingStrategy abstractFolder)
                        abstractFolder.UpdateFoldings(mFoldingManager, this.Document);
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
            if (this.mFoldingManager == null)
                return;

            if (this.mFoldingManager.AllFoldings == null)
                return;

            foreach (var loFolding in this.mFoldingManager.AllFoldings)
            {
                loFolding.IsFolded = true;
            }

            // Unfold the first fold (if any) to give a useful overview on content
            FoldingSection foldSection = this.mFoldingManager.GetNextFolding(0);

            if (foldSection != null)
                foldSection.IsFolded = false;
        }

        /// <summary>
        /// Goes through all foldings in the displayed text and unfolds them
        /// so that users can see all text items (without having to play with folding).
        /// </summary>
        private void ExpandAllTextFoldings()
        {
            if (this.mFoldingManager == null)
                return;

            if (this.mFoldingManager.AllFoldings == null)
                return;

            foreach (var loFolding in this.mFoldingManager.AllFoldings)
            {
                loFolding.IsFolded = false;
            }
        }
        #endregion Fold Unfold Command
        #endregion
    }
}
