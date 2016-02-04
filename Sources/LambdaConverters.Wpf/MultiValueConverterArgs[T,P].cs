using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace LambdaConverters
{
    /// <summary>
    /// Provides data for parameterized conversion functions.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="P">The parameter type.</typeparam>
    public partial struct MultiValueConverterArgs<T, P>
    {
        readonly IReadOnlyList<T> values;

        internal MultiValueConverterArgs([NotNull] IReadOnlyList<T> values, P parameter, CultureInfo culture)
        {
            this.values = values;

            Parameter = parameter;
            Culture = culture;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        [NotNull]
        public IReadOnlyList<T> Values => values ?? new List<T>().AsReadOnly();

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        public P Parameter { get; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture { get; }
    }
}