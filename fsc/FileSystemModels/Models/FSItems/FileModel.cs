namespace FileSystemModels.Models.FSItems
{
    using FileSystemModels.Interfaces;
    using System.IO;
    using System.Security;

    public class FileModel : Base.FileSystemModel
    {
        #region constructors
        /// <summary>
        /// Parameterized class  constructor
        /// </summary>
        /// <param name="model"></param>
        [SecuritySafeCritical]
        public FileModel(IPathModel model)
          : base(model)
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets an instance of the parent directory or null
        /// if file is no longer available or in-accessible or ...
        /// </summary>
        public DirectoryInfo Directory
        {
            get
            {
                var file = GetFileInfo();
                
                return (file != null ? file.Directory : null);
            }
        }

        /// <summary>
        /// Gets a string representing the directory's full path.
        /// or string.Empty if file is no longer available or in-accessible or ...
        /// </summary>
        public string DirectoryName
        {
            get
            {
                var file = GetFileInfo();

                return (file != null ? file.DirectoryName : string.Empty);
            }
        }

        /// <summary>
        /// Gets a true value if file actually exists, or false if not,
        /// if file is no longer available or in-accessible or ...
        /// </summary>
        public bool Exists
        {
            get
            {
                var file = GetFileInfo();

                return (file != null ? file.Exists : false);
            }
        }

        /// <summary>
        /// Gets a false value if file is not readonly,
        /// or true if it is readonly, or if file is no
        /// longer available or in-accessible or ...
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                var file = GetFileInfo();

                return (file != null ? file.IsReadOnly : true);
            }
        }

        /// <summary>
        /// Gets the length of a file in bytes,
        /// or 0 if file is no longer available or in-accessible or ...
        /// </summary>
        public long Length
        {
            get
            {
                var file = GetFileInfo();

                return (file != null ? file.Length : 0);
            }
        }

        private FileInfo GetFileInfo()
        {
            try
            {
                var file = new FileInfo(Model.Path);
                return file;
            }
            catch
            {
            }

            return null;
        }
        #endregion properties
    }
}
