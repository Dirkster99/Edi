namespace ICSharpCode.AvalonEdit.Edi.BlockSurround
{
  /// <summary>
  /// Class can be used to mark/unmark text in a block of code.
  /// 
  /// Workflow:
  /// - The user selects a part of the text and hits CTRL+1
  /// - The editor marks the selected text as comment or
  ///   uncomments the selected section if the user selected
  ///   a comment.
  /// </summary>
  public class BlockDefinition
  {
    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public BlockDefinition(string blockstart,
                         string blockend,
                         BlockAt typeOfBlock,
                         string fileextension,
                         System.Windows.Input.Key key,
                         System.Windows.Input.ModifierKeys modifier = 0)
      : this()
    {
      TypeOfBlock = typeOfBlock;
      StartBlock = blockstart;
      EndBlock = blockend;
      FileExtension = fileextension;

      Key = key;
      Modifier = modifier;
    }

    /// <summary>
    /// Hide standard constructor to ensur construction with minimal
    /// required data items.
    /// </summary>
    /// <returns></returns>
    protected BlockDefinition()
    {
      TypeOfBlock = BlockAt.StartAndEnd;
      StartBlock = EndBlock = FileExtension = string.Empty;

      Key = System.Windows.Input.Key.D1;
      Modifier = System.Windows.Input.ModifierKeys.Control;
    }
    #endregion constructor

    #region enum
    /// <summary>
    /// Define whether a selection is parsed/edit
    /// at both ends or only at one end.
    /// </summary>
    public enum BlockAt
    {
      /// <summary>
      /// Edit selected text at its start only
      /// </summary>
      Start,

      /// <summary>
      /// Edit selected text at its end only
      /// </summary>
      End,

      /// <summary>
      /// Edit selected text at start and end.
      /// </summary>
      StartAndEnd,
    }
    #endregion enum

    #region properties
    /// <summary>
    /// Get/set type of block selection/change
    /// </summary>
    public BlockAt TypeOfBlock { get; set; }

    /// <summary>
    /// String to remove/add at the begining of a selection.
    /// </summary>
    public string StartBlock { get; set; }

    /// <summary>
    /// String to remove/add at the end of a selection.
    /// </summary>
    public string EndBlock { get; set; }

    /// <summary>
    /// Configures the file extension for which this selection should be applied.
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// Configures the key that a user can use to apply the selection add/remove function.
    /// </summary>
    public System.Windows.Input.Key Key { get; set; }

    /// <summary>
    /// Configures the key modifier (if any) that a user can use to apply the selection add/remove function.
    /// </summary>
    public System.Windows.Input.ModifierKeys Modifier { get; set; }
    #endregion properties
  }
}
