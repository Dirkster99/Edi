using System;

namespace Edi.Apps.Events
{
	/// <summary>
	/// Class implements ...
	/// </summary>
	public class LoadLayoutEventArgs
	{
		#region constructor
		/// <summary>
		/// Class constructor from default parameters.
		/// </summary>
		/// <param name="xmlLayout"></param>
		/// <param name="layoutId"></param>
		public LoadLayoutEventArgs(string xmlLayout, Guid layoutId)
			: this()
		{
			XmlLayout = xmlLayout;
			LayoutId = layoutId;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public LoadLayoutEventArgs()
		{
			XmlLayout = string.Empty;
			LayoutId = Guid.Empty;
		}
		#endregion constructor

		#region properties
		public string XmlLayout { get; set; }

		public Guid LayoutId { get; private set; }
		#endregion properties
	}
}
