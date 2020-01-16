using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaConverters
{
    partial struct MultiValueConverterArgs<T> : IEquatable<MultiValueConverterArgs<T>>
    {
        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MultiValueConverterArgs<T> x, MultiValueConverterArgs<T> y)
            =>
                (x.values != null && y.values != null && x.values.SequenceEqual(y.values) || x.values == null && y.values == null) &&
                Equals(x.Culture, y.Culture);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MultiValueConverterArgs<T> x, MultiValueConverterArgs<T> y) => !(x == y);

        /// <inheritdoc />
        public override int GetHashCode()
            => (values?.Aggregate(0, (a, item) => a ^ EqualityComparer<T>.Default.GetHashCode(item)) ?? 0) ^ (Culture?.GetHashCode() ?? 0);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is MultiValueConverterArgs<T> args && Equals(args);

        /// <inheritdoc />
        public bool Equals(MultiValueConverterArgs<T> other) => this == other;
    }
}