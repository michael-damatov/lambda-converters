using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace LambdaConverters
{
    /// <summary>
    /// Provides data for conversion functions.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public partial struct MultiValueConverterArgs<T>
    {
        readonly IReadOnlyList<T> values;

        internal MultiValueConverterArgs([NotNull] IReadOnlyList<T> values, CultureInfo culture)
        {
            this.values = values;

            Culture = culture;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        [NotNull]
        public IReadOnlyList<T> Values => values ?? new List<T>().AsReadOnly();

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture { get; }
    }
}