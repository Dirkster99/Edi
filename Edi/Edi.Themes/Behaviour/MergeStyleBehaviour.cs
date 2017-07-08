namespace Edi.Themes.Behaviour
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Globalization;

	/// <summary>
	/// This class can be used to adjust styles that are BasedOn another style when
	/// changing a theme at run-time. Normally, styles are not merged. This class
	/// however enables merging of existing partial style definitions in Window/control
	/// XAML with theme specific XAML.
	/// 
	/// Sample
	/// Usage: http://social.msdn.microsoft.com/Forums/da-DK/wpf/thread/63696841-0358-4f7a-abe1-e6062518e3d6
	/// Source: http://stackoverflow.com/questions/5223133/merge-control-style-with-global-style-set-by-another-project-dynamically
	/// </summary>
	public class MergeStyleBehaviour
	{
		#region fields
		/// <summary>
		/// AutoMergeStyle
		/// </summary>
		public static readonly DependencyProperty AutoMergeStyleProperty =
		DependencyProperty.RegisterAttached("AutoMergeStyle", typeof(bool), typeof(MergeStyleBehaviour),
				new FrameworkPropertyMetadata((bool)false,
						new PropertyChangedCallback(OnAutoMergeStyleChanged)));

		/// <summary>
		/// BaseOnStyle
		/// </summary>
		public static readonly DependencyProperty BaseOnStyleProperty =
				DependencyProperty.RegisterAttached("BaseOnStyle", typeof(Style), typeof(MergeStyleBehaviour),
						new FrameworkPropertyMetadata((Style)null,
								new PropertyChangedCallback(OnBaseOnStyleChanged)));

		/// <summary>
		/// OriginalStyle
		/// </summary>
		public static readonly DependencyProperty OriginalStyleProperty =
													 DependencyProperty.RegisterAttached("OriginalStyle", typeof(Style), typeof(MergeStyleBehaviour),
													 new FrameworkPropertyMetadata((Style)null));
		#endregion fields

		#region public static methods
		/// <summary>
		/// AutoMergeStyle
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static bool GetAutoMergeStyle(DependencyObject d)
		{
			try
			{
				return (bool)d.GetValue(AutoMergeStyleProperty);
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}

			return false;
		}

		/// <summary>
		/// AutoMergeStyle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="value"></param>
		public static void SetAutoMergeStyle(DependencyObject d, bool value)
		{
			try
			{
				d.SetValue(AutoMergeStyleProperty, value);
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}
		}

		/// <summary>
		/// BaseOnStyle
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Style GetBaseOnStyle(DependencyObject d)
		{
			try
			{
				return (Style)d.GetValue(BaseOnStyleProperty);
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}

			return null;
		}

		/// <summary>
		/// BaseOnStyle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="value"></param>
		public static void SetBaseOnStyle(DependencyObject d, Style value)
		{
			try
			{
				////Console.WriteLine("Behavior::SetBaseOnStyle");
				d.SetValue(BaseOnStyleProperty, value);
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}
		}

		/// <summary>
		/// OriginalStyle
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Style GetOriginalStyle(DependencyObject d)
		{
			try
			{
				return (Style)d.GetValue(OriginalStyleProperty);
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}

			return null;
		}

		/// <summary>
		/// OriginalStyle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="value"></param>
		public static void SetOriginalStyle(DependencyObject d, Style value)
		{
			try
			{
				d.SetValue(OriginalStyleProperty, value);
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}
		}
		#endregion public static methods

		#region private static methods
		/// <summary>
		/// AutoMergeStyle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnAutoMergeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			try
			{
				if (e.OldValue == e.NewValue)
				{
					return;
				}

				Control control = d as Control;
				if (control == null)
				{
					throw new NotSupportedException("AutoMergeStyle can only used in Control");
				}

				if ((bool)e.NewValue)
				{
					Type type = d.GetType();
					control.SetResourceReference(MergeStyleBehaviour.BaseOnStyleProperty, type);
				}
				else
				{
					control.ClearValue(MergeStyleBehaviour.BaseOnStyleProperty);
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}
		}

		/// <summary>
		/// BaseOnStyle
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnBaseOnStyleChanged(DependencyObject d,
																						 DependencyPropertyChangedEventArgs e)
		{
			if (e == null) return;

			if (d == null) return;

			//// if (e.OldValue == null) return;

			Control control = null;
			try
			{
				if (e.OldValue == e.NewValue)
				{
					return;
				}

				control = d as Control;
				if (control == null)
				{
					throw new NotSupportedException("BaseOnStyle can only be used in Control");
				}

				Style baseOnStyle = e.NewValue as Style;
				Style originalStyle = GetOriginalStyle(control);

				if (originalStyle == null)
				{
					originalStyle = control.Style;
					SetOriginalStyle(control, originalStyle);
				}

				Style newStyle = originalStyle;

				if (originalStyle.IsSealed)
				{
					////Console.WriteLine("+++ ORIGINAL STYLE IS SEALED. +++ ");

					newStyle = new Style(originalStyle.TargetType);
					////newStyle.TargetType = originalStyle.TargetType;

					// 1. Copy resources, setters, triggers
					newStyle.Resources = originalStyle.Resources;
					foreach (var st in originalStyle.Setters)
					{
						newStyle.Setters.Add(st);
					}

					foreach (var tg in originalStyle.Triggers)
					{
						newStyle.Triggers.Add(tg);
					}

					// 2. Set BaseOn Style
					newStyle.BasedOn = baseOnStyle;
				}
				else
					originalStyle.BasedOn = baseOnStyle;

				try
				{
					control.Style = newStyle;
				}
				catch (Exception exp)
				{
					string sInfo = string.Format(CultureInfo.CurrentCulture, "newStyle: {0}", (newStyle != null ? newStyle.TargetType.FullName : "(null)"));
					sInfo += string.Format(CultureInfo.CurrentCulture, "DependencyObject d: {0}", (d != null ? d.ToString() : "(null)"));

					Console.WriteLine(exp.ToString() + Environment.NewLine + Environment.NewLine + sInfo);
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.ToString());
			}
		}
	}
		#endregion private static methods
}
