namespace FolderBrowser.ViewModels.Messages
{
    using FileSystemModels.ViewModels.Base;

    /// <summary>
    /// Class keeps track of a message display that can pop-up messages
    /// and disappear depending on whether the IsErrorMessageAvailable
    /// property is true or not.
    /// </summary>
    internal class DisplayMessageViewModel : ViewModelBase, IDisplayMessageViewModel
    {
        private bool mIsErrorMessageAvailable;
        private string mMessage;

        /// <summary>
        /// Decide whether a message is available for display or not.
        /// </summary>
        public bool IsErrorMessageAvailable
        {
            get
            {
                return mIsErrorMessageAvailable;

            }

            set
            {
                if (mIsErrorMessageAvailable != value)
                {
                    mIsErrorMessageAvailable = value;
                    RaisePropertyChanged(() => IsErrorMessageAvailable);
                }
            }
        }

        /// <summary>
        /// Gets/sets the message for display.
        /// </summary>
        public string Message
        {
            get
            {
                return mMessage;

            }

            set
            {
                if (mMessage != value)
                {
                    mMessage = value;
                    RaisePropertyChanged(() => Message);
                }
            }
        }
        
        /// <summary>
        /// Resets the current message with the given string.
        /// </summary>
        /// <param name="Message"></param>
        public void SetMessage(string message)
        {
            this.Message = message;
            this.IsErrorMessageAvailable = true;
        }
    }
}
