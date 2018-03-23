using System.Windows;
using System.Windows.Media;

namespace Edi.Core.Utillities
{
	/// <summary>
	/// Helper class to find a child item of a given item in the Visual Tree of WPF 
	/// </summary>
	public static class VisualTreeHelperExtension
	{
		/// <summary>
		/// Looks for a child control within a parent by name
		/// </summary>
		public static DependencyObject FindChild(this DependencyObject parent, string name)
		{
			// confirm parent and name are valid.
			if (parent == null || string.IsNullOrEmpty(name))
				return null;

			FrameworkElement frameworkElement = parent as FrameworkElement;
			if (frameworkElement != null && frameworkElement.Name == name)
				return parent;

			DependencyObject result = null;

			if (frameworkElement != null)
				frameworkElement.ApplyTemplate();

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				result = FindChild(child, name);
				if (result != null)
					break;
			}

			return result;
		}

		/// <summary>
		/// Looks for a child control within a parent by type
		/// </summary>
		public static T FindChild<T>(this DependencyObject parent)
				where T : DependencyObject
		{
			// confirm parent is valid.
			if (parent == null)
				return null;
			if (parent is T)
				return parent as T;

			DependencyObject foundChild = null;

            if (parent is FrameworkElement)
            {
                FrameworkElement frameworkElement = parent as FrameworkElement;
                frameworkElement.ApplyTemplate();
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				foundChild = FindChild<T>(child);
				if (foundChild != null)
					break;
			}

			return foundChild as T;
		}
	}
}
