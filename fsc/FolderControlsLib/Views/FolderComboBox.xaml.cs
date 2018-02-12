namespace FolderControlsLib.Views
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;

  /// <summary>
  /// Represents a selection control with a drop-down list that can be shown or
  /// hidden by clicking the arrow on the control.
  /// 
  /// This class implements a lookless combobox control with
  /// a file system entry drop-down selection that can be used to pre-select
  /// a list of useful combobox entries.
  /// </summary>
  [Localizability(LocalizationCategory.ComboBox)]
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ComboBoxItem))]
  [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
  [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
  public class FolderComboBox : ComboBox
  {
    #region constructor
    /// <summary>
    /// Static class constructor to register look-less <seealso cref="FolderComboBox"/> class
    /// control with the dependency property system.
    /// </summary>
    static FolderComboBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderComboBox), new FrameworkPropertyMetadata(typeof(FolderComboBox)));
    }

    /// <summary>
    /// Standard public class constructor.
    /// </summary>
    public FolderComboBox()
      : base()
    {
    }
    #endregion constructor
  }
}
