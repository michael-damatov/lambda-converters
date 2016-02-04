using System;
using System.Collections.Generic;

namespace LambdaConverters
{
    partial struct ValueConverterArgs<T> : IEquatable<ValueConverterArgs<T>>
    {
        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValueConverterArgs<T> x, ValueConverterArgs<T> y) => Equals(x.Value, y.Value) && Equals(x.Culture, y.Culture);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValueConverterArgs<T> x, ValueConverterArgs<T> y) => !Equals(x.Value, y.Value) || !Equals(x.Culture, y.Culture);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value) ^ (Culture?.GetHashCode() ?? 0);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is ValueConverterArgs<T> && Equals((ValueConverterArgs<T>)obj);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public bool Equals(ValueConverterArgs<T> other) => this == other;
    }
}