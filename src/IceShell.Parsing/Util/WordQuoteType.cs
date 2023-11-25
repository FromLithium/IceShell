// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Parsing.Util;

/// <summary>
/// An enumeration of all possible word quoting types.
/// </summary>
public enum WordQuoteType
{
    /// <summary>
    /// The word is not quoted.
    /// </summary>
    None,
    /// <summary>
    /// The word is quoted by a pair of double quotes (<c>"</c>).
    /// </summary>
    Quotes,
    /// <summary>
    /// The word is quoted by a pair of round parentheses (<c>()</c>).
    /// </summary>
    Parentheses
}