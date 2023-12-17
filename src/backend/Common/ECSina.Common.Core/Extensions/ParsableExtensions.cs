using System.Diagnostics.CodeAnalysis;

namespace ECSina.Common.Core.Extensions;

public static class ParsableExtensions
{
    /// <summary>
    /// Tries to parse a string into a value
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="result">On return, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
    /// <param name="formatProvider">An object that provides culture-specific formatting information about <paramref name="s" />.</param>
    /// <returns><c>true</c> if <paramref name="s" /> was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParseTo<T>(
        [NotNullWhen(true)] this string? s,
        [MaybeNullWhen(false)] out T result,
        IFormatProvider? formatProvider = null
    )
        where T : IParsable<T>
    {
        return T.TryParse(s, formatProvider, out result);
    }
}
