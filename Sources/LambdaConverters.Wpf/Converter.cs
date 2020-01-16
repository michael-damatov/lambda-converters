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
            object defaultInputTypeValue,
            object defaultOutputTypeValue,
            [NotNull] Type inputType,
            [NotNull] Type outputType,
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

        internal object DefaultInputTypeValue { get; }

        internal object DefaultOutputTypeValue { get; }

        [NotNull]
        internal Type InputType { get; }

        [NotNull]
        internal Type OutputType { get; }

        internal bool IsConvertFunctionAvailable { get; }

        internal bool IsConvertBackFunctionAvailable { get; }

        [Pure]
        internal object GetErrorValue(object defaultValue)
        {
            switch (ErrorStrategy)
            {
                case ConverterErrorStrategy.ReturnDefaultValue:
                    return defaultValue;

                case ConverterErrorStrategy.UseFallbackOrDefaultValue:
                    return DependencyProperty.UnsetValue;

                case ConverterErrorStrategy.DoNothing:
                    return Binding.DoNothing;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}