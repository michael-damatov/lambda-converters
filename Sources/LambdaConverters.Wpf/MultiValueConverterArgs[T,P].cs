using System;
using System.Collections.Generic;
using System.Globalization;

namespace LambdaConverters
{
    /// <summary>
    /// Provides data for parameterized conversion functions.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="P">The parameter type.</typeparam>
    public partial struct MultiValueConverterArgs<T, P>
    {
        readonly IReadOnlyList<T>? values;

        internal MultiValueConverterArgs(IReadOnlyList<T> values, P parameter, CultureInfo? culture)
        {
            this.values = values;

            Parameter = parameter;
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
        /// Gets the parameter.
        /// </summary>
        public P Parameter { get; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo? Culture { get; }
    }
}