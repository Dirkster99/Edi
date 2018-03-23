using System;

namespace Edi.Core.Interfaces
{
	/// <summary>
	/// Interface that defines common properties
	/// of toolwindows and document items for avlaondock content.
	/// 
	/// Based on Gemini:
	/// https://github.com/tgjones/gemini/blob/master/src/Gemini/Framework/ILayoutItem.cs
	/// </summary>
	public interface ILayoutItem
	{
		////Guid Id { get; }
		string ContentId { get; }
		string Title { get; }

		Uri IconSource { get; }
		bool IsSelected { get; set; }
		
		////bool ShouldReopenOnStart { get; }
		////void LoadState(BinaryReader reader);
		////void SaveState(BinaryWriter writer);
	}
}