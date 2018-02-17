// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Windows;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Default implementation for TextRunTypographyProperties.
	/// </summary>
	public class DefaultTextRunTypographyProperties : TextRunTypographyProperties
	{
		/// <inheritdoc/>
		public override FontVariants Variants => FontVariants.Normal;

	    /// <inheritdoc/>
		public override bool StylisticSet1 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet2 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet3 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet4 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet5 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet6 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet7 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet8 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet9 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet10 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet11 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet12 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet13 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet14 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet15 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet16 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet17 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet18 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet19 => false;

	    /// <inheritdoc/>
		public override bool StylisticSet20 => false;

	    /// <inheritdoc/>
		public override int StylisticAlternates => 0;

	    /// <inheritdoc/>
		public override int StandardSwashes => 0;

	    /// <inheritdoc/>
		public override bool StandardLigatures => true;

	    /// <inheritdoc/>
		public override bool SlashedZero => false;

	    /// <inheritdoc/>
		public override FontNumeralStyle NumeralStyle => FontNumeralStyle.Normal;

	    /// <inheritdoc/>
		public override FontNumeralAlignment NumeralAlignment => FontNumeralAlignment.Normal;

	    /// <inheritdoc/>
		public override bool MathematicalGreek => false;

	    /// <inheritdoc/>
		public override bool Kerning => true;

	    /// <inheritdoc/>
		public override bool HistoricalLigatures => false;

	    /// <inheritdoc/>
		public override bool HistoricalForms => false;

	    /// <inheritdoc/>
		public override FontFraction Fraction => FontFraction.Normal;

	    /// <inheritdoc/>
		public override FontEastAsianWidths EastAsianWidths => FontEastAsianWidths.Normal;

	    /// <inheritdoc/>
		public override FontEastAsianLanguage EastAsianLanguage => FontEastAsianLanguage.Normal;

	    /// <inheritdoc/>
		public override bool EastAsianExpertForms => false;

	    /// <inheritdoc/>
		public override bool DiscretionaryLigatures => false;

	    /// <inheritdoc/>
		public override int ContextualSwashes => 0;

	    /// <inheritdoc/>
		public override bool ContextualLigatures => true;

	    /// <inheritdoc/>
		public override bool ContextualAlternates => true;

	    /// <inheritdoc/>
		public override bool CaseSensitiveForms => false;

	    /// <inheritdoc/>
		public override bool CapitalSpacing => false;

	    /// <inheritdoc/>
		public override FontCapitals Capitals => FontCapitals.Normal;

	    /// <inheritdoc/>
		public override int AnnotationAlternates => 0;
	}
}
