using System.Data;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides helpers for detecting optimistic concurrency violations using row-version tokens.
    /// </summary>
    public static class VersionCheckerHelper
    {
        /// <summary>
        /// Compares the version token captured at load time against the current database value.
        /// </summary>
        /// <param name="oldVersion">The version value recorded when the record was first loaded.</param>
        /// <param name="currentVersion">The version value read back from the database immediately before saving.</param>
        /// <returns><see langword="true"/> when the versions match and the save may proceed.</returns>
        /// <exception cref="DBConcurrencyException">
        /// Thrown when <paramref name="oldVersion"/> differs from <paramref name="currentVersion"/>,
        /// indicating that another user or process has modified the record since it was last loaded.
        /// </exception>
        public static bool ConcurrencyCheck(long oldVersion, long currentVersion)
        {
            if (oldVersion != currentVersion)
                throw new DBConcurrencyException(
                    "Concurrency Violation: The data you are trying to modify has been updated by another " +
                    "user or process since you last accessed it. Please refresh the data and try again.");

            return true;
        }
    }
}
