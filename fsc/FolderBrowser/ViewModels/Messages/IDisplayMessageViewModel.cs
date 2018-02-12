namespace FolderBrowser.ViewModels.Messages
{
    using System;

    /// <summary>
    /// Defines an interface for a viewmodel class that can be
    /// used to pop-up messages in a UI (without using messageboxes).
    /// </summary>
    public interface IDisplayMessageViewModel : ISetMessageDisplay
    {
        bool IsErrorMessageAvailable { get; set; }

        string Message { get; set; }
    }

    public interface ISetMessageDisplay
    {
        void SetMessage(string Message);
    }
}
