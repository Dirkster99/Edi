using System;

namespace Edi.Core.Interfaces
{
	public interface ILayoutableWindow
	{
		/// <summary>
		/// Standard Closed Window event.
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Gets the current AvalonDockManager Xml layout and returns it as a string.
		/// </summary>
		string CurrentAdLayout { get; }
	}
}