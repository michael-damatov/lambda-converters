using System.Globalization;

namespace LambdaConverters
{
    /// <summary>
    /// Provides data for parameterized conversion functions.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="P">The parameter type.</typeparam>
    public partial struct ValueConverterArgs<T, P>
    {
        internal ValueConverterArgs(T value, P parameter, CultureInfo culture)
        {
            Value = value;
            Parameter = parameter;
            Culture = culture;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value { get; }

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