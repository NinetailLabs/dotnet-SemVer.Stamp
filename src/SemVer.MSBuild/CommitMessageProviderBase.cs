﻿using System;
using JetBrains.Annotations;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global

namespace SemVer.MSBuild
{
  public abstract class CommitMessageProviderBase : ICommitMessageProvider
  {
    /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException"><paramref name="path" /> is not rooted.</exception>
    protected CommitMessageProviderBase([NotNull] string path,
                                        [CanBeNull] string baseRevision)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }
      if (!System.IO.Path.IsPathRooted(path))
      {
        throw new ArgumentException($"{nameof(path)} must be rooted.",
                                    nameof(path));
      }
      this.Path = this.PathAddBackslash(path);
      this.BaseRevision = baseRevision;
    }

    [NotNull]
    protected virtual string Path { get; }

    [CanBeNull]
    protected virtual string BaseRevision { get; }

    /// <remarks>https://stackoverflow.com/a/20406065</remarks>
    [NotNull]
    public virtual string PathAddBackslash([NotNull] string path)
    {
      // They're always one character but EndsWith is shorter than
      // array style access to last path character. Change this
      // if performance are a (measured) issue.
      string separator1 = System.IO.Path.DirectorySeparatorChar.ToString();
      string separator2 = System.IO.Path.AltDirectorySeparatorChar.ToString();

      // Trailing white spaces are always ignored but folders may have
      // leading spaces. It's unusual but it may happen. If it's an issue
      // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
      path = path.TrimEnd();

      // Argument is always a directory name then if there is one
      // of allowed separators then I have nothing to do.
      if (path.EndsWith(separator1) || path.EndsWith(separator2))
        return path;

      // If there is the "alt" separator then I add a trailing one.
      // Note that URI format (file://drive:\path\filename.ext) is
      // not supported in most .NET I/O functions then we don't support it
      // here too. If you have to then simply revert this check:
      // if (path.Contains(separator1))
      //     return path + separator1;
      //
      // return path + separator2;
      if (path.Contains(separator2))
        return path + separator2;

      // If there is not an "alt" separator I add a "normal" one.
      // It means path may be with normal one or it has not any separator
      // (for example if it's just a directory name). In this case I
      // default to normal as users expect.
      return path + separator1;
    }

    /// <inheritdoc />
    public abstract string[] GetCommitMessages();
  }
}
