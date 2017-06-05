namespace Edi.Core.View.Pane
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Controls;
	using System.ComponentModel.Composition;

	/// <summary>
	/// Select a tool window style for an instance of its view.
	/// </summary>
	public class PanesStyleSelector : StyleSelector
	{
		#region fields
		private Dictionary<Type, Style> mStyleDirectory = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public PanesStyleSelector()
		{
		}
		#endregion constructor

		#region methods
		/// <summary>
		/// Returns a System.Windows.Style based on custom logic.
		/// </summary>
		/// <param name="item">The content.</param>
		/// <param name="container">The element to which the style will be applied.</param>
		/// <returns>Returns an application-specific style to apply; otherwise, null.</returns>
		public override System.Windows.Style SelectStyle(object item,
																										 System.Windows.DependencyObject container)
		{
			if (this.mStyleDirectory == null)
				return null;

			if (item == null)
				return null;

			Style o;
			Type t = item.GetType();
			this.mStyleDirectory.TryGetValue(t, out o);

			if (o != null)
				return o;

			// Get next base of the current type in inheritance tree
			Type t1 = item.GetType().BaseType;

			// Traverse backwards in the inheritance chain to find a mapping there
			while (t1 != t && t != null)
			{
				t = t1;
				this.mStyleDirectory.TryGetValue(t, out o);

				if (o != null)
					return o;

				t1 = item.GetType().BaseType;
			}

			return base.SelectStyle(item, container);
		}

		/// <summary>
		/// Register a (viewmodel) class type with a <seealso cref="Style"/> for a view.
		/// </summary>
		/// <param name="typeOfViewmodel"></param>
		/// <param name="styleOfView"></param>
		public void RegisterStyle(Type typeOfViewmodel, Style styleOfView)
		{
			if (this.mStyleDirectory == null)
				this.mStyleDirectory = new Dictionary<Type, Style>();

			this.mStyleDirectory.Add(typeOfViewmodel, styleOfView);
		}
		#endregion methods
	}
}
