namespace EdiApp.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Media;
	using EdiDocuments.ViewModels.EdiDoc;
	using ICSharpCode.AvalonEdit.Edi;
	using ICSharpCode.AvalonEdit.Highlighting;
	using ICSharpCode.AvalonEdit.Highlighting.Themes;
	using MsgBox;
	using Themes.Definition;

	public partial class ApplicationViewModel
	{
		/// <summary>
		/// Change WPF theme.
		/// 
		/// This method can be called when the theme is to be reseted by all means
		/// (eg.: when powering application up).
		/// 
		/// !!! Use the CurrentTheme property to change !!!
		/// !!! the theme when App is running           !!!
		/// </summary>
		public void ResetTheme()
		{
			// Reset customized resources (if there are any from last change) and
			// enforce reload of original values from resource dictionary
			if (HighlightingManager.Instance.BackupDynResources != null)
			{
				try
				{
					foreach (string t in HighlightingManager.Instance.BackupDynResources)
					{
						Application.Current.Resources[t] = null;
					}
				}
				catch
				{
				}
				finally
				{
					HighlightingManager.Instance.BackupDynResources = null;
				}
			}

			// Get WPF Theme definition from Themes Assembly
			ThemeBase nextThemeToSwitchTo = this.mThemesManager.SelectedTheme;
			this.SwitchToSelectedTheme(nextThemeToSwitchTo);

			// Backup highlighting names (if any) and restore highlighting associations after reloading highlighting definitions
			var HlNames = new List<string>();

			foreach (EdiViewModel f in this.Documents)
			{
				if (f != null)
				{
					if (f.HighlightingDefinition != null)
						HlNames.Add(f.HighlightingDefinition.Name);
					else
						HlNames.Add(null);
				}
			}

			// Is the current theme configured with a highlighting theme???
			////this.Config.FindHighlightingTheme(
			HighlightingThemes hlThemes = nextThemeToSwitchTo.HighlightingStyles;

			// Re-load all highlighting patterns and re-apply highlightings
			HighlightingExtension.RegisterCustomHighlightingPatterns(hlThemes);

			//Re-apply highlightings after resetting highlighting manager
			List<EdiViewModel> l = this.Documents;
			for (int i = 0; i < l.Count; i++)
			{
				if (l[i] != null)
				{
					if (HlNames[i] == null) // The highlighting is null if highlighting is switched off for this file(!)
						continue;

					IHighlightingDefinition hdef = HighlightingManager.Instance.GetDefinition(HlNames[i]);

					if (hdef != null)
						l[i].HighlightingDefinition = hdef;
				}
			}

			var backupDynResources = new List<string>();

			// Apply global styles to theming elements (dynamic resources in resource dictionary) of editor control
			if (HighlightingManager.Instance.HlThemes != null)
			{
				foreach (WidgetStyle w in HighlightingManager.Instance.HlThemes.GlobalStyles)
					ApplyWidgetStyle(w, backupDynResources);
			}

			if (backupDynResources.Count > 0)
				HighlightingManager.Instance.BackupDynResources = backupDynResources;
		}

		/// <summary>
		/// Applies a widget style to a dynamic resource in the WPF Resource Dictionary.
		/// The intention is to be more flexible and allow users to configure other editor
		/// theme colors than thoses that are pre-defined.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="backupDynResources"></param>
		private void ApplyWidgetStyle(WidgetStyle w,
																	List<string> backupDynResources)
		{
			if (w == null)
				return;

			switch (w.Name)
			{
				case "DefaultStyle":
					ApplyToDynamicResource("EditorBackground", w.bgColor, backupDynResources);
					ApplyToDynamicResource("EditorForeground", w.fgColor, backupDynResources);
					break;

				case "CurrentLineBackground":
					ApplyToDynamicResource("EditorCurrentLineBackgroundColor", w.bgColor, backupDynResources);
					break;

				case "LineNumbersForeground":
					ApplyToDynamicResource("EditorLineNumbersForeground", w.fgColor, backupDynResources);
					break;

				case "Selection":
					ApplyToDynamicResource("EditorSelectionBrush", w.bgColor, backupDynResources);
					ApplyToDynamicResource("EditorSelectionBorder", w.borderColor, backupDynResources);
					ApplyToDynamicResource("EditorSelectionForeground", w.fgColor, backupDynResources);
					break;

				case "Hyperlink":
					ApplyToDynamicResource("LinkTextBackgroundBrush", w.bgColor, backupDynResources);
					ApplyToDynamicResource("LinkTextForegroundBrush", w.fgColor, backupDynResources);
					break;

				case "NonPrintableCharacter":
					ApplyToDynamicResource("NonPrintableCharacterBrush", w.fgColor, backupDynResources);
					break;

				default:
					logger.WarnFormat("WidgetStyle named '{0}' is not supported.", w.Name);
					break;
			}
		}

		/// <summary>
		/// Re-define an existing <seealso cref="SolidColorBrush"/> and backup the originial color
		/// as it was before the application of the custom coloring.
		/// </summary>
		/// <param name="ResourceName"></param>
		/// <param name="NewColor"></param>
		/// <param name="backupDynResources"></param>
		private void ApplyToDynamicResource(string ResourceName,
																				SolidColorBrush NewColor,
																				List<string> backupDynResources)
		{
			if (Application.Current.Resources[ResourceName] != null && NewColor != null)
			{
				// Re-coloring works with SolidColorBrushs linked as DynamicResource
				if (Application.Current.Resources[ResourceName] is SolidColorBrush)
				{
					var oldBrush = Application.Current.Resources[ResourceName] as SolidColorBrush;
					backupDynResources.Add(ResourceName);

					Application.Current.Resources[ResourceName] = NewColor.Clone();
				}
			}
		}

		/// <summary>
		/// Attempt to switch to the theme stated in <paramref name="nextThemeToSwitchTo"/>.
		/// The given name must map into the <seealso cref="Themes.ThemesVM.EnTheme"/> enumeration.
		/// </summary>
		/// <param name="nextThemeToSwitchTo"></param>
		private bool SwitchToSelectedTheme(ThemeBase nextThemeToSwitchTo)
		{
			const string themesModul = "Themes.dll";

			try
			{
				// set the style of the message box display in back-end system.
				Msg.Style = MsgBoxStyle.System;

				// Get WPF Theme definition from Themes Assembly
				ThemeBase theme = this.mThemesManager.SelectedTheme;

				if (theme != null)
				{
					Application.Current.Resources.MergedDictionaries.Clear();

					string ThemesPathFileName = Assembly.GetEntryAssembly().Location;

					ThemesPathFileName = System.IO.Path.GetDirectoryName(ThemesPathFileName);
					ThemesPathFileName = System.IO.Path.Combine(ThemesPathFileName, themesModul);
					Assembly assembly = Assembly.LoadFrom(ThemesPathFileName);

					if (System.IO.File.Exists(ThemesPathFileName) == false)
					{
						MsgBox.Msg.Show(string.Format(CultureInfo.CurrentCulture,
																					Util.Local.Strings.STR_THEMING_MSG_CANNOT_FIND_PATH, themesModul),
																					Util.Local.Strings.STR_THEMING_CAPTION,
																					MsgBoxButtons.OK, MsgBoxImage.Error);

						return false;
					}

					foreach (var item in theme.Resources)
					{
						try
						{
							var Res = new Uri(item, UriKind.Relative);


                            if (Application.LoadComponent(Res) is ResourceDictionary dictionary)
                                Application.Current.Resources.MergedDictionaries.Add(dictionary);
                        }
						catch (Exception Exp)
						{
							MsgBox.Msg.Show(Exp, string.Format(CultureInfo.CurrentCulture, "'{0}'", item), MsgBoxButtons.OK, MsgBoxImage.Error);
						}
					}
				}
			}
			catch (Exception exp)
			{
				MsgBox.Msg.Show(exp, Util.Local.Strings.STR_THEMING_CAPTION,
												MsgBoxButtons.OK, MsgBoxImage.Error);

				return false;
			}
			finally
			{
				// set the style of the message box display in back-end system.
				if (nextThemeToSwitchTo.WPFThemeName != "Generic")
					Msg.Style = MsgBoxStyle.WPFThemed;
			}

			return true;
		}
	}
}
