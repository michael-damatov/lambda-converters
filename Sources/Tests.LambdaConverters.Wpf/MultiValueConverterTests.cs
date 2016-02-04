using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using LambdaConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Shared;

namespace Tests.LambdaConverters.Wpf
{
    [TestClass]
    public sealed class MultiValueConverterTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void NoFunctions()
        {
            // invalid error strategy
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                () => MultiValueConverter.Create<int, string>(errorStrategy: (ConverterErrorStrategy)int.MaxValue),
                "errorStrategy");

            // with ConverterErrorStrategy.ReturnDefaultValue (default)
            Assert.AreEqual(null, MultiValueConverter.Create<int, string>().Convert(null, null, null, null));
            Assert.AreEqual(null, MultiValueConverter.Create<int, string>().Convert(new object[] { }, null, null, null));
            Assert.AreEqual(null, MultiValueConverter.Create<int, string>().Convert(new object[] { 1, 2 }, null, null, null));
            Assert.AreEqual(0, MultiValueConverter.Create<int, int>().Convert(null, null, null, null));
            Assert.AreEqual(0, MultiValueConverter.Create<int, int>().Convert(new object[] { }, null, null, null));
            Assert.AreEqual(0, MultiValueConverter.Create<int, int>().Convert(new object[] { 1, 2 }, null, null, null));
            Assert.AreEqual(false, MultiValueConverter.Create<int, bool>().Convert(null, null, null, null));
            Assert.AreEqual(false, MultiValueConverter.Create<int, bool>().Convert(new object[] { }, null, null, null));
            Assert.AreEqual(false, MultiValueConverter.Create<int, bool>().Convert(new object[] { 1, 2 }, null, null, null));

            Assert.IsNull(MultiValueConverter.Create<string, int>().ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(new object[] { }, MultiValueConverter.Create<string, int>().ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new object[] { null, null },
                MultiValueConverter.Create<string, int>().ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));
            Assert.IsNull(MultiValueConverter.Create<int, int>().ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(new object[] { }, MultiValueConverter.Create<int, int>().ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new object[] { 0, 0 },
                MultiValueConverter.Create<int, int>().ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));
            Assert.IsNull(MultiValueConverter.Create<bool, int>().ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(new object[] { }, MultiValueConverter.Create<bool, int>().ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new object[] { false, false },
                MultiValueConverter.Create<bool, int>().ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));

