namespace MiniUML.View.Windows
{
/***
   Dirkster 2019/08 deactivated this since function causes compiler error and
                 this window appears not to be used right now
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using Edi.Core;
    using MiniUML.Framework;
    using MiniUML.Model.ViewModels.Document;
    using MsgBox;

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
                    StaticServices.MsgBox.Show(MiniUML.Framework.Local.Strings.STR_MSG_INVALID_RESOLUTION,
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
                StaticServices.MsgBox.Show(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID,
                                           MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                                           MsgBoxButtons.OK, MsgBoxImage.Warning);
            }
        }
    }

    public class ExportDocumentWindowFactory : IFactory
    {
        public object CreateObject(IMessageBoxService msgBox)
        {
            return new ExportDocumentWindow();
        }
    }
***/
}
