namespace Edi.Core.Models.Utillities.FileSystem
{
    using System;
    using System.IO;
    using System.Text;

    // Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this
    // software and associated documentation files (the "Software"), to deal in the Software
    // without restriction, including without limitation the rights to use, copy, modify, merge,
    // publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
    // to whom the Software is furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all copies or
    // substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
    // INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
    // PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
    // FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
    // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    // DEALINGS IN THE SOFTWARE.
    /// <summary>
    /// Source:
    /// https://github.com/icsharpcode/SharpDevelop/blob/master/src/Main/Core/Project/Src/Services/FileUtility/PathName.cs
    /// Represents a path to a file or directory.
    /// </summary>
    public abstract class PathName
	{
		protected readonly string NormalizedPath;

		protected PathName(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			if (path.Length == 0)
				throw new ArgumentException("The empty string is not a valid path");
			NormalizedPath = NormalizePath(path);
		}

		protected PathName(PathName path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			NormalizedPath = path.NormalizedPath;
		}

		public static implicit operator string(PathName path)
		{
			return path?.NormalizedPath;
		}

		public override string ToString()
		{
			return NormalizedPath;
		}

		/// <summary>
		/// Gets whether this path is relative.
		/// </summary>
		public bool IsRelative => !Path.IsPathRooted(NormalizedPath);

		/// <summary>
		/// Gets the directory name.
		/// </summary>
		/// <remarks>
		/// Corresponds to <c>System.IO.Path.GetDirectoryName</c>
		/// </remarks>
		public DirectoryName GetParentDirectory()
		{
			if (NormalizedPath.Length < 2 || NormalizedPath[1] != ':')
				return DirectoryName.Create(Path.Combine(NormalizedPath, ".."));
			return DirectoryName.Create(Path.GetDirectoryName(NormalizedPath));
		}

		/// <summary>
		/// Gets the normalized version of fileName.
		/// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
		/// </summary>
		public static string NormalizePath(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return fileName;

			int i;

			bool isWeb = false;
			for (i = 0; i < fileName.Length; i++)
			{
				if (fileName[i] == '/' || fileName[i] == '\\')
					break;
				if (fileName[i] == ':')
				{
					if (i > 1)
						isWeb = true;
					break;
				}
			}

			char outputSeparator = isWeb ? '/' : Path.DirectorySeparatorChar;
			bool isRelative;

			StringBuilder result = new StringBuilder();
			if (isWeb == false && fileName.StartsWith(@"\\", StringComparison.Ordinal) || fileName.StartsWith("//", StringComparison.Ordinal))
			{
				// UNC path
				i = 2;
				result.Append(outputSeparator);
				isRelative = false;
			}
			else
			{
				i = 0;
				isRelative = !isWeb && (fileName.Length < 2 || fileName[1] != ':');
			}
			int levelsBack = 0;
			int segmentStartPos = i;
			for (; i <= fileName.Length; i++)
			{
				if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
				{
					int segmentLength = i - segmentStartPos;
					switch (segmentLength)
					{
						case 0:
							// ignore empty segment (if not in web mode)
							if (isWeb)
							{
								result.Append(outputSeparator);
							}
							break;
						case 1:
							// ignore /./ segment, but append other one-letter segments
							if (fileName[segmentStartPos] != '.')
							{
								if (result.Length > 0) result.Append(outputSeparator);
								result.Append(fileName[segmentStartPos]);
							}
							break;
						case 2:
							if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
							{
								// remove previous segment
								int j;
								for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--)
								{
								}

								if (j > 0)
								{
									result.Length = j;
								}
								else if (isRelative)
								{
									if (result.Length == 0)
										levelsBack++;
									else
										result.Length = 0;
								}
								break;
							}
							else
							{
								// append normal segment
								goto default;
							}
						default:
							if (result.Length > 0) result.Append(outputSeparator);
							result.Append(fileName, segmentStartPos, segmentLength);
							break;
					}
					segmentStartPos = i + 1; // remember start position for next segment
				}
			}

			if (isWeb == false)
			{
				if (isRelative)
				{
					for (int j = 0; j < levelsBack; j++)
					{
						result.Insert(0, ".." + outputSeparator);
					}
				}
				if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
				{
					result.Length -= 1;
				}
				if (result.Length == 2 && result[1] == ':')
				{
					result.Append(outputSeparator);
				}
				if (result.Length == 0)
					return ".";
			}

			return result.ToString();
		}
	}
}
