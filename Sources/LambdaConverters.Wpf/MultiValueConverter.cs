using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using JetBrains.Annotations;

namespace LambdaConverters
{
    /// <summary>
    /// A factory class used to create lambda-based instances of the <see cref="IMultiValueConverter"/> interface.
    /// </summary>
    public static class MultiValueConverter
    {
        abstract class Converter : LambdaConverters.Converter, IMultiValueConverter
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

            [Pure]
            [ContractAnnotation("targetTypes: null => null; targetTypes: notnull => notnull", true)]
            internal object[] GetErrorValues(object defaultValue, Type[] targetTypes)
            {
                if (targetTypes != null)
                {
                    var value = GetErrorValue(defaultValue);

                    var result = new object[targetTypes.Length];
                    for (var i = 0; i < result.Length; i++)
                    {
                        result[i] = value;
                    }
                    return result;
                }

                return null;
            }

            protected abstract object ConvertInternal([NotNull] object[] values, object parameter, CultureInfo culture);

            protected abstract object[] ConvertBackInternal(object value, Type[] targetTypes, object parameter, CultureInfo culture);

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
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

                if (values == null)
                {
                    EventSource.Log.NullValues(ErrorStrategy.ToString());

                    return GetErrorValue(DefaultOutputTypeValue);
                }

                return ConvertInternal(values, parameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (!IsConvertBackFunctionAvailable)
                {
                    EventSource.Log.MissingConvertBackFunction("convertBackFunction", ErrorStrategy.ToString());

                    return GetErrorValues(DefaultInputTypeValue, targetTypes);
                }

                if (targetTypes != null)
                {
                    for (var i = 0; i < targetTypes.Length; i++)
                    {
                        var targetType = targetTypes[i];
                        if (targetType != null)
                        {
                            if (!targetType.IsAssignableFrom(InputType))
                            {
                                EventSource.Log.NonAssignableTargetTypeAtPositionForBackConversion(
                                    targetType.Name,
                                    i,
                                    InputType.Name,
                                    ErrorStrategy.ToString());

                                return GetErrorValues(DefaultInputTypeValue, targetTypes);
                            }
                        }
                        else
                        {
                            EventSource.Log.NonRequestedTargetTypeAtPosition(i);
                        }
                    }
                }
                else
                {
                    EventSource.Log.NonRequestedTargetType();
                }

                return ConvertBackInternal(value, targetTypes, parameter, culture);
            }
        }

        sealed class Converter<I, O> : Converter
        {
            readonly Func<MultiValueConverterArgs<I>, O> convertFunction;
            readonly Func<ValueConverterArgs<O>, IEnumerable<I>> convertBackFunction;

            internal Converter(
                Func<MultiValueConverterArgs<I>, O> convertFunction,
                Func<ValueConverterArgs<O>, IEnumerable<I>> convertBackFunction,
                ConverterErrorStrategy errorStrategy)
                : base(errorStrategy, default(I), default(O), typeof(I), typeof(O), convertFunction != null, convertBackFunction != null)
            {
                this.convertFunction = convertFunction;
                this.convertBackFunction = convertBackFunction;
            }

            protected override object ConvertInternal(object[] values, object parameter, CultureInfo culture)
            {
                if (parameter != null)
                {
                    EventSource.Log.ParameterInParameterlessConverter(parameter.GetType().Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(O));
                }

                var inputValues = new I[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    try
                    {
                        inputValues[i] = (I)value;
                    }
                    catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                    {
                        EventSource.Log.UnableToCastAtPositionToInputType(
                            value?.GetType().Name ?? "null",
                            i,
                            typeof(I).Name,
                            ErrorStrategy.ToString());

                        return GetErrorValue(default(O));
                    }
                }

                return convertFunction(new MultiValueConverterArgs<I>(new ReadOnlyCollection<I>(inputValues), culture));
            }

