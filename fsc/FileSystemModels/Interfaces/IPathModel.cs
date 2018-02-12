namespace FileSystemModels.Interfaces
{
    using FileSystemModels.Models.FSItems.Base;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface defines properties and methods of elements related to a path
    /// in the file system. Such elements are, virtual folders, drives, network
    /// drives, folder, files, and shortcuts.
    /// </summary>
    public interface IPathModel : IEqualityComparer,
                                  IEqualityComparer<IPathModel>, ICloneable,
                                  IComparable,
                                  IComparable<IPathModel>,
                                  IEquatable<IPathModel>
                                  
    {
        #region properties
        /// <summary>
        /// Gets the path of this <seealso cref="PathModel"/> object.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the type of item of this <seealso cref="PathModel"/> object.
        /// </summary>
        FSItemType PathType { get; }

        /// <summary>
        /// Gets the name of this item. For folders this is the folder
        /// name without its path;
        /// </summary>
        string Name { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Determine whether a given path is an existing directory or not.
        /// </summary>
        /// <returns>true if this directory exists and otherwise false</returns>
        bool DirectoryPathExists();

        /// <summary>
        /// Async version to determine whether a given path is an existing directory or not.
        /// </summary>
        /// <returns>true if this directory exists and otherwise false</returns>
        Task<bool> DirectoryPathExistsAsync();
        #endregion methods
    }
}
