namespace MWindowInterfacesLib.MsgBox.Enums
{
  /// <summary>
  /// This enumeration describes the possible results from displaying a message. A result is directly
  /// equivalent to a GUI element (OK, Cancel, Yes, No button) that a user clicked to close the message view.
  /// </summary>
  public enum MsgBoxResult
  {
    /// <summary>
    /// This is mostly technically needed for properties that implement
    /// automatic magic, such as, setting a useful default button. This
    /// magic occurs only if this default parameter is set in the
    /// constructor/interface - otherwise the button set by the caller
    /// is used.
    /// </summary>
    None = 0,

    /// <summary>
    /// This can be used to tell the messagebox sub-system explicitly to not
    /// set any default button (which is rather un-uasual but possible if
    /// the user needs to determine somthing that has a real 50:50 chance
    /// of being ansered).
    /// 
    /// This Enum member can only be set in the defaultbutton parameter of the
    /// constructor but will never appear as actual result of a messagebox display.
    /// </summary>
    NoDefaultButton = 1,

    /*** A value greater one is a value that repesents an ectual button that can close the dialog ***/

    /// <summary>
    /// The result value of the message box is OK.
    /// </summary>
    OK = 2,

    /// <summary>
    /// The result value of the message box is Cancel.
    /// </summary>
    Cancel = 3,

    /// <summary>
    /// The result value of the message box is Yes.
    /// </summary>
    Yes = 4,

    /// <summary>
    /// The result value of the message box is No.
    /// </summary>
    No = 5,

    /// <summary>
    /// The result value of the message box is Close.
    /// </summary>
    Close = 6
  }
}
