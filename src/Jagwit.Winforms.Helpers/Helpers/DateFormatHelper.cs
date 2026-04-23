using System;

namespace Jagwit.Winforms.Helpers.Helpers
{
    /// <summary>
    /// Provides static methods for formatting <see cref="DateTime"/> values into display-ready strings.
    /// </summary>
    public static class DateFormatHelper
    {
        /// <summary>Formats a date as <c>MMMM dd, yyyy</c> (e.g. April 23, 2026).</summary>
        /// <param name="date">The date to format.</param>
        /// <param name="format">Optional custom format string. Defaults to <c>MMMM dd, yyyy</c>.</param>
        public static string FormatDate(DateTime date, string format = "MMMM dd, yyyy")
            => date.ToString(format);

        /// <summary>Formats a date and time as <c>MMMM dd, yyyy | hh:mm tt</c> (e.g. April 23, 2026 | 02:30 PM).</summary>
        /// <param name="date">The date and time to format.</param>
        /// <param name="format">Optional custom format string. Defaults to <c>MMMM dd, yyyy | hh:mm tt</c>.</param>
        public static string FormatDateWithTime(DateTime date, string format = "MMMM dd, yyyy | hh:mm tt")
            => date.ToString(format);

        /// <summary>Formats a date as <c>MMM. dd, yyyy</c> (e.g. Apr. 23, 2026).</summary>
        /// <param name="date">The date to format.</param>
        /// <param name="format">Optional custom format string. Defaults to <c>MMM. dd, yyyy</c>.</param>
        public static string FormatShortDate(DateTime date, string format = "MMM. dd, yyyy")
            => date.ToString(format);
    }
}
