namespace Panelverse.Core.Sorting;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Provides natural (human-friendly) string comparison, e.g. "2" < "10" and
/// "page2.jpg" < "page10.jpg" while remaining case-insensitive for letters.
/// </summary>
public static class NaturalSort
{
    private static readonly Regex NumberRegex = new Regex("\\d+", RegexOptions.Compiled);

    public static int Compare(string? left, string? right)
    {
        if (ReferenceEquals(left, right)) return 0;
        if (left is null) return -1;
        if (right is null) return 1;

        int i = 0, j = 0;
        while (i < left.Length && j < right.Length)
        {
            var c1 = left[i];
            var c2 = right[j];

            // If both start with a number, compare by numeric value spans
            if (char.IsDigit(c1) && char.IsDigit(c2))
            {
                var m1 = NumberRegex.Match(left, i);
                var m2 = NumberRegex.Match(right, j);

                var n1 = ParseNumber(m1.Value);
                var n2 = ParseNumber(m2.Value);
                if (n1 != n2) return n1.CompareTo(n2);

                // If equal numerically, prefer fewer leading zeros
                if (m1.Value.Length != m2.Value.Length)
                    return m1.Value.Length.CompareTo(m2.Value.Length);

                i += m1.Length;
                j += m2.Length;
                continue;
            }

            // Case-insensitive char compare
            var ci = char.ToUpperInvariant(c1).CompareTo(char.ToUpperInvariant(c2));
            if (ci != 0) return ci;
            i++;
            j++;
        }

        return left.Length.CompareTo(right.Length);
    }

    private static long ParseNumber(string value)
    {
        // Handle large numbers gracefully without exceptions
        if (long.TryParse(value, out var result)) return result;
        var trimmed = value.TrimStart('0');
        return trimmed.Length;
    }
}


