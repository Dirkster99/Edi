namespace Edi.Core.Models.Enums
{
	/// <summary>
	/// Determine the source of the AvalonDock layout.
	/// </summary>
	public enum LayoutLoaded
	{
		/// <summary>
		/// AvalonDockLayout was never loadd from last session. Therefore, default is applied.
		/// </summary>
		FromDefault,

		/// <summary>
		/// AvalonDockLayout was loadd from last session and is applied.
		/// </summary>
		FromStorage
	}
}
