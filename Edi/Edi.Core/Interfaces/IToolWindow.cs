namespace Edi.Core.ViewModels
{
	using Interfaces;
	using Interfaces.Enums;

	public interface IToolWindow : ILayoutItem
	{
		#region properties
		string Name	{ get; }

		bool IsVisible { get; }

		bool IsActive { get; }

		bool CanHide { get; }

		PaneLocation PreferredLocation { get; }
		double PreferredWidth { get; }
		double PreferredHeight { get; }
		#endregion properties

		#region methods
		void SetToolWindowVisibility(bool isVisible = true);
		#endregion methods
	}
}
