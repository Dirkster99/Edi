namespace Edi.Settings.ProgramSettings
{
    using Edi.Settings.Interfaces;
    using System;

	/// <summary>
	/// Base class for enumeration over languages (and their locale) that
	/// are supported with specific (non-English) button and tool tip strings.
	/// 
	/// The class definition is based on BCP 47 which in turn is used to
	/// set the UI and thread culture (which in turn selects the correct
	/// string resource in each referenced assembly).
	/// </summary>
	internal class Lang : ILang
    {
        #region constructors
        /// <summary>
        /// Class constructor from required properties.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="local"></param>
        /// <param name="name"></param>
        public Lang(string language,
                                  string local,
                                  string name)
            : this()
        {
            this.Language = language;
            this.Locale = local;
            this.Name = name;
        }

        /// <summary>
        /// Hidden standard class constructor.
        /// </summary>
        protected Lang()
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Get BCP47 language tag for this language
        /// See also http://en.wikipedia.org/wiki/IETF_language_tag
        /// </summary>
        public string BCP47
        {
            get
            {
                if (string.IsNullOrEmpty(this.Locale) == false)
                    return String.Format("{0}-{1}", this.Language, this.Locale);
                else
                    return String.Format("{0}", this.Language);
            }
        }

        /// <summary>
        /// Get BCP47 language tag for this language
        /// See also http://en.wikipedia.org/wiki/IETF_language_tag
        /// </summary>
        public string DisplayName
        {
            get
            {
                return String.Format("{0} {1}", this.Name, this.BCP47);
            }
        }

        /// <summary>
        /// Gets the language code of a language eg.: EN for English or DE for German
        /// </summary>
		public string Language { get; set; }

        /// <summary>
        /// Gets the local for a language code
        /// A local for EN is for example US
        /// </summary>
		public string Locale { get; set; }

        /// <summary>
        /// Gets a humanreadable string that describes a language - local pair
        /// in a displayable localized way eg.: 'Deutsch (German)'
        /// </summary>
		public string Name { get; set; }
        #endregion properties
    }
}
