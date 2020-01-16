using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using LambdaConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Shared;

namespace Tests.LambdaConverters.Wpf
{
    [TestClass]
    public sealed class TemplateSelectorTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void NoFunctions()
        {
            // invalid error strategy
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                () => TemplateSelector.Create<int>(errorStrategy: (SelectorErrorStrategy)int.MaxValue),
                "errorStrategy");

            // with ConverterErrorStrategy.ReturnDefaultValue (default)
            Assert.AreEqual(null, TemplateSelector.Create<int>().SelectTemplate(1, null));

            // with ConverterErrorStrategy.ReturnNewEmptyDataTemplate
            DataTemplate result = TemplateSelector.Create<int>(errorStrategy: SelectorErrorStrategy.ReturnNewEmptyDataTemplate).SelectTemplate(1, null);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasContent);
        }

        [TestMethod]
        public void WithSelectFunction()
        {
            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(TemplateSelector.Create<int>(e => new DataTemplate()).SelectTemplate(true, null));
            Assert.IsNull(TemplateSelector.Create<int>(e => new DataTemplate()).SelectTemplate(null, null));

            // with a valid input value
            DataTemplate template = new DataTemplate();
            Assert.AreEqual(template, TemplateSelector.Create<int>(
                e =>
                {
                    Assert.AreEqual(1, e.Item);

                    return template;
                }).SelectTemplate(1, null));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CompareTemplateSelectorArgs()
        {
            StructAssert.IsCorrect<TemplateSelectorArgs<int>>();

            var arg = default(TemplateSelectorArgs<int>);

            DataTemplate template = new DataTemplate();
            Assert.AreEqual(
                template,
                TemplateSelector.Create<int>(
                    e =>
                    {
                        arg = e;

                        return template;
                    }).SelectTemplate(1, null));

            StructAssert.AreEqual(arg, (x, y) => x == y, (x, y) => x != y);
            StructAssert.AreNotEqual(arg, default, (x, y) => x == y, (x, y) => x != y);

            new HashSet<TemplateSelectorArgs<int>> { default, arg, arg };
        }
    }
}