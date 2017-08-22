namespace MRULib.MRU.Interfaces
{
    using System;
    using MRULib.MRU.Models;
    using System.ComponentModel;
    using MRULib.MRU.ViewModels;

    /// <summary>
    /// Defines an interface to a viewmodel entry that can be
    /// used to list all recently used files.
    /// </summary>
    public interface IMRUEntryViewModel : INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets the pathmodel stored in this viewmodel.
        /// </summary>
        PathModel File { get; }

        /// <summary>
        /// Gets the path and file name of the referenced file.
        /// </summary>
        string PathFileName { get; }

        /// <summary>
        /// Gets the path and file name of the referenced file in a Uri
        /// object using the file:/// Uri syntax convention.
        /// </summary>
        string PathfileNameUri { get; }

        /// <summary>
        /// Gets the fact whether this item is currently pinned in the list or not.
        /// </summary>
        int IsPinned { get; }

        /// <summary>
        /// Gets the path and file name of the referenced file.
        /// </summary>
        DateTime LastUpdate { get; }

        /// <summary>
        /// Gets the Group (today, yesterday, etc.) for the current LastUpdate
        /// with reference to <see cref="DateTime.Now"/> or IsPinned if IsPinned is true.
        /// </summary>
        GroupViewModel GroupItem { get; }

        /// <summary>
        /// Gets a shortend path (with ellipses if necessary) for output in UI components.
        /// </summary>
        string DisplayPathFileName { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method updates the <see cref="GroupItem"/> property with regard to the
        /// gap between the current system time and the time stored in
        /// <see cref="LastUpdate"/> property.
        /// </summary>
        void UpdateGroup();

        /// <summary>
        /// Sets the <see cref="IsPinned"/> property to true or false.
        /// </summary>
        /// <param name="isPinned"></param>
        void SetIsPinned(int isPinned);

        /// <summary>
        /// Sets the <see cref="LastUpdate"/> property to the specified value.
        /// </summary>
        /// <param name="lastUpdate"></param>
        void SetLastUpdate(DateTime lastUpdate);
        #endregion methods
    }
}