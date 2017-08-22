namespace MRULib
{
    using MRULib.MRU.Interfaces;
    using System;

    /// <summary>
    /// Provides methodes for generating MRU List View items via defined interfaces
    /// rather than using classes known ourside of this library.
    /// </summary>
    public class MRU_Service
    {
        /// <summary>
        /// Implements a method for creating a listviewmodel that drives the backend of the MRU List.
        /// </summary>
        /// <returns></returns>
        public static IMRUListViewModel Create_List()
        {
            return new MRULib.MRU.ViewModels.MRUListViewModel();
        }

        /// <summary>
        /// Provides a parameterized standard construction of entry viewmodel item.
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="isPinned"></param>
        public static IMRUEntryViewModel Create_Entry(string pathFileName
                                                     , bool isPinned = false)
        {
            var intIsPinned = (isPinned == false ? 0 : 1);

            return new MRULib.MRU.ViewModels.MRUEntryViewModel(pathFileName, intIsPinned);
        }

        /// <summary>
        /// Provides a parameterized standard construction of entry viewmodel item.
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="isPinned"></param>
        /// <param name="lastUpdate"></param>
        public static IMRUEntryViewModel Create_Entry(string pathFileName
                                                    , DateTime lastUpdate
                                                    , bool isPinned = false)
        {
            var intIsPinned = (isPinned == false ? 0 : 1);

            return new MRULib.MRU.ViewModels.MRUEntryViewModel(pathFileName, lastUpdate, intIsPinned);
        }
    }
}
