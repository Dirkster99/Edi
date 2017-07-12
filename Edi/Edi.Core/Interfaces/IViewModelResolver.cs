namespace Edi.Core.Interfaces
{
	using System;
	using Edi.Core.ViewModels.Base;
	using Microsoft.Practices.Prism.Mvvm;
	using Microsoft.Practices.Prism.ViewModel;

	/// <summary>
	/// Interface to resolve string id into a
	/// matching viewmodel that represents a tool window or document.
	/// </summary>
	public interface IViewModelResolver
	{
		//Guid LayoutID { get; }

		/// <summary>
		/// Get a matching viewmodel for a view through its content_id.
		/// </summary>
		/// <param name="content_id"></param>
		/// <returns>viewmodel for a content_id or null</returns>
		object ContentViewModelFromID(string content_id);
	}
}
