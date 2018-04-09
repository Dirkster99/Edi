namespace Edi.Core.ViewModels
{
    using Edi.Core.Interfaces.Enums;

    /// <summary>
    /// AvalonDock base class viewmmodel to support tool window views.
    /// </summary>
    public abstract class ToolViewModel : PaneViewModel, IToolWindow
	{
		#region fields
		private bool _mIsVisible = true;
		private bool _mCanHide = true;
		#endregion fields

		#region constructors
		/// <summary>
		/// Base constructor from nam of tool window item
		/// </summary>
		/// <param name="name">Name of tool window displayed in GUI</param>
		protected ToolViewModel(string name)
		{
			Name = name;
			Title = name;
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets a displayable name of this item.
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// Gets/sets property to determine whether this item is visible or not.
		/// </summary>
		public bool IsVisible
		{
			get => _mIsVisible;

			set
			{
				if (_mIsVisible != value)
				{
					_mIsVisible = value;

					if (value)         // Switching visibility on should switch hide mode off
						_mCanHide = false;

					RaisePropertyChanged(() => IsVisible);
				}
			}
		}

		/// <summary>
		/// Gets/sets whether this item can hide or not.
		/// </summary>
		public bool CanHide
		{
			get => _mCanHide;

			set
			{
				if (_mCanHide != value)
				{
					_mCanHide = value;
					RaisePropertyChanged(() => CanHide);
				}
			}
		}

		public abstract PaneLocation PreferredLocation { get; }

		public virtual double PreferredWidth => 300;

		public virtual double PreferredHeight => 300;

		#endregion properties

		#region methods
		/// <summary>
		/// Ensures the visibility of this toolwindow.
		/// </summary>
		/// <param name="isVisible"></param>
		public virtual void SetToolWindowVisibility(bool isVisible = true)
		{
			IsVisible = isVisible;
		}
		#endregion methods
	}
}
