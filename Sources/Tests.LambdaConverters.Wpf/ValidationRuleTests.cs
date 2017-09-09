using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls;
using LambdaConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Shared;

namespace Tests.LambdaConverters.Wpf
{
    [TestClass]
    public sealed class ValidationRuleTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void NoFunctions()
        {
            // invalid error strategy
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                () => Validator.Create<int>(errorStrategy: (RuleErrorStrategy)int.MaxValue),
                "errorStrategy");

            // with ConverterErrorStrategy.ReturnDefaultValue (default)
            Assert.AreEqual(null, Validator.Create<int>().Validate(1, null));

            // with ConverterErrorStrategy.ReturnInvalid
            ValidationResult result = Validator.Create<int>(errorStrategy: RuleErrorStrategy.ReturnInvalid).
                                                 Validate(1, null);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.ErrorContent);

            // with ConverterErrorStrategy.ReturnValid
            result = Validator.Create<int>(errorStrategy: RuleErrorStrategy.ReturnValid).
                                                 Validate(1, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
            Assert.IsNull(result.ErrorContent);
        }

        [TestMethod]
        public void WithRuleFunction()
        {
            // with an input value of an unexpected type (use default error strategy)
            Assert.IsNull(Validator.Create<int>(e => new ValidationResult(false, null)).Validate(true, null));
            Assert.IsNull(Validator.Create<int>(e => new ValidationResult(false, null)).Validate(null, null));

            // with a valid input value
            ValidationResult result = Validator.Create<int>(e => new ValidationResult(true, "a")).Validate(1, null);
            Assert.IsNotNull(result);
            Assert.AreEqual("a", result.ErrorContent);
            result = Validator.Create<int>(
                e =>
                {
                    Assert.AreEqual(1, e.Value);
                    Assert.IsNull(e.Culture);

                    return new ValidationResult(false, e.Value.ToString());
                }).Validate(1, null);
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.ErrorContent);
            result = Validator.Create<int>(
                e =>
                {
                    Assert.AreEqual(1, e.Value);
                    Assert.AreEqual(new CultureInfo("en-GB"), e.Culture);

                    return new ValidationResult(false, e.Value.ToString());
                }).Validate(1, new CultureInfo("en-GB"));
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.ErrorContent);
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CompareValidationRuleArgs()
        {
            StructAssert.IsCorrect<ValidationRuleArgs<int>>();

            var arg = default(ValidationRuleArgs<int>);

            ValidationResult result = Validator.Create<int>(
                e =>
                {
                    arg = e;
                    result = new ValidationResult(true, e.Value.ToString());
                    return result;
                }).Validate(1, new CultureInfo("en-GB"));

            Assert.AreEqual(
                result.ErrorContent,
                "1");

            StructAssert.AreEqual(arg, (x, y) => x == y, (x, y) => x != y);
            StructAssert.AreNotEqual(arg, default(ValidationRuleArgs<int>), (x, y) => x == y, (x, y) => x != y);

            new HashSet<ValidationRuleArgs<int>> { default(ValidationRuleArgs<int>), arg, arg };
        }
    }
}