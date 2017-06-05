namespace EdiDialogs.GotoLine
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
		private TextBox mTxtLineNumber;

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
			this.mTxtLineNumber = null;
		}

		/// <summary>
		/// This method is executed when the XAML for this control is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			try
			{
				this.mTxtLineNumber = this.GetTemplateChild("PART_TxtLineNumber") as TextBox;

				if (this.mTxtLineNumber != null)
				{
					this.mTxtLineNumber.Loaded += (s, e) =>  // Set textbox to be intially focussed
					{
						this.mTxtLineNumber.Focus();
					};

					this.mTxtLineNumber.GotKeyboardFocus += (s, e) =>
					{
						this.mTxtLineNumber.SelectAll();
					};
				}
			}
			catch (System.Exception e)
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

			if (this.mTxtLineNumber != null)
				this.mTxtLineNumber.SelectAll();
		}
	}
}
