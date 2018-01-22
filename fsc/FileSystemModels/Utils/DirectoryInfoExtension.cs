namespace FileSystemModels.Utils
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  /// <summary>
  /// Class implements an extension of the <seealso cref="System.IO.DirectoryInfo"/> class.
  /// </summary>
  public static class DirectoryInfoExtension
  {
    /// <summary>
    /// Method implements an extension that lets us filter files
    /// with multiple filter aruments.
    /// </summary>
    /// <param name="dir">Points at the folder that is queried for files and folder entries.</param>
    /// <param name="extensions">Contains the extension that we want to filter for, eg: string[]{"*.*"} or string[]{"*.tex", "*.txt"}</param>
    public static IEnumerable<FileInfo> SelectFilesByFilter(this DirectoryInfo dir,
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
    public static IEnumerable<DirectoryInfo> SelectDirectoriesByFilter(this DirectoryInfo dir,
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
  }
}
