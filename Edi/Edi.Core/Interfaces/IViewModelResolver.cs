namespace Edi.Core.Interfaces
{
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
		/// <param name="contentId"></param>
		/// <returns>viewmodel for a content_id or null</returns>
		object ContentViewModelFromId(string contentId);
	}
}
