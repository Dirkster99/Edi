namespace Edi.Apps.Events
{
	using System;

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
		/// <param name="layoutID"></param>
		public LoadLayoutEventArgs(string xmlLayout, Guid layoutID)
			: this()
		{
			XmlLayout = xmlLayout;
			LayoutID = layoutID;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public LoadLayoutEventArgs()
		{
			XmlLayout = string.Empty;
			LayoutID = Guid.Empty;
		}
		#endregion constructor

		#region properties
		public string XmlLayout { get; set; }

		public Guid LayoutID { get; private set; }
		#endregion properties
	}
}
