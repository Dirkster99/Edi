namespace FolderBrowser.Interfaces
{
    using System;

    public interface ICustomFolderItemViewModel
    {
        string Path { get; }
        Environment.SpecialFolder SpecialFolder { get; }
    }
}
