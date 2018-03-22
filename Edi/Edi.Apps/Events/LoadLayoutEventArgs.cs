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
		/// <param name="layoutId"></param>
		public LoadLayoutEventArgs(string xmlLayout, Guid layoutId)
			: this()
		{
			this.XmlLayout = xmlLayout;
			this.LayoutId = layoutId;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public LoadLayoutEventArgs()
		{
			this.XmlLayout = string.Empty;
			this.LayoutId = Guid.Empty;
		}
		#endregion constructor

		#region properties
		public string XmlLayout { get; set; }

		public Guid LayoutId { get; private set; }
		#endregion properties
	}
}
