namespace FileSystemModels.Events
{
    using System;

    /// <summary>
    /// Class implements ...
    /// </summary>
    public class FilterChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Path of directory...
        /// </summary>
        public string FilterText { get; set; }
    }
}
