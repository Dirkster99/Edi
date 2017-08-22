namespace MRULib.Controls
{
    using System.Globalization;

    internal static class Extensions
    {
        /// <summary>
        /// Extend the string constructor with a string.Format like syntax.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string s, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, s, args);
        }
    }
}
