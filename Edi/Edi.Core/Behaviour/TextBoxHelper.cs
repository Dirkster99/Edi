namespace Edi.Core.Behaviour
{
	using System.Windows;
	using System.Windows.Controls;

	public static class TextBoxSelect
	{
		private static readonly DependencyProperty SelectedTextProperty =
				DependencyProperty.RegisterAttached(
						"SelectedText",
						typeof(string),
						typeof(TextBoxSelect),
						new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextChanged));

		public static string GetSelectedText(DependencyObject obj)
		{
			return (string)obj.GetValue(SelectedTextProperty);
		}

		public static void SetSelectedText(DependencyObject obj, string value)
		{
			obj.SetValue(SelectedTextProperty, value);
		}

		private static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
            if (obj is TextBox)
            {
                TextBox tb = obj as TextBox;
                if (e.OldValue == null && e.NewValue != null)
                {
                    tb.SelectionChanged += tb_SelectionChanged;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    tb.SelectionChanged -= tb_SelectionChanged;
                }


                if (e.NewValue is string)
                {
                    string newValue = e.NewValue as string;

                    if (newValue == tb.Text && newValue != tb.SelectedText) // Just select the complete text if new values and text content is equal
                        tb.SelectAll();
                    else
                        tb.SelectedText = newValue as string;
                }
            }
        }

		static void tb_SelectionChanged(object sender, RoutedEventArgs e)
		{
            if (sender is TextBox)
            {
                TextBox tb = sender as TextBox;
                SetSelectedText(tb, tb.SelectedText);
            }
        }

	}
}
