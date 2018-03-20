// SharpDevelop samples
// Copyright (c) 2010, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
namespace Edi.Documents.ViewModels.EdiDoc
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	using ICSharpCode.AvalonEdit.Document;
	using ICSharpCode.AvalonEdit.Highlighting;

	/// <summary>
	/// Implements a text to HTML converter that can be used to save highlighted text as HTML.
	/// 
	/// Source: https://github.com/xiaochuwang/SharpDevelop-master/blob/master/samples/HtmlSyntaxColorizer/HtmlWriter.cs
	/// 
	/// Usage:
	/// IHighlightingDefinition highlightDefinition = HighlightingManager.Instance.GetDefinition("C#");
	/// 
	/// HtmlWriter w = new HtmlWriter();
	/// w.ShowLineNumbers = true;
	/// w.AlternateLineBackground = true;
	/// 
	/// string code = File.ReadAllText("../../Main.cs");
	/// string html = w.GenerateHtml(code, highlightDefinition);
	/// File.WriteAllText("output.html", "<html><body>" + html + "</body></html>");
	/// 
	/// Process.Start("output.html"); // view in browser
	/// </summary>
	public class HtmlWriter
	{
		#region fields
		public bool AlternateLineBackground;
		public bool ShowLineNumbers;

		public string MainStyle = "font-size: small; font-family: Consolas, \"Courier New\", Courier, Monospace;";
		public string LineStyle = "margin: 0em;";
		public string AlternateLineStyle = "margin: 0em; width: 100%; background-color: #f0f0f0;";

		#region Stylesheet cache
		/// <summary>
		/// Specifies whether a CSS stylesheet should be created to reduce the size of the created code.
		/// The default value is true.
		/// </summary>
		public bool CreateStylesheet = true;

		/// <summary>
		/// Specifies the prefix to use in front of generate style class names.
		/// </summary>
		public string StyleClassPrefix = "code";

	    private readonly Dictionary<string, string> _stylesheetCache = new Dictionary<string, string>();
	    private StringBuilder _stylesheet = new StringBuilder();
		#endregion
		#endregion fields

		#region Stylesheet cache
		/// <summary>
		/// Resets the CSS stylesheet cache. Stylesheet classes will be cached on GenerateHtml calls.
		/// If you want to reuse the HtmlWriter for multiple .html files.
		/// </summary>
		public void ResetStylesheetCache()
		{
			_stylesheetCache.Clear();
		}

	    private string GetClass(string style)
		{
		    if (!_stylesheetCache.TryGetValue(style, out var className))
            {
                className = StyleClassPrefix + _stylesheetCache.Count;
                _stylesheet.Append('.');
                _stylesheet.Append(className);
                _stylesheet.Append(" { ");
                _stylesheet.Append(style);
                _stylesheet.AppendLine(" }");
                _stylesheetCache[style] = className;
            }
            return className;
		}
		#endregion Stylesheet cache

		#region methods
		public string GenerateHtml(string code, IHighlightingDefinition highlightDefinition)
		{
			return GenerateHtml(new TextDocument(code), highlightDefinition);
		}

		public string GenerateHtml(TextDocument document, IHighlightingDefinition highlightDefinition)
		{
			DocumentHighlighter documentHighlighter = null;

			if (highlightDefinition != null)
				documentHighlighter = new DocumentHighlighter(document, highlightDefinition);

			return GenerateHtml(document, documentHighlighter);
		}

		public string GenerateHtml(TextDocument document, IHighlighter highlighter)
		{
			string myMainStyle = MainStyle;
			string lineNumberStyle = "color: #606060;";

			DocumentLine docline = null;
			string textLine = null;

			if (highlighter == null)
				docline = document.GetLineByNumber(1);

			StringWriter output = new StringWriter();
			if (ShowLineNumbers || AlternateLineBackground)
			{
				output.Write("<div");
				WriteStyle(output, myMainStyle);
				output.WriteLine(">");

				int longestNumberLength = 1 + (int)Math.Log10(document.LineCount);

				for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
				{
					HighlightedLine line = null;

					if (highlighter != null)
						line = highlighter.HighlightLine(lineNumber);
					else
					{
						textLine = document.GetText(docline);
						docline = docline.NextLine;
					}

					output.Write("<pre");
					if (AlternateLineBackground && (lineNumber % 2) == 0)
					{
						WriteStyle(output, AlternateLineStyle);
					}
					else
					{
						WriteStyle(output, LineStyle);
					}
					output.Write(">");

					if (ShowLineNumbers)
					{
						output.Write("<span");
						WriteStyle(output, lineNumberStyle);
						output.Write('>');
						output.Write(lineNumber.ToString().PadLeft(longestNumberLength));
						output.Write(":  ");
						output.Write("</span>");
					}

					PrintWords(output, line, textLine);
					output.WriteLine("</pre>");
				}
				output.WriteLine("</div>");
			}
			else
			{
				output.Write("<pre");
				WriteStyle(output, myMainStyle + LineStyle);
				output.WriteLine(">");
				for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
				{
					HighlightedLine line = highlighter.HighlightLine(lineNumber);
					PrintWords(output, line, textLine);
					output.WriteLine();
				}
				output.WriteLine("</pre>");
			}
			if (CreateStylesheet && _stylesheet.Length > 0)
			{
				string result = "<style type=\"text/css\">" + _stylesheet + "</style>" + output;
				_stylesheet = new StringBuilder();
				return result;
			}

		    return output.ToString();
		}   

	    private void PrintWords(TextWriter writer, HighlightedLine line, string text)
	    {
	        writer.Write(line != null ? line.ToHtml(new MyHtmlOptions(this)) : text);
	    }

	    private void WriteStyle(TextWriter writer, string style)
		{
			if (CreateStylesheet)
			{
				writer.Write(" class=\"");
				writer.Write(GetClass(style));
				writer.Write('"');
			}
			else
			{
				writer.Write(" style='");
				writer.Write(style);
				writer.Write("'");
			}
		}
		#endregion methods

		#region private class
		private class MyHtmlOptions : HtmlOptions
		{
		    private readonly HtmlWriter _htmlWriter;

			internal MyHtmlOptions(HtmlWriter htmlWriter)
			{
				this._htmlWriter = htmlWriter;
			}

			public override void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
			{
				_htmlWriter.WriteStyle(writer, color.ToCss());
			}
		}
		#endregion private class
	}
}