            // with ConverterErrorStrategy.UseFallbackOrDefaultValue
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                MultiValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .Convert(null, null, null, null));
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                MultiValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .Convert(new object[] { }, null, null, null));
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                MultiValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .Convert(new object[] { 1, 2 }, null, null, null));

            Assert.IsNull(
                MultiValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(
                new object[] { },
                MultiValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue },
                MultiValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));

            // with ConverterErrorStrategy.DoNothing
            Assert.AreEqual(
                Binding.DoNothing,
                MultiValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.DoNothing).Convert(null, null, null, null));
            Assert.AreEqual(
                Binding.DoNothing,
                MultiValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.DoNothing).Convert(new object[] { }, null, null, null));
            Assert.AreEqual(
                Binding.DoNothing,
                MultiValueConverter.Create<int, string>(errorStrategy: ConverterErrorStrategy.DoNothing)
                    .Convert(new object[] { 1, 2 }, null, null, null));

            Assert.IsNull(MultiValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.DoNothing).ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(
                new object[] { },
                MultiValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.DoNothing).ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new[] { Binding.DoNothing, Binding.DoNothing },
                MultiValueConverter.Create<string, int>(errorStrategy: ConverterErrorStrategy.DoNothing)
                    .ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));
        }

        [TestMethod]
        public void WithConvertFunction()
        {
            // with a wrong target type (use default error strategy)
            Assert.IsNull(MultiValueConverter.Create<int, string>(e => null).Convert(new object[] { 1, 2 }, typeof(bool), null, null));

            // without a target type
            Assert.AreEqual("a", MultiValueConverter.Create<int, string>(e => "a").Convert(new object[] { 1, 2 }, null, null, null));

            // with an unexpected parameter (use default error strategy)
            Assert.IsNull(MultiValueConverter.Create<int, string>(e => null).Convert(new object[] { 1, 2 }, typeof(string), "p", null));

            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(MultiValueConverter.Create<int, string>(e => null).Convert(new object[] { true, null }, typeof(string), null, null));
            Assert.IsNull(MultiValueConverter.Create<int, string>(e => null).Convert(new object[] { null, true }, typeof(string), null, null));
            Assert.IsNull(MultiValueConverter.Create<int, string>(e => null).Convert(new object[] { }, typeof(string), null, null));
            Assert.IsNull(MultiValueConverter.Create<int, string>(e => null).Convert(null, typeof(string), null, null));

            // with a valid input value
            Assert.AreEqual("a", MultiValueConverter.Create<int, string>(e => "a").Convert(new object[] { 1, 2 }, typeof(string), null, null));
            Assert.AreEqual(
                "3",
                MultiValueConverter.Create<int, string>(
                    e =>
                    {
                        SequenceAssert.AreEqual(new[] { 1, 2 }, e.Values);
                        Assert.IsNull(e.Culture);

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2 }, typeof(string), null, null));
            Assert.AreEqual(
                "3",
                MultiValueConverter.Create<int, string>(
                    e =>
                    {
                        SequenceAssert.AreEqual(new[] { 1, 2 }, e.Values);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2 }, typeof(string), null, new CultureInfo("en-GB")));
        }

        [TestMethod]
        public void WithConvertBackFunction()
        {
            // with a wrong target type (use default error strategy)
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(1, new[] { typeof(bool) }, null, null));

            // without a target type
            Assert.IsNull(MultiValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(1, new Type[] { }, null, null));
            Assert.IsNull(
                MultiValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(1, new[] { typeof(string), null }, null, null));
            SequenceAssert.AreEqual(
                new[] { "a", "b" },
                MultiValueConverter.Create<string, int>(convertBackFunction: e => new[] { "a", "b" }).ConvertBack(1, null, null, null));

            // with an unexpected parameter (use default error strategy)
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(1, new[] { typeof(string) }, "p", null));

            // with an input value of an unexpected type (use default error strategy)
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(true, new[] { typeof(string) }, null, null));
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int>(convertBackFunction: e => null).ConvertBack(null, new[] { typeof(string) }, null, null));

            // with a valid input value
            SequenceAssert.AreEqual(
                new[] { "a", "b" },
                MultiValueConverter.Create<string, int>(convertBackFunction: e => new[] { "a", "b" })
                    .ConvertBack(1, new[] { typeof(string), typeof(string) }, null, null));
            SequenceAssert.AreEqual(
                new[] { "1", "1" },
                MultiValueConverter.Create<string, int>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.IsNull(e.Culture);

                        return new[] { e.Value.ToString(), e.Value.ToString() };
                    }).ConvertBack(1, new[] { typeof(string), typeof(string) }, null, null));
            SequenceAssert.AreEqual(
                new[] { "1", "1" },
                MultiValueConverter.Create<string, int>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return new[] { e.Value.ToString(), e.Value.ToString() };
                    }).ConvertBack(1, new[] { typeof(string), typeof(string) }, null, new CultureInfo("en-GB")));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void NoFunctions_UsingConverterParameter()
        {
            // invalid error strategy
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                () => MultiValueConverter.Create<int, string, bool>(errorStrategy: (ConverterErrorStrategy)int.MaxValue),
                "errorStrategy");

            // with ConverterErrorStrategy.ReturnDefaultValue (default)
            Assert.AreEqual(null, MultiValueConverter.Create<int, string, bool>().Convert(null, null, null, null));
            Assert.AreEqual(null, MultiValueConverter.Create<int, string, bool>().Convert(new object[] { }, null, null, null));
            Assert.AreEqual(null, MultiValueConverter.Create<int, string, bool>().Convert(new object[] { 1, 2 }, null, null, null));
            Assert.AreEqual(0, MultiValueConverter.Create<int, int, bool>().Convert(null, null, null, null));
            Assert.AreEqual(0, MultiValueConverter.Create<int, int, bool>().Convert(new object[] { }, null, null, null));
            Assert.AreEqual(0, MultiValueConverter.Create<int, int, bool>().Convert(new object[] { 1, 2 }, null, null, null));
            Assert.AreEqual(false, MultiValueConverter.Create<int, bool, bool>().Convert(null, null, null, null));
            Assert.AreEqual(false, MultiValueConverter.Create<int, bool, bool>().Convert(new object[] { }, null, null, null));
            Assert.AreEqual(false, MultiValueConverter.Create<int, bool, bool>().Convert(new object[] { 1, 2 }, null, null, null));

            Assert.IsNull(MultiValueConverter.Create<string, int, bool>().ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(new object[] { }, MultiValueConverter.Create<string, int, bool>().ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new object[] { null, null },
                MultiValueConverter.Create<string, int, bool>().ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));
            Assert.IsNull(MultiValueConverter.Create<int, int, bool>().ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(new object[] { }, MultiValueConverter.Create<int, int, bool>().ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new object[] { 0, 0 },
                MultiValueConverter.Create<int, int, bool>().ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));
            Assert.IsNull(MultiValueConverter.Create<bool, int, bool>().ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(new object[] { }, MultiValueConverter.Create<bool, int, bool>().ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new object[] { false, false },
                MultiValueConverter.Create<bool, int, bool>().ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));

            // with ConverterErrorStrategy.UseFallbackOrDefaultValue
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                MultiValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .Convert(null, null, null, null));
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                MultiValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .Convert(new object[] { }, null, null, null));
            Assert.AreEqual(
                DependencyProperty.UnsetValue,
                MultiValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .Convert(new object[] { 1, 2 }, null, null, null));

            Assert.IsNull(
                MultiValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(
                new object[] { },
                MultiValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue },
                MultiValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.UseFallbackOrDefaultValue)
                    .ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));

            // with ConverterErrorStrategy.DoNothing
            Assert.AreEqual(
                Binding.DoNothing,
                MultiValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.DoNothing).Convert(null, null, null, null));
            Assert.AreEqual(
                Binding.DoNothing,
                MultiValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.DoNothing)
                    .Convert(new object[] { }, null, null, null));
            Assert.AreEqual(
                Binding.DoNothing,
                MultiValueConverter.Create<int, string, bool>(errorStrategy: ConverterErrorStrategy.DoNothing)
                    .Convert(new object[] { 1, 2 }, null, null, null));

            Assert.IsNull(
                MultiValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.DoNothing).ConvertBack(1, null, null, null));
            SequenceAssert.AreEqual(
                new object[] { },
                MultiValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.DoNothing)
                    .ConvertBack(1, new Type[] { }, null, null));
            SequenceAssert.AreEqual(
                new[] { Binding.DoNothing, Binding.DoNothing },
                MultiValueConverter.Create<string, int, bool>(errorStrategy: ConverterErrorStrategy.DoNothing)
                    .ConvertBack(1, new[] { typeof(int), typeof(string) }, null, null));
        }

        [TestMethod]
        public void WithConvertFunction_UsingConverterParameter()
        {
            // with a wrong target type (use default error strategy)
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(new object[] { 1, 2 }, typeof(bool), true, null));

            // without a target type
            Assert.AreEqual("a", MultiValueConverter.Create<int, string, bool>(e => "a").Convert(new object[] { 1, 2 }, null, true, null));

            // with an unexpected parameter (use default error strategy)
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(new object[] { 1, 2 }, typeof(string), "p", null));
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(new object[] { 1, 2 }, typeof(string), null, null));

            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(new object[] { true, null }, typeof(string), true, null));
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(new object[] { null, true }, typeof(string), true, null));
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(new object[] { }, typeof(string), true, null));
            Assert.IsNull(MultiValueConverter.Create<int, string, bool>(e => null).Convert(null, typeof(string), true, null));

            // with a valid input value
            Assert.AreEqual("a", MultiValueConverter.Create<int, string, bool>(e => "a").Convert(new object[] { 1, 2 }, typeof(string), true, null));
            Assert.AreEqual(
                "3",
                MultiValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        SequenceAssert.AreEqual(new[] { 1, 2 }, e.Values);
                        Assert.AreEqual(true, e.Parameter);
                        Assert.IsNull(e.Culture);

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2 }, typeof(string), true, null));
            Assert.AreEqual(
                "3",
                MultiValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        SequenceAssert.AreEqual(new[] { 1, 2 }, e.Values);
                        Assert.AreEqual(false, e.Parameter);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2 }, typeof(string), false, new CultureInfo("en-GB")));
        }

        [TestMethod]
        public void WithConvertBackFunction_UsingConverterParameter()
        {
            // with a wrong target type (use default error strategy)
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, new[] { typeof(bool) }, true, null));

            // without a target type
            Assert.IsNull(MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, new Type[] { }, true, null));
            Assert.IsNull(
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null)
                    .ConvertBack(1, new[] { typeof(string), null }, true, null));
            SequenceAssert.AreEqual(
                new[] { "a", "b" },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => new[] { "a", "b" }).ConvertBack(1, null, true, null));

            // with an unexpected parameter (use default error strategy)
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, new[] { typeof(string) }, "p", null));
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(1, new[] { typeof(string) }, null, null));

            // with an input value of an unexpected type (use default error strategy)
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(true, new[] { typeof(string) }, true, null));
            SequenceAssert.AreEqual(
                new object[] { null },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => null).ConvertBack(null, new[] { typeof(string) }, true, null));

            // with a valid input value
            SequenceAssert.AreEqual(
                new[] { "a", "b" },
                MultiValueConverter.Create<string, int, bool>(convertBackFunction: e => new[] { "a", "b" })
                    .ConvertBack(1, new[] { typeof(string), typeof(string) }, true, null));
            SequenceAssert.AreEqual(
                new[] { "1", "1" },
                MultiValueConverter.Create<string, int, bool>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(true, e.Parameter);
                        Assert.IsNull(e.Culture);

                        return new[] { e.Value.ToString(), e.Value.ToString() };
                    }).ConvertBack(1, new[] { typeof(string), typeof(string) }, true, null));
            SequenceAssert.AreEqual(
                new[] { "1", "1" },
                MultiValueConverter.Create<string, int, bool>(
                    convertBackFunction: e =>
                    {
                        Assert.AreEqual(1, e.Value);
                        Assert.AreEqual(false, e.Parameter);
                        Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                        return new[] { e.Value.ToString(), e.Value.ToString() };
                    }).ConvertBack(1, new[] { typeof(string), typeof(string) }, false, new CultureInfo("en-GB")));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CompareMultiValueConverterArgs()
        {
            var arg1 = default(MultiValueConverterArgs<int>);
            var arg2 = default(MultiValueConverterArgs<int>);

            Assert.AreEqual(
                "3",
                MultiValueConverter.Create<int, string>(
                    e =>
                    {
                        arg1 = e;

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2 }, typeof(string), null, new CultureInfo("en-GB")));
            Assert.AreEqual(
                "6",
                MultiValueConverter.Create<int, string>(
                    e =>
                    {
                        arg2 = e;

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2, 3 }, typeof(string), null, new CultureInfo("en-GB")));

            var x = default(MultiValueConverterArgs<int>);
            Assert.IsFalse(x.Equals(null));

            Assert.IsFalse(x == arg1);
            Assert.IsFalse(arg1 == x);
            Assert.IsFalse(arg1 == arg2);
            Assert.IsTrue(x != arg1);

#pragma warning disable 1718 // Comparison made to same variable; did you mean to compare something else?
            Assert.IsTrue(x == x);
            Assert.IsFalse(x != x);
#pragma warning restore 1718

            Assert.IsTrue(x.Equals(x as object));

            new HashSet<MultiValueConverterArgs<int>> { x, x, arg1 };
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CompareMultiValueConverterArgs_UsingConverterParameter()
        {
            var arg1 = default(MultiValueConverterArgs<int, bool>);
            var arg2 = default(MultiValueConverterArgs<int, bool>);

            Assert.AreEqual(
                "3",
                MultiValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        arg1 = e;

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2 }, typeof(string), true, new CultureInfo("en-GB")));
            Assert.AreEqual(
                "6",
                MultiValueConverter.Create<int, string, bool>(
                    e =>
                    {
                        arg2 = e;

                        return e.Values.Sum().ToString();
                    }).Convert(new object[] { 1, 2, 3 }, typeof(string), true, new CultureInfo("en-GB")));

            var x = default(MultiValueConverterArgs<int, bool>);
            Assert.IsFalse(x.Equals(null));

            Assert.IsFalse(x == arg1);
            Assert.IsFalse(arg1 == x);
            Assert.IsFalse(arg1 == arg2);
            Assert.IsTrue(x != arg1);

#pragma warning disable 1718 // Comparison made to same variable; did you mean to compare something else?
            Assert.IsTrue(x == x);
            Assert.IsFalse(x != x);
#pragma warning restore 1718

            Assert.IsTrue(x.Equals(x as object));

            new HashSet<MultiValueConverterArgs<int, bool>> { x, x, arg1 };
        }

        [TestMethod]
        public void MultiValueConverterArgs_Defaults()
        {
            Assert.IsNotNull(default(MultiValueConverterArgs<int>).Values);
            Assert.IsNotNull(default(MultiValueConverterArgs<int, bool>).Values);
        }
    }
}