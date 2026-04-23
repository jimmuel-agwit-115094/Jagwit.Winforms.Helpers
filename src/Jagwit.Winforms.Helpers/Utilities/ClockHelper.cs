using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides helpers for retrieving the current server date and time via an ADO.NET connection.
    /// </summary>
    public static class ClockHelper
    {
        /// <summary>
        /// Optional override for the date/time source. Useful for unit testing or custom time providers.
        /// When <see langword="null"/>, <see cref="GetServerDateTimeAsync"/> queries the database directly.
        /// </summary>
        public static Func<Task<DateTime>>? DateTimeProvider { get; set; }

        /// <summary>
        /// Returns the current server date and time by executing a scalar SQL expression against
        /// <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">
        /// An open or closed <see cref="DbConnection"/>. If closed, it is opened and closed automatically.
        /// When using EF Core you can obtain it via <c>context.Database.GetDbConnection()</c>.
        /// </param>
        /// <param name="sql">
        /// A scalar SQL expression that returns the current timestamp.
        /// Defaults to <c>SELECT GETDATE()</c> (SQL Server).
        /// Use <c>SELECT NOW()</c> for MySQL or PostgreSQL.
        /// </param>
        /// <returns>The server's current <see cref="DateTime"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the query returns no result.</exception>
        public static async Task<DateTime> GetServerDateTimeAsync(DbConnection connection, string sql = "SELECT GETDATE()")
        {
            if (DateTimeProvider != null)
                return await DateTimeProvider();

            bool shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
                await connection.OpenAsync();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = await command.ExecuteScalarAsync();

                    if (result == null || result == DBNull.Value)
                        throw new InvalidOperationException("The server date/time query returned no result.");

                    return Convert.ToDateTime(result);
                }
            }
            finally
            {
                if (shouldClose)
                    connection.Close();
            }
        }
    }
}
