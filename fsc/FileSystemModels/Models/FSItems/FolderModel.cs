namespace FileSystemModels.Models.FSItems
{
    using FileSystemModels.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;

    public class FolderModel : Base.FileSystemModel
    {
        #region constructors
        /// <summary>
        /// Parameterized class  constructor
        /// </summary>
        /// <param name="model"></param>
        [SecuritySafeCritical]
        public FolderModel(IPathModel model)
          : base(model)
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets whether this folder really exists or not.
        /// </summary>
        public bool Exists
        {
            get
            {
                var dir = GetDirInfo();
                return (dir != null ? dir.Exists : false);
            }
        }

        /// <summary>
        /// Gets the parent directory of this folder.
        /// </summary>
        public DirectoryInfo Parent
        {
            get
            {
                var dir = GetDirInfo();
                return (dir != null ? dir.Parent : null);
            }
        }

        /// <summary>
        /// Gets the root entry of this folder.
        /// </summary>
        public DirectoryInfo Root
        {
            get
            {
                var dir = GetDirInfo();
                return (dir != null ? dir.Root : null);
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Fills the CurrentItems property for display in ItemsControl
        /// based view (ListBox, ListView etc.)
        /// 
        /// This version is parameterized since the filterstring can be parsed
        /// seperately and does not need to b parsed each time when this method
        /// executes.
        /// </summary>
        /*
        internal static Response GetSubFolderItems(string[] filterString,
                                                    bool IsFiltered,
                                                    PathModel folder,
                                                    bool showfolders,
                                                    bool showHidden,
                                                    bool showFiles)
                {
                    List<Base.FileSystemModel> ret = new List<Base.FileSystemModel>();
                    

                    if (folder == null)
                        return new Response("internal error");

                    if (folder.DirectoryPathExists() == false)
                        return new Response(string.Format("'{0}' is not accessible.", folder.Path ));

            try
            {
                        DirectoryInfo cur = new DirectoryInfo(folder.Path);

                        // Retrieve and add (filtered) list of directories
                        if (showfolders == true)
                        {
                            string[] directoryFilter = null;

                            //// if (filterString != null)
                            ////  directoryFilter = new ArrayList(filterString).ToArray() as string[];
                            directoryFilter = null;

                            foreach (DirectoryInfo dir in SelectDirectoriesByFilter(cur, directoryFilter))
                            {
                                if (dir.Attributes.HasFlag(FileAttributes.Hidden) == true)
                                {
                                    if (showHidden == false)
                                    {
                                        if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                            continue;
                                    }
                                }

                                FolderModel info = new FolderModel(new PathModel(dir.FullName, FSItemType.Folder));

                                ret.Add(info);
                            }
                        }

                        if (showFiles == true)
                        {
                            if (IsFiltered == false) // Do not apply the filter if it is not enabled
                                filterString = null;

                            // Retrieve and add (filtered) list of files in current directory
                            foreach (FileInfo f in SelectFilesByFilter(cur, filterString))
                            {
                                if (showHidden == false)
                                {
                                    if (f.Attributes.HasFlag(FileAttributes.Hidden) == true)
                                    {
                                        if ((f.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                            continue;
                                    }
                                }

                                FileModel info = new FileModel(new PathModel(f.FullName, FSItemType.File));

                                ret.Add(info);
                            }
                        }
                    }
                    catch
                    {
                        return new Response("internal error");
                    }

                    return new Response(ResponseType.Data, ret);
                }
*/
        /// <summary>
        /// Method implements an extension that lets us filter files
        /// with multiple filter arguments.
        /// </summary>
        /// <param name="dir">Points at the folder that is queried for files and folder entries.</param>
        /// <param name="extensions">Contains the extension that we want to filter for, eg: string[]{"*.*"} or string[]{"*.tex", "*.txt"}</param>
        public static IEnumerable<FileInfo> SelectFilesByFilter(DirectoryInfo dir,
                                                                params string[] extensions)
        {
            if (dir.Exists == false)
                yield break;

            IEnumerable<FileSystemInfo> matches = new List<FileSystemInfo>();
            if (extensions == null)
            {
                try
                {
                    matches = dir.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
                }
                catch
                {
                    yield break;
                }

                foreach (var file in matches)
                {
                    if (file as FileInfo != null)
                        yield return file as FileInfo;
                }

                yield break;
            }

            List<string> patterns = new List<string>(extensions);
            try
            {
                foreach (var pattern in patterns)
                {
                    matches = matches.Concat(dir.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly));
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Unable to access '{0}'. Skipping...", dir.FullName);
                yield break;
            }
            catch (PathTooLongException ptle)
            {
                Console.WriteLine(@"Could not process path '{0}\{1} ({2})'.", dir.Parent.FullName, dir.Name, ptle.Message);
                yield break;
            }

            ////      try
            ////      {
            ////        foreach (var pattern in patterns)
            ////        {
            ////          matches = matches.Concat(dir.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly));
            ////        }
            ////      }
            ////      catch (UnauthorizedAccessException)
            ////      {
            ////        Console.WriteLine("Unable to access '{0}'. Skipping...", dir.FullName);
            ////        yield break;
            ////      }
            ////      catch (PathTooLongException ptle)
            ////      {
            ////        Console.WriteLine(@"Could not process path '{0}\{1} ({2})'.", dir.Parent.FullName, dir.Name, ptle.Message);
            ////        yield break;
            ////      }
            ////
            ////      Console.WriteLine("Returning all objects that match the pattern(s) '{0}'", string.Join(",", patterns));

            foreach (var file in matches)
            {
                if (file as FileInfo != null)
                    yield return file as FileInfo;
            }
        }

        /// <summary>
        /// Method implements an extension that lets us filter (sub-)directory entries
        /// with multiple filter aruments.
        /// </summary>
        /// <param name="dir">Points at the folder that is queried for sub-directory entries.</param>
        /// <param name="extensions">Contains the extension that we want to filter for, eg: string[]{"*.*"} or string[]{"*.tex", "*.txt"}</param>
        public static IEnumerable<DirectoryInfo> SelectDirectoriesByFilter(DirectoryInfo dir,
                                                                           params string[] extensions)
        {
            if (dir.Exists == false)
                yield break;

            // Enumerate directories without filter if filters are not supplied
            IEnumerable<DirectoryInfo> matches = new List<DirectoryInfo>();
            if (extensions == null)
            {
                try
                {
                    matches = dir.EnumerateDirectories();
                }
                catch
                {
                    yield break;
                }

                foreach (var item in matches)
                    yield return item;

                yield break;
            }

            List<string> patterns = new List<string>(extensions);

            try
            {
                foreach (var pattern in patterns)
                {
                    matches = matches.Concat(dir.EnumerateDirectories(pattern, SearchOption.TopDirectoryOnly));
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Unable to access '{0}'. Skipping...", dir.FullName);
                yield break;
            }
            catch (PathTooLongException ptle)
            {
                Console.WriteLine(@"Could not process path '{0}\{1} ({2})'.", dir.Parent.FullName, dir.Name, ptle.Message);
                yield break;
            }

            ////Console.WriteLine("Returning all objects that match the pattern(s) '{0}'", string.Join(",", _patterns));
            foreach (var file in matches)
            {
                if (file as DirectoryInfo != null)
                    yield return file as DirectoryInfo;
            }
        }

        private DirectoryInfo GetDirInfo()
        {
            try
            {
                var drive = new DirectoryInfo(Model.Path);
                return drive;
            }
            catch
            {
            }

            return null;
        }
        #endregion methods
    }
}
