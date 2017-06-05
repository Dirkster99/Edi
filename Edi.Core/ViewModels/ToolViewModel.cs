namespace Edi.Core.ViewModels
{
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.ViewModels;

	/// <summary>
	/// AvalonDock base class viewmmodel to support tool window views.
	/// </summary>
	public abstract class ToolViewModel : PaneViewModel, IToolWindow
	{
		#region fields
		private bool mIsVisible = true;
		private bool mCanHide = true;
		#endregion fields

		#region constructors
		/// <summary>
		/// Base constructor from nam of tool window item
		/// </summary>
		/// <param name="name">Name of tool window displayed in GUI</param>
		public ToolViewModel(string name)
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
			private set;
		}

		/// <summary>
		/// Gets/sets property to determine whether this item is visible or not.
		/// </summary>
		public bool IsVisible
		{
			get
			{
				return this.mIsVisible;
			}

			set
			{
				if (this.mIsVisible != value)
				{
					this.mIsVisible = value;

					if (value == true)         // Switching visibility on should switch hide mode off
						this.mCanHide = false;

					RaisePropertyChanged(() => this.IsVisible);
				}
			}
		}

		/// <summary>
		/// Gets/sets whether this item can hide or not.
		/// </summary>
		public bool CanHide
		{
			get
			{
				return this.mCanHide;
			}

			set
			{
				if (this.mCanHide != value)
				{
					this.mCanHide = value;
					RaisePropertyChanged(() => this.CanHide);
				}
			}
		}

		public abstract PaneLocation PreferredLocation { get; }

		public virtual double PreferredWidth
		{
			get { return 200; }
		}

		public virtual double PreferredHeight
		{
			get { return 200; }
		}
	
		#endregion properties

		#region methods
		/// <summary>
		/// Ensures the visibility of this toolwindow.
		/// </summary>
		/// <param name="isVisible"></param>
		public virtual void SetToolWindowVisibility(bool isVisible = true)
		{
			this.IsVisible = isVisible;
		}
		#endregion methods
	}
}
