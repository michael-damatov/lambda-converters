using System;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace LambdaConverters
{
    internal abstract class Converter
    {
        protected Converter(
            ConverterErrorStrategy errorStrategy,
            object? defaultInputTypeValue,
            object? defaultOutputTypeValue,
            Type inputType,
            Type outputType,
            bool isConvertFunctionAvailable,
            bool isConvertBackFunctionAvailable)
        {
            ErrorStrategy = errorStrategy;
            DefaultInputTypeValue = defaultInputTypeValue;
            DefaultOutputTypeValue = defaultOutputTypeValue;
            InputType = inputType;
            OutputType = outputType;
            IsConvertFunctionAvailable = isConvertFunctionAvailable;
            IsConvertBackFunctionAvailable = isConvertBackFunctionAvailable;
        }

        internal ConverterErrorStrategy ErrorStrategy { get; }

        internal object? DefaultInputTypeValue { get; }

        internal object? DefaultOutputTypeValue { get; }

        internal Type InputType { get; }

        internal Type OutputType { get; }

        internal bool IsConvertFunctionAvailable { get; }

        internal bool IsConvertBackFunctionAvailable { get; }

        [Pure]
        internal object? GetErrorValue(object? defaultValue)
            => ErrorStrategy switch
            {
                ConverterErrorStrategy.ReturnDefaultValue => defaultValue,
                ConverterErrorStrategy.UseFallbackOrDefaultValue => DependencyProperty.UnsetValue,
                ConverterErrorStrategy.DoNothing => Binding.DoNothing,
                _ => throw new NotSupportedException()
            };
    }
}