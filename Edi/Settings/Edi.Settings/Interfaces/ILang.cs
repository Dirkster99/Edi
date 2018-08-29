namespace Edi.Settings.Interfaces
{
    /// <summary>
    /// Interface to define an API over languages (and their locale) that
    /// are supported with specific (non-English) strings.
    /// 
    /// The class definition is based on BCP 47 which in turn is used to
    /// set the UI and thread culture (which in turn selects the correct
    /// string *.resx resource in each referenced assembly).
    /// </summary>
    public interface ILang
    {
        /// <summary>
        /// Get BCP47 language tag for this language
        /// See also http://en.wikipedia.org/wiki/IETF_language_tag
        /// </summary>
        string BCP47 { get; }

        /// <summary>
        /// Get BCP47 language tag for this language
        /// See also http://en.wikipedia.org/wiki/IETF_language_tag
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the language code of a language eg.: EN for English or DE for German
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets the local for a language code
        /// A local for EN is for example US
        /// </summary>
        string Locale { get; set; }

        /// <summary>
        /// Gets a humanreadable string that describes a language - local pair
        /// in a displayable localized way eg.: 'Deutsch (German)'
        /// </summary>
        string Name { get; set; }
    }
}