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
            [ContractAnnotation("targetTypes: null => null; targetTypes: notnull => notnull")]
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
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The {0} is null, conversion result is a value according to the specified error strategy ({1}).",
                            "convertFunction",
                            ErrorStrategy));

                    return GetErrorValue(DefaultOutputTypeValue);
                }

                if (targetType != null)
                {
                    if (!targetType.IsAssignableFrom(OutputType))
                    {
                        Diagnostics.TraceSource.TraceEvent(
                            TraceEventType.Warning,
                            0,
                            string.Format(
                                "The requested target type ({0}) is not assignable from the specified output type ({1}), " +
                                "conversion result is a value according to the specified error strategy ({2}).",
                                targetType.Name,
                                OutputType.Name,
                                ErrorStrategy));

                        return GetErrorValue(DefaultOutputTypeValue);
                    }
                }
                else
                {
                    Diagnostics.TraceSource.TraceEvent(TraceEventType.Information, 0, "The target type is not requested.");
                }

                if (values == null)
                {
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The provided values are null, conversion result is a value according to the specified error strategy ({0}).",
                            ErrorStrategy));

                    return GetErrorValue(DefaultOutputTypeValue);
                }

                return ConvertInternal(values, parameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (!IsConvertBackFunctionAvailable)
                {
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The {0} is null, back conversion result is a value according to the specified error strategy ({1}).",
                            "convertBackFunction",
                            ErrorStrategy));

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
                                Diagnostics.TraceSource.TraceEvent(
                                    TraceEventType.Warning,
                                    0,
                                    string.Format(
                                        "The requested target type ({0}) at the position {1} is not assignable from the specified input type ({2}), " +
                                        "back conversion result is a value according to the specified error strategy ({3}).",
                                        targetType.Name,
                                        i,
                                        InputType.Name,
                                        ErrorStrategy));

                                return GetErrorValues(DefaultInputTypeValue, targetTypes);
                            }
                        }
                        else
                        {
                            Diagnostics.TraceSource.TraceEvent(
                                TraceEventType.Information,
                                0,
                                string.Format("The target type at the position {0} is not requested.", i));
                        }
                    }
                }
                else
                {
                    Diagnostics.TraceSource.TraceEvent(TraceEventType.Information, 0, "The target types are not requested.");
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
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "A conversion parameter ({0}) is provided, use the appropriate converter, " +
                            "conversion result is a value according to the specified error strategy ({1}).",
                            parameter.GetType().Name,
                            ErrorStrategy));

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
                        Diagnostics.TraceSource.TraceEvent(
                            TraceEventType.Warning,
                            0,
                            string.Format(
                                "The value ({0}) at the position {1} cannot be cast to the specified input type ({2}), " +
                                "conversion result is a value according to the specified error strategy ({3}).",
                                value?.GetType().Name ?? "null",
                                i,
                                typeof(I).Name,
                                ErrorStrategy));

                        return GetErrorValue(default(O));
                    }
                }

                return convertFunction(new MultiValueConverterArgs<I>(new ReadOnlyCollection<I>(inputValues), culture));
            }

            protected override object[] ConvertBackInternal(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (parameter != null)
                {
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "A conversion parameter ({0}) is provided, use the appropriate converter, " +
                            "back conversion result is a value according to the specified error strategy ({1}).",
                            parameter.GetType().Name,
                            ErrorStrategy));

                    return GetErrorValues(default(I), targetTypes);
                }

                O inputValue;
                try
                {
                    inputValue = (O)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The value ({0}) cannot be cast to the specified output type ({1}), " +
                            "back conversion result is a value according to the specified error strategy ({2}).",
                            value?.GetType().Name ?? "null",
                            typeof(O).Name,
                            ErrorStrategy));

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
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The parameter value ({0}) cannot be cast to the specified parameter type ({1}), " +
                            "conversion result is a value according to the specified error strategy ({2}).",
                            parameter?.GetType().Name ?? "null",
                            typeof(P).Name,
                            ErrorStrategy));

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
                        Diagnostics.TraceSource.TraceEvent(
                            TraceEventType.Warning,
                            0,
                            string.Format(
                                "The value ({0}) at the position {1} cannot be cast to the specified input type ({2}), " +
                                "conversion result is a value according to the specified error strategy ({3}).",
                                value?.GetType().Name ?? "null",
                                i,
                                typeof(I).Name,
                                ErrorStrategy));

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
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The parameter value ({0}) cannot be cast to the specified parameter type ({1}), " +
                            "back conversion result is a value according to the specified error strategy ({2}).",
                            parameter?.GetType().Name ?? "null",
                            typeof(P).Name,
                            ErrorStrategy));

                    return GetErrorValues(default(I), targetTypes);
                }

                O inputValue;
                try
                {
                    inputValue = (O)value;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    Diagnostics.TraceSource.TraceEvent(
                        TraceEventType.Warning,
                        0,
                        string.Format(
                            "The value ({0}) cannot be cast to the specified output type ({1}), " +
                            "back conversion result is a value according to the specified error strategy ({2}).",
                            value?.GetType().Name ?? "null",
                            typeof(O).Name,
                            ErrorStrategy));

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
        /// <param name="convertFunction">The <see cref="Convert" /> method.</param>
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
        /// <param name="convertFunction">The <see cref="Convert" /> method.</param>
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