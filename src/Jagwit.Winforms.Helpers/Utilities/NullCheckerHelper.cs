using System;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides methods for validating that entity references loaded from the database are not null.
    /// </summary>
    public static class NullCheckerHelper
    {
        /// <summary>
        /// Throws a <see cref="NullReferenceException"/> if <paramref name="entity"/> is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The entity type. Must be a reference type.</typeparam>
        /// <param name="entity">The entity instance to check.</param>
        /// <exception cref="NullReferenceException">
        /// Thrown when <paramref name="entity"/> is <see langword="null"/>, with a message that includes the type name.
        /// </exception>
        public static void NullCheck<T>(T entity) where T : class
        {
            if (entity == null)
                throw new NullReferenceException(
                    $"The data of type '{typeof(T).Name}' does not exist in the database.");
        }
    }
}
