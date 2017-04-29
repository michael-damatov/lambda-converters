using System;
using System.Windows;
using JetBrains.Annotations;

namespace LambdaConverters
{
    internal abstract class Selector : System.Windows.Controls.DataTemplateSelector
    {
        internal Selector(
            SelectorErrorStrategy errorStrategy,
            object defaultInputTypeValue,
            [NotNull] Type inputType,
            bool isConvertFunctionAvailable)
        {
            ErrorStrategy = errorStrategy;
            DefaultInputTypeValue = defaultInputTypeValue;
            InputType = inputType;
            IsSelectFunctionAvailable = isConvertFunctionAvailable;
        }

        internal SelectorErrorStrategy ErrorStrategy { get; }

        internal object DefaultInputTypeValue { get; }

        [NotNull]
        internal Type InputType { get; }

        internal bool IsSelectFunctionAvailable { get; }

        [Pure]
        internal DataTemplate GetErrorValue(DataTemplate defaultValue)
        {
            switch (ErrorStrategy)
            {
                case SelectorErrorStrategy.ReturnDefaultValue:
                    return defaultValue;

                case SelectorErrorStrategy.ReturnNewEmptyDataTemplate:
                    return new DataTemplate();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}