using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace LambdaConverters
{
    /// <summary>
    /// A factory class used to create lambda-based instances of the <see cref="DataTemplateSelector"/> class.
    /// </summary>
    public static class TemplateSelector
    {
        abstract class Selector : LambdaConverters.Selector
        {
            protected Selector(
                SelectorErrorStrategy errorStrategy,
                object defaultInputTypeValue,
                [NotNull] Type inputType,
                bool isSelectorFunctionAvailable)
                : base(
                    errorStrategy,
                    defaultInputTypeValue,
                    inputType,
                    isSelectorFunctionAvailable) { }

            protected abstract DataTemplate SelectTemplateInternal(object item, DependencyObject container);

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (!IsSelectFunctionAvailable)
                {
                    EventSource.Log.MissingSelectTemplateFunction("selectFunction", ErrorStrategy.ToString());

                    return GetErrorValue(default(DataTemplate));
                }

                return SelectTemplateInternal(item, container);
            }
        }

        sealed class Selector<I> : Selector
        {
            readonly Func<TemplateSelectorArgs<I>, DataTemplate> selectFunction;

            internal Selector(
                Func<TemplateSelectorArgs<I>, DataTemplate> selectFunction,
                SelectorErrorStrategy errorStrategy)
                : base(errorStrategy, default(I), typeof(I), selectFunction != null)
            {
                this.selectFunction = selectFunction;
            }

            protected override DataTemplate SelectTemplateInternal(object item, DependencyObject container)
            {
                I inputValue;
                try
                {
                    inputValue = (I)item;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToInputType(item?.GetType().Name ?? "null", typeof(I).Name, ErrorStrategy.ToString());

                    return GetErrorValue(default(DataTemplate));
                }

                Debug.Assert(selectFunction != null);

                return selectFunction(new TemplateSelectorArgs<I>(inputValue, container));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateSelector" /> class.
        /// </summary>
        /// <typeparam name="I">The item type.</typeparam>
        /// <param name="selectFunction">The <see cref="DataTemplateSelector.SelectTemplate" /> method.</param>
        /// <param name="errorStrategy">The error strategy.</param>
        /// <returns>An <see cref="DataTemplateSelector" /> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="errorStrategy"/> is not a valid <see cref="SelectorErrorStrategy"/> value.
        /// </exception>
        [Pure]
        [NotNull]
        public static DataTemplateSelector Create<I>(
            Func<TemplateSelectorArgs<I>, DataTemplate> selectFunction = null,
            SelectorErrorStrategy errorStrategy = SelectorErrorStrategy.ReturnDefaultValue)
        {
            switch (errorStrategy)
            {
                case SelectorErrorStrategy.ReturnDefaultValue:
                case SelectorErrorStrategy.ReturnNewEmptyDataTemplate:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorStrategy));
            }

            return new Selector<I>(selectFunction, errorStrategy);
        }
    }
}