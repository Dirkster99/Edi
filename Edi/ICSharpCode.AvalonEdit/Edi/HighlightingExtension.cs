﻿namespace ICSharpCode.AvalonEdit.Edi
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Xml;

  using Highlighting;
  using Highlighting.Themes;
  using Highlighting.Xshd;

  /// <summary>
  /// Class for handling file streams and association with xshd highlighting patterns and names
  /// </summary>
  public class HighlightingExtension
  {
     #region methods
    /// <summary>
    /// Register custom highlighting patterns for usage in the editor
    /// </summary>
    public static void RegisterCustomHighlightingPatterns(HighlightingThemes hlThemes = null)
    {
        HighlightingManager.Instance.InitializeDefinitions(hlThemes);

        string path = Path.GetDirectoryName(Application.ResourceAssembly.Location);
        path = Path.Combine(path , "AvalonEdit\\Highlighting");

        if (Directory.Exists(path))
        {
            // Some HighlightingDefinitions contain 'import' statements which means that some
            // XSHDs have to be loaded before others (an exception is thrown otherwise)
            // Therefore, we use filenames to indicate sequence for loading xshds
            SortedSet<string> files = new SortedSet<string>(Directory.GetFiles(path).Where(x =>
            {
                var extension = Path.GetExtension(x);
                return extension.Contains("xshd");
            }));

            foreach (var file in files)
            {
                var definition = LoadXshdDefinition(file);
                var hightlight = LoadHighlightingDefinition(file);

                HighlightingManager.Instance.RegisterHighlighting(definition.Name, definition.Extensions.ToArray(), hightlight);
            }
        }
    }

    private static XshdSyntaxDefinition LoadXshdDefinition(string fullName)
    {
      using (var reader = new XmlTextReader(fullName))
        return HighlightingLoader.LoadXshd(reader);
    }
                                                                                                        
    private static IHighlightingDefinition LoadHighlightingDefinition(string fullName)
    {
      using (var reader = new XmlTextReader(fullName))
        return HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
    #endregion methods
  }
}
