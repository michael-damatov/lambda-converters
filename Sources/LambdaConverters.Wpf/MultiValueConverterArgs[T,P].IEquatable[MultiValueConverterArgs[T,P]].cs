using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LambdaConverters
{
    partial struct MultiValueConverterArgs<T, P> : IEquatable<MultiValueConverterArgs<T, P>>
    {
        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MultiValueConverterArgs<T, P> x, MultiValueConverterArgs<T, P> y)
            => (x.values != null && y.values != null && x.values.SequenceEqual(y.values) || x.values == null && y.values == null) &&
                EqualityComparer<P>.Default.Equals(x.Parameter, y.Parameter) &&
                EqualityComparer<CultureInfo?>.Default.Equals(x.Culture, y.Culture);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MultiValueConverterArgs<T, P> x, MultiValueConverterArgs<T, P> y) => !(x == y);

        /// <inheritdoc />
        public override int GetHashCode()
            => (values?.Aggregate(0, (a, item) => a ^ (item is { } ? EqualityComparer<T>.Default.GetHashCode(item) : 0)) ?? 0) ^
                (Parameter is { } ? EqualityComparer<P>.Default.GetHashCode(Parameter) : 0) ^
                (Culture != null ? EqualityComparer<CultureInfo>.Default.GetHashCode(Culture) : 0);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is MultiValueConverterArgs<T, P> args && Equals(args);

        /// <inheritdoc />
        public bool Equals(MultiValueConverterArgs<T, P> other) => this == other;
    }
}