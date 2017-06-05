namespace EdiApp.Enums
{
	/// <summary>
	/// Determine whether an error on load file operation
	/// is silent (no user notification) or not.
	/// </summary>
	public enum CloseDocOnError
	{
		/// <summary>
		/// Close documents automatically without message (when re-loading on startup)
		/// </summary>
		WithoutUserNotification,

		/// <summary>
		/// Close documents on error with message (when doin' interactive File>Open)
		/// </summary>
		WithUserNotification
	}
}
