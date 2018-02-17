namespace Edi.Core.Models.Utillities.FileSystem
{
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

	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Source: https://github.com/icsharpcode/SharpDevelop/blob/master/src/Main/Core/Project/Src/Services/FileUtility/DirectoryName.cs
	/// Represents a path to a directory.
	/// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
	/// </summary>
	[TypeConverter(typeof(DirectoryNameConverter))]
	public sealed class DirectoryName : PathName
	{
		#region fields
		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		#endregion fields

		#region constructors
		public DirectoryName(string path)
			: base(path)
		{
		}

		////[Obsolete("The input already is a DirectoryName")]
		////public DirectoryName(DirectoryName path)
		////	: base(path)
		////{
		////}
		#endregion constructors

		#region methods
		/// <summary>
		/// Creates a DirectoryName instance from the string.
		/// It is valid to pass null or an empty string to this method (in that case, a null reference will be returned).
		/// </summary>
		public static DirectoryName Create(string DirectoryName)
		{
			if (string.IsNullOrEmpty(DirectoryName))
				return null;
			else
				return new DirectoryName(DirectoryName);
		}

		////[Obsolete("The input already is a DirectoryName")]
		////public static DirectoryName Create(DirectoryName directoryName)
		////{
		////	return directoryName;
		////}

		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public DirectoryName Combine(DirectoryName relativePath)
		{
			if (relativePath == null)
				return null;
			return Create(Path.Combine(normalizedPath, relativePath));
		}

		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public FileName Combine(FileName relativePath)
		{
			if (relativePath == null)
				return null;
			return FileName.Create(Path.Combine(normalizedPath, relativePath));
		}

		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public FileName CombineFile(string relativeFileName)
		{
			if (relativeFileName == null)
				return null;
			return FileName.Create(Path.Combine(normalizedPath, relativeFileName));
		}

		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public DirectoryName CombineDirectory(string relativeDirectoryName)
		{
			if (relativeDirectoryName == null)
				return null;
			return Create(Path.Combine(normalizedPath, relativeDirectoryName));
		}

		/// <summary>
		/// Converts the specified absolute path into a relative path (relative to <c>this</c>).
		/// </summary>
		public DirectoryName GetRelativePath(DirectoryName path)
		{
			if (path == null)
				return null;
			return Create(GetRelativePath(normalizedPath, path));
		}

		/// <summary>
		/// Converts the specified absolute path into a relative path (relative to <c>this</c>).
		/// </summary>
		public FileName GetRelativePath(FileName path)
		{
			if (path == null)
				return null;
			return FileName.Create(GetRelativePath(normalizedPath, path));
		}

		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			if (string.IsNullOrEmpty(baseDirectoryPath))
			{
				return absPath;
			}
			if (IsUrl(absPath) || IsUrl(baseDirectoryPath))
			{
				return absPath;
			}

			baseDirectoryPath = NormalizePath(baseDirectoryPath);
			absPath = NormalizePath(absPath);

			string[] bPath = baseDirectoryPath != "." ? baseDirectoryPath.Split(separators) : new string[0];
			string[] aPath = absPath != "." ? absPath.Split(separators) : new string[0];
			int indx = 0;
			for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
			{
				if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
					break;
			}

			if (indx == 0 && (Path.IsPathRooted(baseDirectoryPath) || Path.IsPathRooted(absPath)))
			{
				return absPath;
			}

			if (indx == bPath.Length && indx == aPath.Length)
			{
				return ".";
			}
			StringBuilder erg = new StringBuilder();
			for (int i = indx; i < bPath.Length; ++i)
			{
				erg.Append("..");
				erg.Append(Path.DirectorySeparatorChar);
			}
			erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
			if (erg[erg.Length - 1] == Path.DirectorySeparatorChar)
				erg.Length -= 1;
			return erg.ToString();
		}

		public static bool IsUrl(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return path.IndexOf("://", StringComparison.Ordinal) > 0;
		}

		/// <summary>
		/// Gets the directory name as a string, including a trailing backslash.
		/// </summary>
		public string ToStringWithTrailingBackslash()
		{
			if (normalizedPath.EndsWith("\\", StringComparison.Ordinal))
				return normalizedPath; // trailing backslash exists in normalized version for root of drives ("C:\")
			else
				return normalizedPath + "\\";
		}

		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as DirectoryName);
		}

		public bool Equals(DirectoryName other)
		{
			if (other != null)
				return string.Equals(normalizedPath, other.normalizedPath, StringComparison.OrdinalIgnoreCase);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(normalizedPath);
		}

		public static bool operator ==(DirectoryName left, DirectoryName right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}

		public static bool operator !=(DirectoryName left, DirectoryName right)
		{
			return !(left == right);
		}

		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator ==(DirectoryName left, string right)
		{
			return (string)left == right;
		}

		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator !=(DirectoryName left, string right)
		{
			return (string)left != right;
		}

		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator ==(string left, DirectoryName right)
		{
			return left == (string)right;
		}

		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator !=(string left, DirectoryName right)
		{
			return left != (string)right;
		}
		#endregion
		#endregion methods
	}
}
