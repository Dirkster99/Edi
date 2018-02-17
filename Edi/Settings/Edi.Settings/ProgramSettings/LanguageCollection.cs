﻿namespace Edi.Settings.ProgramSettings
{
	using System;

	/// <summary>
	/// Base class for enumeration over languages (and their locale) that
	/// are supported with specific (non-English) button and tool tip strings.
	/// 
	/// The class definition is based on BCP 47 which in turn is used to
	/// set the UI and thread culture (which in turn selects the correct
	/// string resource in each referenced assembly).
	/// </summary>
	public class LanguageCollection
	{
		public string Language { get; set; }
		public string Locale { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// Get BCP47 language tag for this language
		/// See also http://en.wikipedia.org/wiki/IETF_language_tag
		/// </summary>
		public string BCP47
		{
			get
			{
			    if (string.IsNullOrEmpty(Locale) == false)
					return $"{Language}-{Locale}";
			    return $"{Language}";
			}
		}

		/// <summary>
		/// Get BCP47 language tag for this language
		/// See also http://en.wikipedia.org/wiki/IETF_language_tag
		/// </summary>
		public string DisplayName => $"{Name} {BCP47}";
	}
}
