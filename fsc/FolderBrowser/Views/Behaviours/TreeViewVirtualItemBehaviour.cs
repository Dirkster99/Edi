namespace FolderBrowser.Views.Behaviours
{
    using FolderBrowser.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Class implements an attached behaviour to bring a selected TreeViewItem in VIRTUALIZED TreeView
    /// into view when selection is driven by the viewmodel (not the user). The System.Windows.Interactivity
    /// library is required for this behavior to compile.
    /// 
    /// Sample Usage:
    /// &lt;i:Interaction.Behaviors>
    ///     &lt;behav:BringVirtualTreeViewItemIntoViewBehavior SelectedItem = "{Binding SelectPathItem}" />
    /// &lt;/ i:Interaction.Behaviors>
    /// 
    /// This behaviour requires a binding to a path like structure of tree view (viewmodel) items.
    /// This implementation requieres an array of objects (object[] SelectedItem) that represents
    /// each tree view item along the path that should be browsed with this behaviour.
    /// 
    /// The <see cref="OnSelectedItemChanged"/> method executes when the bound property has changed.
    /// The behavior browses then along the given path and ensures that all requested items exist
    /// even if we are using a virtual tree.
    /// 
    /// Allows two-way binding of a TreeView's selected item.
    /// Sources:
    /// http://stackoverflow.com/q/183636/46635
    /// http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
    /// </summary>
    internal class TreeViewVirtualItemBehaviour
    {
        public static IParent GetSelectedItem(DependencyObject obj)
        {
            return (IParent)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, IParent value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }


        private static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached("SelectedItem"
                , typeof(IParent)
                , typeof(TreeViewVirtualItemBehaviour)
                , new FrameworkPropertyMetadata(null,
                    //FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        /// <summary>
        /// This method is invoked when the value bound at the dependency
        /// property <see cref="SelectedItem"/> has changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectedItemChanged(DependencyObject d
                                                , DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var rootControl = d as ItemsControl;
                var currentParent = d as ItemsControl;

                // Sanity check: Are we looking at the least required data we need?
                var itemNode = e.NewValue as IParent;
                if (itemNode == null)
                    return;

                Stack<object> Nodes = new Stack<object>();     // Unwind IParent Stack

                for (; itemNode != null; itemNode = itemNode.Parent)
                    Nodes.Push(itemNode);

                var newNode = Nodes.ToArray();

                if (newNode.Length < 1)             // Traverse path in a forward fashion
                    return;

                // params look good so lets find the attached tree view (aka ItemsControl)
                //var behavior = d as BringVirtualTreeViewItemIntoViewBehavior;
                //var tree = behavior.AssociatedObject;

                // Now loop through each item in the array of bound path items and make sure they exist
                for (int i = 0; i < newNode.Length; i++)
                {
                    var node = newNode[i];

                    // first try the easy way
                    var newParentObject = currentParent.ItemContainerGenerator.ContainerFromItem(node);
                    var newParent = newParentObject as TreeViewItem;

                    if (newParent == null)
                    {
                        // if this failed, it's probably because of virtualization, and we will have to do it the hard way.
                        // this code is influenced by TreeViewItem.ExpandRecursive decompiled code, and the MSDN sample at
                        // http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
                        // see also the question at http://stackoverflow.com/q/183636/46635
                        currentParent.ApplyTemplate();
                        var itemsPresenter = (ItemsPresenter)currentParent.Template.FindName("ItemsHost", currentParent);
                        if (itemsPresenter != null)
                            itemsPresenter.ApplyTemplate();
                        else
                            currentParent.UpdateLayout();

                        var virtualizingPanel = GetItemsHost(currentParent) as VirtualizingPanel;

                        // ...but virtualization does not work with invisible controls ...
                        // For example: Hidden behind another tab control etc...
                        if (rootControl.IsVisible == false)
                            return;

                        CallEnsureGenerator(virtualizingPanel);

                        var index = currentParent.Items.IndexOf(node);
                        if (index < 0)
                        {
                            // This is raised when the item in the path array is not part of the tree collection
                            // This can be tricky, because Binding an ObservableDictionary to the treeview will
                            // require that we need an array of KeyValuePairs<K,T>[] here :-(
#if DEBUG
                            System.Console.WriteLine("1> Node '" + node + "' cannot be fount in container");
                            return;
                            ////                    throw new InvalidOperationException("Node '" + node + "' cannot be fount in container");
#else
                        // Use your favourite logger here since the exception will otherwise kill the appliaction
                        System.Console.WriteLine("Node '" + node + "' cannot be fount in container");
#endif
                        }

                        if (index >= 0) // Bugfix: Include index == 0 here
                        {
                            virtualizingPanel.BringIndexIntoViewPublic(index);
                            var obj = currentParent.ItemContainerGenerator.ContainerFromIndex(index);
                            newParent = obj as TreeViewItem;

                            if (newParent == null)
                            {
                                System.Console.WriteLine("3> Node '" + node + "' cannot be found in container");
                                return;
                            }
                        }
                    }

                    if (newParent == null)
                    {
#if DEBUG
                        System.Console.WriteLine("2> Node '" + node + "' cannot be found in container");
                        return;
                        ////               throw new InvalidOperationException("Tree view item cannot be found or created for node '" + node + "'");
#else
                    // Use your favourite logger here since the exception will otherwise kill the appliaction
                    System.Console.WriteLine("2> Node '" + node + "' cannot be found in container");
#endif
                    }

                    if (node == newNode[newNode.Length - 1])
                    {
                        newParent.IsSelected = true;
                        newParent.BringIntoView();
                        break;
                    }

                    // Make sure nodes (except for last child node) are expanded to make children visible
                    if (i < newNode.Length - 1 && newParent != null)
                        newParent.IsExpanded = true;

                    currentParent = newParent;
                }
            }
            catch (Exception exp)
            {
                System.Console.WriteLine("Exception in TreeViewVirtualItemBehaviour.OnSelectedItemChanged:");
                System.Console.WriteLine(exp.Message);
                System.Console.WriteLine(exp.StackTrace);
            }
        }

        #region Functions to get internal members using reflection
        // Some functionality we need is hidden in internal members, so we use reflection to get them
        #region ItemsControl.ItemsHost

        static readonly PropertyInfo ItemsHostPropertyInfo = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Panel GetItemsHost(ItemsControl itemsControl)
        {
            Debug.Assert(itemsControl != null);
            return ItemsHostPropertyInfo.GetValue(itemsControl, null) as Panel;
        }

        #endregion ItemsControl.ItemsHost

        #region Panel.EnsureGenerator
        private static readonly MethodInfo EnsureGeneratorMethodInfo = typeof(Panel).GetMethod("EnsureGenerator", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void CallEnsureGenerator(Panel panel)
        {
            // this can actually happen if the treeview is currently not visible
            // and initialized on demand ... (eg.: in TabControl)
            Debug.Assert(panel != null);
            EnsureGeneratorMethodInfo.Invoke(panel, null);
        }
        #endregion Panel.EnsureGenerator
        #endregion Functions to get internal members using reflection
    }
}
