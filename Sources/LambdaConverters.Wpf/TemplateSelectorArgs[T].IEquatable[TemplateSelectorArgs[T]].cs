using System;
using System.Collections.Generic;

namespace LambdaConverters
{
    partial struct TemplateSelectorArgs<T> : IEquatable<TemplateSelectorArgs<T>>
    {
        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TemplateSelectorArgs<T> x, TemplateSelectorArgs<T> y) => Equals(x.Item, y.Item) && Equals(x.Container, y.Container);

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TemplateSelectorArgs<T> x, TemplateSelectorArgs<T> y) => !Equals(x.Item, y.Item) || !Equals(x.Container, y.Container);

        /// <inheritdoc />
        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Item) ^ (Container?.GetHashCode() ?? 0);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is TemplateSelectorArgs<T> args && Equals(args);

        /// <inheritdoc />
        public bool Equals(TemplateSelectorArgs<T> other) => this == other;
    }
}