using System;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace LambdaConverters
{
    internal abstract class Rule : System.Windows.Controls.ValidationRule
    {
        internal Rule(
            RuleErrorStrategy errorStrategy,
            object defaultInputTypeValue,
            [NotNull] Type inputType,
            bool isRuleFunctionAvailable)
        {
            ErrorStrategy = errorStrategy;
            DefaultInputTypeValue = defaultInputTypeValue;
            InputType = inputType;
            IsRuleFunctionAvailable = isRuleFunctionAvailable;
        }

        internal RuleErrorStrategy ErrorStrategy { get; }

        internal object DefaultInputTypeValue { get; }

        [NotNull]
        internal Type InputType { get; }

        internal bool IsRuleFunctionAvailable { get; }

        [Pure]
        internal ValidationResult GetErrorValue(ValidationResult defaultValue)
        {
            switch (ErrorStrategy)
            {
                case RuleErrorStrategy.ReturnDefaultValue:
                    return defaultValue;

                case RuleErrorStrategy.ReturnInvalid:
                    return new ValidationResult(false, null);

                case RuleErrorStrategy.ReturnValid:
                    return new ValidationResult(true, null);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}