namespace MWindowInterfacesLib.MsgBox.Enums
{
  /// <summary>
  /// Type of images that can be displayed in a message box
  /// (indexes need to increment in the shown order because they
  /// into a static array: MsgBoxViewModel.MsgBoxImageResourcesUris.
  /// </summary>
  public enum MsgBoxImage
  {
    /// <summary>
    /// Display a standard image to indicate an imformal message
    /// </summary>
    Information = 0,
    
    /// <summary>
    /// Display a standard image to indicate a message that contains a question
    /// </summary>
    Question = 1,

    /// <summary>
    /// Display a standard image to indicate a message that contains an error
    /// </summary>
    Error = 2,

    /// <summary>
    /// Display a standard image to indicate a message that contains nice to know information
    /// </summary>
    OK = 3,

    /// <summary>
    /// Display a standard image to indicate a message that contains information about a fatal problem.
    /// </summary>
    Alert = 4,

    /// <summary>
    /// Display a standard image to indicate a message that is not classified.
    /// </summary>
    Default = 5,

    /// <summary>
    /// Display a standard image to indicate a message that contains a warning.
    /// </summary>
    Warning = 6,

    /// <summary>
    /// Display a standard image with a light turned off.
    /// </summary>
    Default_OffLight = 7,

    /// <summary>
    /// Display a standard image with a red light turned on.
    /// </summary>
    Default_RedLight = 8,
    
    /// <summary>
    /// Display a standard image with a orange light turned on.
    /// </summary>
    Information_Orange = 9,

    /// <summary>
    /// Display a standard image indicating an important information.
    /// </summary>
    Information_Red = 10,

    /// <summary>
    /// Display a standard image indicating the end of a process run or program exit.
    /// </summary>
    Process_Stop = 11,

    /// <summary>
    /// Do not show an image at all.
    /// </summary>
    None = 12
  }
}
