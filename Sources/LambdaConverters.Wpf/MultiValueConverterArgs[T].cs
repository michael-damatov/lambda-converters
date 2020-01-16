using System;
using System.Collections.Generic;
using System.Globalization;

namespace LambdaConverters
{
    /// <summary>
    /// Provides data for conversion functions.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public partial struct MultiValueConverterArgs<T>
    {
        readonly IReadOnlyList<T>? values;

        internal MultiValueConverterArgs(IReadOnlyList<T> values, CultureInfo? culture)
        {
            this.values = values;

            Culture = culture;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public IReadOnlyList<T> Values => values ??
#if NET45
            new T[] { };
#else
            Array.Empty<T>();
#endif

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo? Culture { get; }
    }
}