namespace MiniUML.View.Windows
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using Framework;
    using Model.ViewModels.Document;

    /// <summary>
    /// Interaction logic for ExportDocumentWindow.xaml
    /// </summary>
    public partial class ExportDocumentWindow : Window
    {
        public ExportDocumentWindow()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            Title = (string)Application.Current.Resources["ApplicationName"];
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double resolution = double.Parse(_dpiTextBox.Text,
                       NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
                       CultureInfo.CurrentCulture);

                if (resolution <= 0)
                {
                    var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                    msgBox.Show(Framework.Local.Strings.STR_MSG_INVALID_RESOLUTION,
                                Framework.Local.Strings.STR_MSG_Warning_Caption,
                                MsgBoxButtons.OK, MsgBoxImage.Warning);
                    return;
                }

                ExportDocumentWindowViewModel viewModel = DataContext as ExportDocumentWindowViewModel;
                viewModel.prop_Resolution = resolution;

                DialogResult = true;
                Close();
            }
            catch (SystemException)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID,
                            Framework.Local.Strings.STR_MSG_Warning_Caption,
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
