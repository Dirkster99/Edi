namespace FileSystemModels.Browse
{
    public enum BrowseResult
    {
        // Result is unknown since browse task is currently running
        // or completed with unknown result ...
        Unknown = 0,

        // Browse Process OK - destination does exist and is accessible
        Complete = 1,

        // Error indicator - destination does not exist or is not accessible
        InComplete = 2
    }
}
