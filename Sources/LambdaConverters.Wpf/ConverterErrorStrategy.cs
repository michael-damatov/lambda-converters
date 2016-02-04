using System.Windows.Data;

namespace LambdaConverters
{
    /// <summary>
    /// Defines the converter error strategy.
    /// </summary>
    public enum ConverterErrorStrategy
    {
        /// <summary>
        /// The default value of the specified target type is returned.
        /// </summary>
        ReturnDefaultValue,

        /// <summary>
        /// No value is returned, but the binding is instructed use the <see cref="BindingBase.FallbackValue"/>, if available, or the default target 
        /// property value.
        /// </summary>
        UseFallbackOrDefaultValue,

        /// <summary>
        /// No value is returned and the binding is instructed not to use the <see cref="BindingBase.FallbackValue"/> or the default target property 
        /// value.
        /// </summary>
        DoNothing,
    }
}