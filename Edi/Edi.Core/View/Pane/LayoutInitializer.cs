namespace Edi.Core.View.Pane
{
    using System;
    using System.Windows;
    using Edi.Core.Interfaces.Enums;
    using Edi.Core.ViewModels;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// Initialize the AvalonDock Layout. Methods in this class
    /// are called before and after the layout is changed.
    /// </summary>
    public class LayoutInitializer : ILayoutUpdateStrategy
	{
		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Method is called when a completely new layout item is
		/// to be inserted into the current avalondock layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="anchorableToShow"></param>
		/// <param name="destinationContainer"></param>
		/// <returns></returns>
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
            if (anchorableToShow.Content is IToolWindow)
            {
                IToolWindow tool = anchorableToShow.Content as IToolWindow;

                var preferredLocation = tool.PreferredLocation;

                LayoutAnchorGroup layoutGroup = null;

                switch (preferredLocation)
                {
                    case PaneLocation.Left:
                        layoutGroup = FindAnchorableGroup(layout, preferredLocation);
                        break;

                    case PaneLocation.Right:
                        layoutGroup = FindAnchorableGroup(layout, preferredLocation);
                        break;

                    case PaneLocation.Bottom:
                        layoutGroup = FindAnchorableGroup(layout, preferredLocation);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (layoutGroup != null)
                {
                    ////group.InsertChildAt(0, anchorableToShow);
                    layoutGroup.Children.Add(anchorableToShow);
                }

                return true;
            }

            return false;
		}

		private static LayoutAnchorGroup FindAnchorableGroup(LayoutRoot layout,
		                                                     PaneLocation location)
		{
			try
			{
				LayoutAnchorSide panelGroupParent = null;

				switch (location)
				{
					case PaneLocation.Left:
						panelGroupParent = layout.LeftSide;
					break;

					case PaneLocation.Right:
						panelGroupParent = layout.RightSide;
					break;

					case PaneLocation.Bottom:
						panelGroupParent = layout.BottomSide;
					break;

					default:
						throw new ArgumentOutOfRangeException("location:" + location);
				}

				if (panelGroupParent.Children.Count == 0)
				{
					var layoutAnchorGroup = new LayoutAnchorGroup();

					panelGroupParent.Children.Add(layoutAnchorGroup);

					return layoutAnchorGroup;
				}
				else
				{
					return panelGroupParent.Children[0];
				}

			}
			catch (Exception exp)
			{
				logger.Error(exp);
			}

			return null;
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
            // If this is the first anchorable added to this pane, then use the preferred size.
            if (anchorableShown.Content is IToolWindow)
            {
                IToolWindow tool = anchorableShown.Content as IToolWindow;
                if (anchorableShown.Parent is LayoutAnchorablePane)
                {
                    LayoutAnchorablePane anchorablePane = anchorableShown.Parent as LayoutAnchorablePane;

                    if (anchorablePane.ChildrenCount == 1)
                    {
                        switch (tool.PreferredLocation)
                        {
                            case PaneLocation.Left:
                            case PaneLocation.Right:
                                anchorablePane.DockWidth = new GridLength(tool.PreferredWidth, GridUnitType.Pixel);
                                break;
                            case PaneLocation.Bottom:
                                anchorablePane.DockHeight = new GridLength(tool.PreferredHeight, GridUnitType.Pixel);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
		}
	}
}
