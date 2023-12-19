// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Utilities;

/// <summary>
/// Provides extension methods for enumerable objects.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Determines whether the specified enumerable have at least count items.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="count">The amount of items.</param>
    /// <typeparam name="T">The type of the items inside the specified enumerable.</typeparam>
    /// <returns><see langword="true"/> if the specified enumerable have at least count items; otherwise, <see langword="false"/>.</returns>
    public static bool HaveAtLeast<T>(this IEnumerable<T> enumerable, int count)
    {
        var currentCount = 0;
        
        foreach (var _ in enumerable)
        {
            currentCount++;

            if (currentCount >= count)
            {
                return true;
            }
        }

        return false;
    }
}