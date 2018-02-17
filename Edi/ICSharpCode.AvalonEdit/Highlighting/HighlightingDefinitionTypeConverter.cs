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

using System;
using System.ComponentModel;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Converts between strings and <see cref="IHighlightingDefinition"/> by treating the string as the definition name
	/// and calling <c>HighlightingManager.Instance.<see cref="HighlightingManager.GetDefinition">GetDefinition</see>(name)</c>.
	/// </summary>
	public sealed class HighlightingDefinitionTypeConverter : TypeConverter
	{
		/// <inheritdoc/>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
		    if (sourceType == typeof(string))
				return true;
		    return base.CanConvertFrom(context, sourceType);
		}
		
		/// <inheritdoc/>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
		    if (value is string)
            {
                string definitionName = value as string;
                return HighlightingManager.Instance.GetDefinition(definitionName);
            }

		    return base.ConvertFrom(context, culture, value);
		}
		
		/// <inheritdoc/>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
		    if (destinationType == typeof(string))
				return true;
		    return base.CanConvertTo(context, destinationType);
		}
		
		/// <inheritdoc/>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
		    if (value is IHighlightingDefinition && destinationType == typeof(string))
            {
                IHighlightingDefinition definition = value as IHighlightingDefinition;
                return definition.Name;
            }

		    return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