            protected override object[] ConvertBackInternal(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (parameter != null)
                {
                    EventSource.Log.ParameterInParameterlessConverterForBackConversion(
                        parameter.GetType().Name,
                        ErrorStrategy.ToString());

                    return GetErrorValues(default(I), targetTypes);
                }

                O inputValue;
                try
                {
                    inputValue = (O)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToOutputType(value?.GetType().Name ?? "null", typeof(O).Name, ErrorStrategy.ToString());

                    return GetErrorValues(default(I), targetTypes);
                }

                Debug.Assert(convertBackFunction != null);

                var result = convertBackFunction(new ValueConverterArgs<O>(inputValue, culture));
                return result != null ? (from object item in result select item).ToArray() : null;
            }
        }

        sealed class Converter<I, O, P> : Converter
        {
            readonly Func<MultiValueConverterArgs<I, P>, O> convertFunction;
            readonly Func<ValueConverterArgs<O, P>, IEnumerable<I>> convertBackFunction;

            internal Converter(
                Func<MultiValueConverterArgs<I, P>, O> convertFunction,
                Func<ValueConverterArgs<O, P>, IEnumerable<I>> convertBackFunction,
                ConverterErrorStrategy errorStrategy)
                : base(errorStrategy, default(I), default(O), typeof(I), typeof(O), convertFunction != null, convertBackFunction != null)
            {
                this.convertFunction = convertFunction;
                this.convertBackFunction = convertBackFunction;
            }

            protected override object ConvertInternal(object[] values, object parameter, CultureInfo culture)
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

                var inputValues = new I[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    try
                    {
                        inputValues[i] = (I)value;
                    }
                    catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                    {
                        EventSource.Log.UnableToCastAtPositionToInputType(
                            value?.GetType().Name ?? "null",
                            i,
                            typeof(I).Name,
                            ErrorStrategy.ToString());

                        return GetErrorValue(default(O));
                    }
                }

                return convertFunction(new MultiValueConverterArgs<I, P>(new ReadOnlyCollection<I>(inputValues), parameterValue, culture));
            }

            protected override object[] ConvertBackInternal(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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

                    return GetErrorValues(default(I), targetTypes);
                }

                O inputValue;
                try
                {
                    inputValue = (O)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToOutputType(value?.GetType().Name ?? "null", typeof(O).Name, ErrorStrategy.ToString());

                    return GetErrorValues(default(I), targetTypes);
                }

                Debug.Assert(convertBackFunction != null);

                var result = convertBackFunction(new ValueConverterArgs<O, P>(inputValue, parameterValue, culture));
                return result != null ? (from object item in result select item).ToArray() : null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IMultiValueConverter" /> interface.
        /// </summary>
        /// <typeparam name="I">The input value type.</typeparam>
        /// <typeparam name="O">The output value type.</typeparam>
        /// <param name="convertFunction">The <see cref="IValueConverter.Convert" /> method.</param>
        /// <param name="convertBackFunction">The <see cref="IValueConverter.ConvertBack" /> method.</param>
        /// <param name="errorStrategy">The error strategy.</param>
        /// <returns>An <see cref="IMultiValueConverter" /> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="errorStrategy"/> is not a valid <see cref="ConverterErrorStrategy"/> value.
        /// </exception>
        [Pure]
        [NotNull]
        public static IMultiValueConverter Create<I, O>(
            Func<MultiValueConverterArgs<I>, O> convertFunction = null,
            Func<ValueConverterArgs<O>, IEnumerable<I>> convertBackFunction = null,
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
        /// Initializes a new instance of the <see cref="IMultiValueConverter" /> interface.
        /// </summary>
        /// <typeparam name="I">The input value type.</typeparam>
        /// <typeparam name="O">The output value type.</typeparam>
        /// <typeparam name="P">The converter parameter type.</typeparam>
        /// <param name="convertFunction">The <see cref="IValueConverter.Convert" /> method.</param>
        /// <param name="convertBackFunction">The <see cref="IValueConverter.ConvertBack" /> method.</param>
        /// <param name="errorStrategy">The error strategy.</param>
        /// <returns>An <see cref="IMultiValueConverter" /> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="errorStrategy"/> is not a valid <see cref="ConverterErrorStrategy"/> value.
        /// </exception>
        [Pure]
        [NotNull]
        public static IMultiValueConverter Create<I, O, P>(
            Func<MultiValueConverterArgs<I, P>, O> convertFunction = null,
            Func<ValueConverterArgs<O, P>, IEnumerable<I>> convertBackFunction = null,
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