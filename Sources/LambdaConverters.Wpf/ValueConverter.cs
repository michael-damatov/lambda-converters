using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace LambdaConverters
{
    /// <summary>
    /// A factory class used to create lambda-based instances of the <see cref="IValueConverter"/> interface.
    /// </summary>
    public static class ValueConverter
    {
        abstract class Converter : LambdaConverters.Converter, IValueConverter
        {
            protected Converter(
                ConverterErrorStrategy errorStrategy,
                object defaultInputTypeValue,
                object defaultOutputTypeValue,
                [NotNull] Type inputType,
                [NotNull] Type outputType,
                bool isConvertFunctionAvailable,
                bool isConvertBackFunctionAvailable)
                : base(
                    errorStrategy,
                    defaultInputTypeValue,
                    defaultOutputTypeValue,
                    inputType,
                    outputType,
                    isConvertFunctionAvailable,
                    isConvertBackFunctionAvailable) { }

            protected abstract object ConvertInternal(object value, object parameter, CultureInfo culture);

            protected abstract object ConvertBackInternal(object value, object parameter, CultureInfo culture);

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (!IsConvertFunctionAvailable)
                {
                    EventSource.Log.MissingConvertFunction("convertFunction", ErrorStrategy.ToString());

                    return GetErrorValue(DefaultOutputTypeValue);
                }

                if (targetType != null)
                {
                    if (!targetType.IsAssignableFrom(OutputType))
                    {
                        EventSource.Log.NonAssignableTargetType(targetType.Name, OutputType.Name, ErrorStrategy.ToString());

                        return GetErrorValue(DefaultOutputTypeValue);
                    }
                }
                else
                {
                    EventSource.Log.NonRequestedTargetType();
                }

                return ConvertInternal(value, parameter, culture);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (!IsConvertBackFunctionAvailable)
                {
                    EventSource.Log.MissingConvertBackFunction("convertBackFunction", ErrorStrategy.ToString());

                    return GetErrorValue(DefaultInputTypeValue);
                }

                if (targetType != null)
                {
                    if (!targetType.IsAssignableFrom(InputType))
                    {
                        EventSource.Log.NonAssignableTargetTypeForBackConversion(targetType.Name, InputType.Name, ErrorStrategy.ToString());

                        return GetErrorValue(DefaultInputTypeValue);
                    }
                }
                else
                {
                    EventSource.Log.NonRequestedTargetType();
                }

                return ConvertBackInternal(value, parameter, culture);
            }
        }

        sealed class Converter<I, O> : Converter
        {
            readonly Func<ValueConverterArgs<I>, O> convertFunction;
            readonly Func<ValueConverterArgs<O>, I> convertBackFunction;

            internal Converter(
                Func<ValueConverterArgs<I>, O> convertFunction,
                Func<ValueConverterArgs<O>, I> convertBackFunction,
                ConverterErrorStrategy errorStrategy)
                : base(errorStrategy, default(I), default(O), typeof(I), typeof(O), convertFunction != null, convertBackFunction != null)
            {
                this.convertFunction = convertFunction;
                this.convertBackFunction = convertBackFunction;
            }

            protected override object ConvertInternal(object value, object parameter, CultureInfo culture)
            {
                if (parameter != null)
                {
                    EventSource.Log.ParameterInParameterlessConverter(parameter.GetType().Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(O));
                }

                I inputValue;
                try
                {
                    inputValue = (I)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToInputType(value?.GetType().Name ?? "null", typeof(I).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(O));
                }

                Debug.Assert(convertFunction != null);

                return convertFunction(new ValueConverterArgs<I>(inputValue, culture));
            }

            protected override object ConvertBackInternal(object value, object parameter, CultureInfo culture)
            {
                if (parameter != null)
                {
                    EventSource.Log.ParameterInParameterlessConverterForBackConversion(parameter.GetType().Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(I));
                }

                O inputValue;
                try
                {
                    inputValue = (O)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToOutputType(value?.GetType().Name ?? "null", typeof(O).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(I));
                }

                Debug.Assert(convertBackFunction != null);

                return convertBackFunction(new ValueConverterArgs<O>(inputValue, culture));
            }
        }

        sealed class Converter<I, O, P> : Converter
        {
            readonly Func<ValueConverterArgs<I, P>, O> convertFunction;
            readonly Func<ValueConverterArgs<O, P>, I> convertBackFunction;

            internal Converter(
                Func<ValueConverterArgs<I, P>, O> convertFunction,
                Func<ValueConverterArgs<O, P>, I> convertBackFunction,
                ConverterErrorStrategy errorStrategy)
                : base(errorStrategy, default(I), default(O), typeof(I), typeof(O), convertFunction != null, convertBackFunction != null)
            {
                this.convertFunction = convertFunction;
                this.convertBackFunction = convertBackFunction;
            }

            protected override object ConvertInternal(object value, object parameter, CultureInfo culture)
            {
                P parameterValue;
                try
                {
                    parameterValue = (P)parameter;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToParameterType(parameter?.GetType().Name ?? "null", typeof(P).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(O));
                }

                I inputValue;
                try
                {
                    inputValue = (I)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToInputType(value?.GetType().Name ?? "null", typeof(I).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(O));
                }

                Debug.Assert(convertFunction != null);

                return convertFunction(new ValueConverterArgs<I, P>(inputValue, parameterValue, culture));
            }

            protected override object ConvertBackInternal(object value, object parameter, CultureInfo culture)
            {
                P parameterValue;
                try
                {
                    parameterValue = (P)parameter;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToParameterTypeForBackConversion(
                        parameter?.GetType().Name ?? "null",
                        typeof(P).Name,
                        ErrorStrategy.ToString());

                    return GetErrorValue(default(I));
                }

                O inputValue;
                try
                {
                    inputValue = (O)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToOutputType(value?.GetType().Name ?? "null", typeof(O).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(I));
                }

                Debug.Assert(convertBackFunction != null);

                return convertBackFunction(new ValueConverterArgs<O, P>(inputValue, parameterValue, culture));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IValueConverter" /> interface.
        /// </summary>
        /// <typeparam name="I">The input value type.</typeparam>
        /// <typeparam name="O">The output value type.</typeparam>
        /// <param name="convertFunction">The <see cref="IValueConverter.Convert" /> method.</param>
        /// <param name="convertBackFunction">The <see cref="IValueConverter.ConvertBack" /> method.</param>
        /// <param name="errorStrategy">The error strategy.</param>
        /// <returns>An <see cref="IValueConverter" /> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="errorStrategy"/> is not a valid <see cref="ConverterErrorStrategy"/> value.
        /// </exception>
        [Pure]
        [NotNull]
        public static IValueConverter Create<I, O>(
            Func<ValueConverterArgs<I>, O> convertFunction = null,
            Func<ValueConverterArgs<O>, I> convertBackFunction = null,
            ConverterErrorStrategy errorStrategy = ConverterErrorStrategy.ReturnDefaultValue)
        {
            switch (errorStrategy)
            {
                case ConverterErrorStrategy.ReturnDefaultValue:
                case ConverterErrorStrategy.UseFallbackOrDefaultValue:
                case ConverterErrorStrategy.DoNothing:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorStrategy));
            }

            return new Converter<I, O>(convertFunction, convertBackFunction, errorStrategy);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IValueConverter" /> interface.
        /// </summary>
        /// <typeparam name="I">The input value type.</typeparam>
        /// <typeparam name="O">The output value type.</typeparam>
        /// <typeparam name="P">The converter parameter type.</typeparam>
        /// <param name="convertFunction">The <see cref="IValueConverter.Convert" /> method.</param>
        /// <param name="convertBackFunction">The <see cref="IValueConverter.ConvertBack" /> method.</param>
        /// <param name="errorStrategy">The error strategy.</param>
        /// <returns>An <see cref="IValueConverter" /> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="errorStrategy"/> is not a valid <see cref="ConverterErrorStrategy"/> value.
        /// </exception>
        [Pure]
        [NotNull]
        public static IValueConverter Create<I, O, P>(
            Func<ValueConverterArgs<I, P>, O> convertFunction = null,
            Func<ValueConverterArgs<O, P>, I> convertBackFunction = null,
            ConverterErrorStrategy errorStrategy = ConverterErrorStrategy.ReturnDefaultValue)
        {
            switch (errorStrategy)
            {
                case ConverterErrorStrategy.ReturnDefaultValue:
                case ConverterErrorStrategy.UseFallbackOrDefaultValue:
                case ConverterErrorStrategy.DoNothing:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorStrategy));
            }

            return new Converter<I, O, P>(convertFunction, convertBackFunction, errorStrategy);
        }
    }
}