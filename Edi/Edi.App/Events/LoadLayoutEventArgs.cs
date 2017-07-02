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
			this.XmlLayout = xmlLayout;
			this.LayoutID = layoutID;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public LoadLayoutEventArgs()
		{
			this.XmlLayout = string.Empty;
			this.LayoutID = Guid.Empty;
		}
		#endregion constructor

		#region properties
		public string XmlLayout { get; set; }

		public Guid LayoutID { get; private set; }
		#endregion properties
	}
}
