namespace MiniUML.View.Windows
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Markup;
  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Document;
  using MsgBox;

  /// <summary>
  /// Interaction logic for ExportDocumentWindow.xaml
  /// </summary>
  public partial class ExportDocumentWindow : Window
  {
    public ExportDocumentWindow()
    {
      this.InitializeComponent();
      this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
      this.Title = (string)Application.Current.Resources["ApplicationName"];
    }

    private void okButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        double resolution = double.Parse(this._dpiTextBox.Text,
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.CurrentCulture);

        if (resolution <= 0)
        {
          Msg.Show(MiniUML.Framework.Local.Strings.STR_MSG_INVALID_RESOLUTION,
                   MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                   MsgBoxButtons.OK, MsgBoxImage.Warning);
          return;
        }

        ExportDocumentWindowViewModel viewModel = DataContext as ExportDocumentWindowViewModel;
        viewModel.prop_Resolution = resolution;

        this.DialogResult = true;
        this.Close();
      }
      catch (SystemException)
      {
        Msg.Show(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID,
                 MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                 MsgBoxButtons.OK, MsgBoxImage.Warning);
      }
    }
  }

  public class ExportDocumentWindowFactory : IFactory
  {
    public object CreateObject()
    {
      return new ExportDocumentWindow();
    }
  }
}
