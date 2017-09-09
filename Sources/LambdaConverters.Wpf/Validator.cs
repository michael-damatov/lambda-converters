using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace LambdaConverters
{
    /// <summary>
    /// A factory class used to create lambda-based instances of the <see cref="ValidationRule"/> class.
    /// </summary>
    public static class Validator
    {
        sealed class Rule<I> : LambdaConverters.Rule
        {
            readonly Func<ValidationRuleArgs<I>, ValidationResult> ruleFunction;

            internal Rule(
                Func<ValidationRuleArgs<I>, ValidationResult> ruleFunction,
                RuleErrorStrategy errorStrategy)
                : base(errorStrategy, default(I), typeof(I), ruleFunction != null)
            {
                this.ruleFunction = ruleFunction;
            }

            ValidationResult ValidateInternal(object item, CultureInfo cultureInfo)
            {
                I inputValue;
                try
                {
                    inputValue = (I)item;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToRuleInputType(item?.GetType().Name ?? "null", typeof(I).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(ValidationResult));
                }

                Debug.Assert(ruleFunction != null);

                return ruleFunction(new ValidationRuleArgs<I>(inputValue, cultureInfo));
            }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                if (!IsRuleFunctionAvailable)
                {
                    EventSource.Log.MissingRuleFunction("ruleFunction", ErrorStrategy.ToString());

                    return GetErrorValue(default(ValidationResult));
                }

                return ValidateInternal(value, cultureInfo);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule" /> class.
        /// </summary>
        /// <typeparam name="I">The value type.</typeparam>
        /// <param name="ruleFunction">The Validate method.</param>
        /// <param name="errorStrategy">The error strategy.</param>
        /// <returns>An <see cref="ValidationRule" /> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="errorStrategy"/> is not a valid <see cref="RuleErrorStrategy"/> value.
        /// </exception>
        [Pure]
        [NotNull]
        public static ValidationRule Create<I>(
            Func<ValidationRuleArgs<I>, ValidationResult> ruleFunction = null,
            RuleErrorStrategy errorStrategy = RuleErrorStrategy.ReturnDefaultValue)
        {
            switch (errorStrategy)
            {
                case RuleErrorStrategy.ReturnDefaultValue:
                case RuleErrorStrategy.ReturnInvalid:
                case RuleErrorStrategy.ReturnValid:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorStrategy));
            }

            return new Rule<I>(ruleFunction, errorStrategy);
        }
    }
}