using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using LambdaConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Shared;

namespace Tests.LambdaConverters.Wpf
{
    [TestClass]
    public sealed class ValueConverterTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void NoFunctions()
        {
            // invalid error strategy
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                () => ValueConverter.Create<int, string>(errorStrategy: (ConverterErrorStrategy)int.MaxValue),
                "errorStrategy");

            // with ConverterErrorStrategy.ReturnDefaultValue (default)
            Assert.AreEqual(null, ValueConverter.Create<int, string>().Convert(1, null, null, null));
            Assert.AreEqual(0, ValueConverter.Create<int, int>().Convert(1, null, null, null));
            Assert.AreEqual(false, ValueConverter.Create<int, bool>().Convert(1, null, null, null));
            Assert.AreEqual(null, ValueConverter.Create<string, int>().ConvertBack(1, null, null, null));
            Assert.AreEqual(0, ValueConverter.Create<int, int>().ConvertBack(1, null, null, null));
            Assert.AreEqual(false, ValueConverter.Create<bool, int>().ConvertBack(1, null, null, null));

            // with ConverterErrorStrategy.UseFallbackOrDefaultValue
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                ValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue).Convert(1, null, null, null));
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                ValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue).ConvertBack(1, null, null, null));

            // with ConverterErrorStrategy.DoNothing
            Assert.AreEqual(
                Binding.DoNothing,
                ValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.DoNothing).Convert(1, null, null, null));
            Assert.AreEqual(
                Binding.DoNothing,
                ValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.DoNothing).ConvertBack(1, null, null, null));
        }

        [TestMethod]
        public void WithConvertFunction()
        {
            // with a wrong target type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<int, string>(e => null).Convert(1, typeof(bool), null, null));

            // without a target type
            Assert.AreEqual("a", ValueConverter.Create<int, string>(e => "a").Convert(1, null, null, null));

            // with an unexpected parameter (use default error strategy)
            Assert.IsNull(ValueConverter.Create<int, string>(e => null).Convert(1, typeof(string), "p", null));

            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<int, string>(e => null).Convert(true, typeof(string), null, null));
            Assert.IsNull(ValueConverter.Create<int, string>(e => null).Convert(null, typeof(string), null, null));

            // with a valid input value
            Assert.AreEqual("a", ValueConverter.Create<int, string>(e => "a").Convert(1, typeof(string), null, null));
            Assert.AreEqual("1", ValueConverter.Create<int, string>(
                e =>
                {
                    Assert.AreEqual(1, e.Value);
                    Assert.IsNull(e.Culture);

                    return e.Value.ToString();
                }).Convert(1, typeof(string), null, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<int, string>(
                    e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return e.Value.ToString();
                    }).Convert(1, typeof(string), null, new CultureInfo("en-GB")));
        }

        [TestMethod]
        public void WithConvertBackFunction()
        {
            // with a wrong target type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(1, typeof(bool), null, null));

            // without a target type
            Assert.AreEqual("a", ValueConverter.Create<string, int>(convertBackFunction: e => "a").ConvertBack(1, null, null, null));

            // with an unexpected parameter (use default error strategy)
            Assert.IsNull(ValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(1, typeof(string), "p", null));

            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(true, typeof(string), null, null));
            Assert.IsNull(ValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(null, typeof(string), null, null));

            // with a valid input value
            Assert.AreEqual("a", ValueConverter.Create<string, int>(convertBackFunction: e => "a").ConvertBack(1, typeof(string), null, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<string, int>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.IsNull(e.Culture);

                        return e.Value.ToString();
                    }).ConvertBack(1, typeof(string), null, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<string, int>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return e.Value.ToString();
                    }).ConvertBack(1, typeof(string), null, new CultureInfo("en-GB")));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void NoFunctions_UsingConverterParameter()
        {
            // invalid error strategy
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                () => ValueConverter.Create<int, string, bool>(errorStrategy: (ConverterErrorStrategy)int.MaxValue),
                "errorStrategy");

            // with ConverterErrorStrategy.ReturnDefaultValue (default)
            Assert.AreEqual(null, ValueConverter.Create<int, string, bool>().Convert(1, null, null, null));
            Assert.AreEqual(0, ValueConverter.Create<int, int, bool>().Convert(1, null, null, null));
            Assert.AreEqual(false, ValueConverter.Create<int, bool, bool>().Convert(1, null, null, null));
            Assert.AreEqual(null, ValueConverter.Create<string, int, bool>().ConvertBack(1, null, null, null));
            Assert.AreEqual(0, ValueConverter.Create<int, int, bool>().ConvertBack(1, null, null, null));
            Assert.AreEqual(false, ValueConverter.Create<bool, int, bool>().ConvertBack(1, null, null, null));

            // with ConverterErrorStrategy.UseFallbackOrDefaultValue
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                ValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue).Convert(1, null, null, null));
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                ValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, null, null, null));

            // with ConverterErrorStrategy.DoNothing
            Assert.AreEqual(
                Binding.DoNothing,
                ValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.DoNothing).Convert(1, null, null, null));
            Assert.AreEqual(
                Binding.DoNothing,
                ValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.DoNothing).ConvertBack(1, null, null, null));
        }

        [TestMethod]
        public void WithConvertFunction_UsingConverterParameter()
        {
            // with a wrong target type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<int, string, bool>(e => null).Convert(1, typeof(bool), true, null));

            // without a target type
            Assert.AreEqual("a", ValueConverter.Create<int, string, bool>(e => "a").Convert(1, null, true, null));

            // with an unexpected parameter type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<int, string, bool>(e => null).Convert(1, typeof(string), "p", null));
            Assert.IsNull(ValueConverter.Create<int, string, bool>(e => null).Convert(1, typeof(string), null, null));

            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<int, string, bool>(e => null).Convert(true, typeof(string), true, null));
            Assert.IsNull(ValueConverter.Create<int, string, bool>(e => null).Convert(null, typeof(string), true, null));

            // with a valid input value
            Assert.AreEqual("a", ValueConverter.Create<int, string, bool>(e => "a").Convert(1, typeof(string), true, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(true, e.Parameter);
                        Assert.IsNull(e.Culture);

                        return e.Value.ToString();
                    }).Convert(1, typeof(string), true, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(false, e.Parameter);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return e.Value.ToString();
                    }).Convert(1, typeof(string), false, new CultureInfo("en-GB")));
        }

        [TestMethod]
        public void WithConvertBackFunction_UsingConverterParameter()
        {
            // with a wrong target type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, typeof(bool), true, null));

            // without a target type
            Assert.AreEqual("a", ValueConverter.Create<string, int, bool>(convertBackFunction: e => "a").ConvertBack(1, null, true, null));

            // with an unexpected parameter type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, typeof(string), "p", null));
            Assert.IsNull(ValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, typeof(string), null, null));

            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(ValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(true, typeof(string), true, null));
            Assert.IsNull(ValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(null, typeof(string), true, null));

            // with a valid input value
            Assert.AreEqual("a", ValueConverter.Create<string, int, bool>(convertBackFunction: e => "a").ConvertBack(1, typeof(string), true, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<string, int, bool>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(true, e.Parameter);
                        Assert.IsNull(e.Culture);

                        return e.Value.ToString();
                    }).ConvertBack(1, typeof(string), true, null));
            Assert.AreEqual(
                "1",
                ValueConverter.Create<string, int, bool>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(false, e.Parameter);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return e.Value.ToString();
                    }).ConvertBack(1, typeof(string), false, new CultureInfo("en-GB")));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CompareValueConverterArgs()
        {
            StructAssert.IsCorrect<ValueConverterArgs<int>>();

            var arg = default(ValueConverterArgs<int>);

            Assert.AreEqual(
                "1",
                ValueConverter.Create<int, string>(
                    e =>
                    {
                        arg = e;

                        return e.Value.ToString();
                    }).Convert(1, typeof(string), null, new CultureInfo("en-GB")));

            StructAssert.AreEqual(arg, (x, y) => x == y, (x, y) => x != y);
            StructAssert.AreNotEqual(arg, default, (x, y) => x == y, (x, y) => x != y);

            new HashSet<ValueConverterArgs<int>> { default, arg, arg };
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CompareValueConverterArgs_UsingConverterParameter()
        {
            StructAssert.IsCorrect<ValueConverterArgs<int, bool>>();

            var arg = default(ValueConverterArgs<int, bool>);

            Assert.AreEqual(
                "1",
                ValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        arg = e;

                        return e.Value.ToString();
                    }).Convert(1, typeof(string), true, new CultureInfo("en-GB")));

            StructAssert.AreEqual(arg, (x, y) => x == y, (x, y) => x != y);
            StructAssert.AreNotEqual(arg, default, (x, y) => x == y, (x, y) => x != y);

            new HashSet<ValueConverterArgs<int, bool>> { default, arg, arg };
        }
    }
}