namespace Edi.Core.Models
{
    using Edi.Core.ViewModels;

    /// <summary>
    /// Class implements 
    /// </summary>
    public class RegisterToolWindowEventArgs
	{
		#region constructor
		/// <summary>
		/// Class constructor from default parameters.
		/// </summary>
		/// <param name="tool"></param>
		public RegisterToolWindowEventArgs(ToolViewModel tool)
			: this()
		{
			Tool = tool;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public RegisterToolWindowEventArgs()
		{
			Tool = null;
		}
		#endregion constructor

		#region properties
        /// <summary>
        /// Gets the 
        /// </summary>
		public ToolViewModel Tool { get; }
		#endregion properties
	}
}