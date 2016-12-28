using System;
using System.Collections.Generic;

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
            => Equals(x.Value, y.Value) && Equals(x.Parameter, y.Parameter) && Equals(x.Culture, y.Culture);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValueConverterArgs<T, P> x, ValueConverterArgs<T, P> y)
            => !Equals(x.Value, y.Value) || !Equals(x.Parameter, y.Parameter) || !Equals(x.Culture, y.Culture);

        /// <inheritdoc />
        public override int GetHashCode()
            => EqualityComparer<T>.Default.GetHashCode(Value) ^ EqualityComparer<P>.Default.GetHashCode(Parameter) ^ (Culture?.GetHashCode() ?? 0);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ValueConverterArgs<T, P> && Equals((ValueConverterArgs<T, P>)obj);

        /// <inheritdoc />
        public bool Equals(ValueConverterArgs<T, P> other) => this == other;
    }
}