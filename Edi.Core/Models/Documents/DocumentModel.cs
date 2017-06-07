namespace Edi.Core.Models.Documents
{
    using System;
    using Edi.Core.Interfaces.Documents;
    using Edi.Core.Models.Utillities.FileSystem;

    /// <summary>
    /// Class models the basic properties and behaviours of a low level file stored on harddisk.
    /// </summary>
    public class DocumentModel : IDocumentModel, IDisposable
    {
        #region fields
        private FileName mFileName;

        private FileChangeWatcher mFileChangeWatcher = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Hidden standard class constructor
        /// </summary>
        public DocumentModel()
        {
            this.SetDefaultDocumentModel();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public DocumentModel(DocumentModel copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            this.IsReadonly = copyThis.IsReadonly;
            this.IsReal = copyThis.IsReal;
            this.mFileName = new FileName(copyThis.mFileName);
        }
        #endregion constructors

        /// <summary>
        /// Occurs when the file name has changed.
        /// </summary>
        public event EventHandler FileNameChanged;

        #region properties
        /// <summary>
        /// Gets whether the file content on storake (harddisk) can be changed
        /// or whether file is readonly through file properties.
        /// </summary>
        public bool IsReadonly { get; private set; }

        /// <summary>
        /// Determines whether a document has ever been stored on disk or whether
        /// the current path and other file properties are currently just initialized
        /// in-memory with defaults.
        /// </summary>
        public bool IsReal { get; private set; }

        /// <summary>
        /// Gets the complete path and file name for this document.
        /// </summary>
        public string FileNamePath
        {
            get
            {
                if (this.mFileName == null)
                    return null;

                return this.mFileName.ToString();
            }
        }

        /// <summary>
        /// Gets the name of a file.
        /// </summary>
        public string FileName
        {
            get
            {
                if (this.mFileName == null)
                    return null;

                return System.IO.Path.GetFileName(this.FileNamePath);
            }
        }

        /// <summary>
        /// Gets the path of a file.
        /// </summary>
        public string Path
        {
            get
            {
                return System.IO.Path.GetFullPath(this.FileNamePath);
            }
        }

        /// <summary>
        /// Gets the file extension of the document represented by this path.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return this.mFileName.GetExtension();
            }
        }

        public bool WasChangedExternally
        {
            get
            {
                if (this.mFileChangeWatcher == null)
                    return false;

                return this.mFileChangeWatcher.WasChangedExternally;
            }

            set
            {
                if (this.mFileChangeWatcher == null)
                    return;

                if (this.mFileChangeWatcher.WasChangedExternally != value)
                    this.mFileChangeWatcher.WasChangedExternally = value;
            }
        }
        #endregion properties

        #region methods
        protected virtual void ChangeFileName(FileName newValue)
        {
            ////SD.MainThread.VerifyAccess();

            //// Already done by caller
            ////this.fileName = newValue;

            FileNameChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Assigns a filename and path to this document model. This will also
        /// refresh all properties (IsReadOnly etc..) that can be queried for this document.
        /// </summary>
        /// <param name="fileNamePath"></param>
        /// <param name="isReal">Determines whether file exists on disk
        /// (file open -> properties are refreshed from persistence) or not
        /// (properties are reset to default).</param>
        public void SetFileNamePath(string fileNamePath, bool isReal)
        {
            if (fileNamePath != null)
                this.mFileName = new FileName(fileNamePath);

            this.IsReal = isReal;

            if (this.IsReal == true && fileNamePath != null)
            {
                this.QueryFileProperies();
                this.ChangeFileName(this.mFileName);
            }
        }

        /// <summary>
        /// Resets the IsReal property to adjust model when a new document has been saved
        /// for the very first time.
        /// </summary>
        /// <param name="IsReal">Determines whether file exists on disk
        /// (file open -> properties are refreshed from persistence) or not
        /// (properties are reset to default).</param>
        public void SetIsReal(bool isReal)
        {
            this.IsReal = isReal;

            if (this.IsReal == true)
            {
                if (this.mFileChangeWatcher == null)
                    this.QueryFileProperies();
            }
        }

        /// <summary>
        /// Query sub-system for basic properties if this file is supposed to exist in persistence.
        /// </summary>
        private void QueryFileProperies()
        {
            try
            {
                if (this.IsReal == true)
                {
                    System.IO.FileInfo f = new System.IO.FileInfo(this.FileNamePath);
                    this.IsReadonly = f.IsReadOnly;

                    if (this.mFileChangeWatcher != null)
                    {
                        this.mFileChangeWatcher.Dispose();
                        this.mFileChangeWatcher = null;
                    }

                    this.mFileChangeWatcher = new FileChangeWatcher(this);
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error in QueryFileProperies", exp);
            }
        }

        /// <summary>
        /// Set a file specific value to determine whether file
        /// watching is enabled/disabled for this file.
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public bool EnableDocumentFileWatcher(bool isEnabled)
        {
            try
            {
                if (isEnabled == true)
                {
                    // Enable file watcher for this file
                    if (this.IsReal == false)
                        return false;

                    if (this.mFileChangeWatcher == null)
                        this.QueryFileProperies();

                    if (this.mFileChangeWatcher == null)
                        return false;

                    if (this.mFileChangeWatcher.Enabled == false)
                        this.mFileChangeWatcher.Enabled = true;

                    return true;
                }
                else
                {
                    // Disable file watcher for this file
                    if (this.mFileChangeWatcher == null)
                        return false;

                    if (this.mFileChangeWatcher.Enabled == true)
                        this.mFileChangeWatcher.Enabled = false;

                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Resets all document property values to their defaults.
        /// </summary>
        private void SetDefaultDocumentModel()
        {
            this.IsReadonly = true;
            this.IsReal = false;
            this.mFileName = null;
        }

        public void Dispose()
        {
            if (this.mFileChangeWatcher != null)
            {
                this.mFileChangeWatcher.Dispose();
                this.mFileChangeWatcher = null;
            }
            GC.SuppressFinalize(this);
        }
        #endregion methods
    }
}
