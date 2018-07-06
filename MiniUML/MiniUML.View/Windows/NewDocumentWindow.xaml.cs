namespace MiniUML.View.Windows
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using MiniUML.Framework;
    using MiniUML.Model.ViewModels.Document;
    using MiniUML.View.Converter;
    using MsgBox;
    using CommonServiceLocator;

    /// <summary>
    /// Interaction logic for NewDocumentWindow.xaml
    /// </summary>
    public partial class NewDocumentWindow : Window
    {
        private IMessageBoxService _MsgBox = null;

        public NewDocumentWindow()
        {
            this.InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            this.Title = (string)Application.Current.Resources["ApplicationName"];

            _MsgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
        }

        private bool getValues(out Size pageSize, out Thickness pageMargins)
        {
            pageSize = new Size();
            pageMargins = new Thickness();

            DiuToCentimetersConverter converter = new DiuToCentimetersConverter();

            try
            {
                double pageWidth = (double)converter.ConvertBack(_pageWidthTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
                double pageHeight = (double)converter.ConvertBack(_pageHeightTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
                double pageMarginTop = (double)converter.ConvertBack(_pageMarginTopTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
                double pageMarginBottom = (double)converter.ConvertBack(_pageMarginBottomTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
                double pageMarginLeft = (double)converter.ConvertBack(_pageMarginLeftTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
                double pageMarginRight = (double)converter.ConvertBack(_pageMarginRightTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);

                if (pageWidth < 0 || pageHeight < 0)
                {
                    _MsgBox.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_HEIGHT_WIDTH_NEGATIVE, pageWidth, pageHeight),
                                 MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                                 MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);

                    return false;
                }

                if (pageMarginTop < 0 || pageMarginRight < 0 || pageMarginLeft < 0 || pageMarginBottom < 0)
                {
                    _MsgBox.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_MARGINS_NEGATIVE),
                                 MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                                 MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);

                    return false;
                }

                if (pageMarginTop + pageMarginBottom > pageHeight || pageMarginLeft + pageMarginRight > pageWidth)
                {
                    _MsgBox.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_MARGIN_LARGER_THAN_PAGESIZE),
                                 MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                                 MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);

                    return false;
                }

                pageSize = new Size(pageWidth, pageHeight);
                pageMargins = new Thickness(pageMarginLeft, pageMarginTop, pageMarginRight, pageMarginBottom);
                return true;
            }
            catch (FormatException)
            {
                _MsgBox.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID),
                             MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                             MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);
            }
            catch (OverflowException)
            {
                _MsgBox.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID),
                             MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                             MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);
            }

            return false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            PageViewModelBase viewModel = DataContext as PageViewModelBase;

            Size pageSize;
            Thickness pageMargins;
            if (this.getValues(out pageSize, out pageMargins))
            {
                viewModel.prop_PageSize = pageSize;
                viewModel.prop_PageMargins = pageMargins;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void setDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Size pageSize;
            Thickness pageMargins;
            if (this.getValues(out pageSize, out pageMargins))
            {
                // TODO XXX SettingsManager.Settings["DefaultPageSize"] = pageSize;
                // TODO XXX SettingsManager.Settings["DefaultPageMargins"] = pageMargins;
                // TODO XXX SettingsManager.SaveSettings();
            }
        }
    }

    public class NewDocumentWindowFactory : IFactory
    {
        public object CreateObject()
        {
            return new NewDocumentWindow();
        }
    }
}
