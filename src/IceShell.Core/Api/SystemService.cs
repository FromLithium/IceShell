// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Provides services to access system functions.
/// </summary>
public static partial class SystemService
{
    [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8)]
    // ReSharper disable once IdentifierTypo
    // ReSharper disable once InconsistentNaming
    private static partial int rpmatch(string response);

    /// <summary>
    /// Determines whether the provided response is an affirmative answer or a negative answer.
    /// </summary>
    /// <param name="response">The response to check.</param>
    /// <param name="defaultAnswer">The response to return if the user have not provided a response.</param>
    /// <returns>The type of the answer.</returns>
    /// <remarks>
    /// <para>On GNU/Linux systems, this method is a wrapper of the <see href="https://www.man7.org/linux/man-pages/man3/rpmatch.3.html">rpmatch</see> system method;
    /// on other systems, this method determines based on the following values (case insensitive):</para>
    /// <list type="bullet">
    ///     <item><c>Y</c> -> Yes</item>
    ///     <item><c>N</c> -> No</item>
    ///     <item><c>Yes</c></item>
    ///     <item><c>No</c></item>
    /// </list>
    /// </remarks>
    public static YesNoAnswer MatchYesNo(string response, YesNoAnswer defaultAnswer = YesNoAnswer.Invalid)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return defaultAnswer;
        }

        if (OperatingSystem.IsLinux())
        {
            var nativeYesNo = rpmatch(response);

            return nativeYesNo switch
            {
                1 => YesNoAnswer.Yes,
                0 => YesNoAnswer.No,
                _ => YesNoAnswer.Invalid
            };
        }

        if (response.Equals("Yes", StringComparison.OrdinalIgnoreCase))
        {
            return YesNoAnswer.Yes;
        }

        if (response.Equals("No", StringComparison.OrdinalIgnoreCase))
        {
            return YesNoAnswer.No;
        }

        if (response.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            return YesNoAnswer.Yes;
        }

        if (response.Equals("n", StringComparison.OrdinalIgnoreCase))
        {
            return YesNoAnswer.Yes;
        }

        return YesNoAnswer.Invalid;
    }
}
