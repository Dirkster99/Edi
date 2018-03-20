namespace Edi.Dialogs.GotoLine
{
	using System;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// This class implement the view part of a goto text editor line dialog
	/// as Custom (look-less) WPF Control.
	/// </summary>
	public class GotoLineView : Control
	{
		/// <summary>
		/// Link to required <seealso cref="TextBox"/> input control
		/// </summary>
		private TextBox _mTxtLineNumber;

		/// <summary>
		/// Style key for look-less control
		/// </summary>
		static GotoLineView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GotoLineView), new FrameworkPropertyMetadata(typeof(GotoLineView)));
		}

		/// <summary>
		/// Constructur
		/// </summary>
		public GotoLineView()
		{
			_mTxtLineNumber = null;
		}

		/// <summary>
		/// This method is executed when the XAML for this control is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			try
			{
				_mTxtLineNumber = GetTemplateChild("PART_TxtLineNumber") as TextBox;

				if (_mTxtLineNumber != null)
				{
					_mTxtLineNumber.Loaded += (s, e) =>  // Set textbox to be intially focussed
					{
						_mTxtLineNumber.Focus();
					};

					_mTxtLineNumber.GotKeyboardFocus += (s, e) =>
					{
						_mTxtLineNumber.SelectAll();
					};
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		/// <summary>
		/// Always select text content when ever this is (re-)rendered
		/// </summary>
		/// <param name="drawingContext"></param>
		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (_mTxtLineNumber != null)
				_mTxtLineNumber.SelectAll();
		}
	}
}
