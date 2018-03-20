namespace Edi.Dialogs.FindReplace
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using Core.Utillities;

	/// <summary>
	/// Implement a view that supports text Find/Replace functionality in an editor
	/// </summary>
	public class FindReplaceView : Control
	{
		private ComboBox _mTxtFind = null;
		private ComboBox _mTxtFind2 = null;
		private ComboBox _mTxtReplace = null;

		static FindReplaceView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FindReplaceView), new FrameworkPropertyMetadata(typeof(FindReplaceView)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			try
			{
				_mTxtFind = GetTemplateChild("PART_TxtFind") as ComboBox;
				_mTxtFind2 = GetTemplateChild("PART_TxtFind2") as ComboBox;
				_mTxtReplace = GetTemplateChild("PART_TxtReplace") as ComboBox;

				// Setting focus into each textbox control is controlled via viewmodel and attached property
				// Each textbox selects all content (by default) when it aquires the focus
				FocusEditableComboBox(_mTxtFind);
				FocusEditableComboBox(_mTxtFind2);
				FocusEditableComboBox(_mTxtReplace);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		/// <summary>
		/// Helper function to focus the textbox inside an editable combobox
		/// </summary>
		/// <param name="ediableComboBox"></param>
		private static void FocusEditableComboBox(ComboBox ediableComboBox)
		{
			if (ediableComboBox != null)
			{
				ediableComboBox.GotKeyboardFocus += (s, e) =>
				{
                    // focus the TextBox inside the ComboBox
                    if (ediableComboBox.FindChild("PART_EditableTextBox") is TextBox)
                    {
                        TextBox textBox = ediableComboBox.FindChild("PART_EditableTextBox") as TextBox;

                        textBox.Focus();
                    }
                };
			}
		}
	}
}
