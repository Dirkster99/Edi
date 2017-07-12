namespace Edi.Dialogs.FindReplace.ViewModel
{
	/// <summary>
	/// This interface must be supported by a viewmodel in order to find/replace text
	/// in each document associated with that viewmodel. The viewModel may delegate
	/// work further to a view via another interface...
	/// 
	/// http://www.codeproject.com/Articles/173509/A-Universal-WPF-Find-Replace-Dialog
	/// </summary>
	public interface IEditor
	{
		string Text { get; }
		int SelectionStart { get; }
		int SelectionLength { get; }

		/// <summary>
		/// Selects the specified portion of Text and scrolls that part into view.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		void Select(int start, int length);
		void Replace(int start, int length, string ReplaceWith);

		/// <summary>
		/// This method is called before a replace all operation.
		/// </summary>
		void BeginChange();

		/// <summary>
		/// This method is called after a replace all operation.
		/// </summary>
		void EndChange();
	}
}
