namespace ICSharpCode.AvalonEdit.Edi.Intellisense
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Xml.Serialization;

  using ICSharpCode.AvalonEdit.CodeCompletion;

  /// <summary>
  /// File based text completion class.
  /// </summary>
  public class FileCompletionDataProvider : ICompletionDataProvider
	{
		private static readonly Dictionary<string, IEnumerable<ICompletionData>> Data = new Dictionary<string, IEnumerable<ICompletionData>>();

    /// <summary>
    /// Get text completion data for a word at a certain position.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="position"></param>
    /// <param name="input"></param>
    /// <param name="highlightingName"></param>
    /// <returns></returns>
		public IEnumerable<ICompletionData> GetData(string text, int position, string input, string highlightingName)
		{

			if (!Data.Keys.Contains(highlightingName))
			{
				var result = GetData(highlightingName);
				Data.Add(highlightingName, result);
			}
			if (input == " ")
				return Data[highlightingName];
			return new List<ICompletionData>();
		}

    /// <summary>
    /// Get text completion data for a type of hightlighting.
    /// </summary>
    /// <param name="highlightingName"></param>
    /// <returns></returns>
		private IEnumerable<ICompletionData> GetData(string highlightingName)
		{
			try
			{			
			  var result = new List<ICompletionData>();
			  var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AvalonEdit\\Intellisense", "Keywords", "options.xml");
			  using (var sr = new StreamReader(path))
			  {
				  var ser = new XmlSerializer(typeof(List<KeywordsFileOption>));
				  var ops = (List<KeywordsFileOption>)ser.Deserialize(sr);

          string sLocation = System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location);

          var filePath = Path.Combine(sLocation, "AvalonEdit\\Intellisense", "Keywords",
					  ops
					  .Where(x => string.Compare(x.HighlightingName, highlightingName) == 0)
					  .Select(x => x.Filename)
					  .FirstOrDefault() ?? string.Empty);

				  if (File.Exists(filePath))
				  {
					  var w = GetWords(filePath);
					  result.AddRange(w);
				  }
			  }

			  return result;
			}
			catch (Exception exp)
			{
				// TODO: приделать логирование
        Console.WriteLine(exp.ToString());
        return new List<ICompletionData>();
			}
		}

		private IEnumerable<ICompletionData> GetWords(string filename)
		{
			return File.Exists(filename) ? 
				File.ReadAllLines(filename).Select(x => new TextCompletionData(x, x)).Cast<ICompletionData>().ToList() 
				: new List<ICompletionData>();
		}
	}

  /// <summary>
  /// Definition of filenames associated with a highlighting name.
  /// </summary>
	public class KeywordsFileOption
	{
    /// <summary>
    /// Name of highlighting to be associated with a file
    /// that contains text completion data.
    /// </summary>
		[XmlAttribute]
		public string HighlightingName { get; set; }

    /// <summary>
    /// Name of file that contains completion data for a type of highlighting.
    /// </summary>
		[XmlAttribute]
		public string Filename { get; set; }
	}
}