using System;
using System.Collections.Generic;
using System.Globalization;

namespace LambdaConverters
{
    partial struct ValueConverterArgs<T, P> : IEquatable<ValueConverterArgs<T, P>>
    {
        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValueConverterArgs<T, P> x, ValueConverterArgs<T, P> y)
            => EqualityComparer<T>.Default.Equals(x.Value, y.Value) &&
                EqualityComparer<P>.Default.Equals(x.Parameter, y.Parameter) &&
                EqualityComparer<CultureInfo?>.Default.Equals(x.Culture, y.Culture);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValueConverterArgs<T, P> x, ValueConverterArgs<T, P> y) => !(x == y);

        /// <inheritdoc />
        public override int GetHashCode()
            => (Value is { } ? EqualityComparer<T>.Default.GetHashCode(Value) : 0) ^
                (Parameter is { } ? EqualityComparer<P>.Default.GetHashCode(Parameter) : 0) ^
                (Culture != null ? EqualityComparer<CultureInfo>.Default.GetHashCode(Culture) : 0);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ValueConverterArgs<T, P> args && Equals(args);

        /// <inheritdoc />
        public bool Equals(ValueConverterArgs<T, P> other) => this == other;
    }
}