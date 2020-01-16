using System;
using System.Collections.Generic;
using System.Globalization;

namespace LambdaConverters
{
    partial struct ValidationRuleArgs<T> : IEquatable<ValidationRuleArgs<T>>
    {
        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValidationRuleArgs<T> x, ValidationRuleArgs<T> y)
            => EqualityComparer<T>.Default.Equals(x.Value, y.Value) && EqualityComparer<CultureInfo?>.Default.Equals(x.Culture, y.Culture);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValidationRuleArgs<T> x, ValidationRuleArgs<T> y) => !(x == y);

        /// <inheritdoc />
        public override int GetHashCode()
            => (Value is { } ? EqualityComparer<T>.Default.GetHashCode(Value) : 0) ^
                (Culture != null ? EqualityComparer<CultureInfo>.Default.GetHashCode(Culture) : 0);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ValidationRuleArgs<T> args && Equals(args);

        /// <inheritdoc />
        public bool Equals(ValidationRuleArgs<T> other) => this == other;
    }
}