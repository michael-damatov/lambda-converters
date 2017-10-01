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
        sealed class Selector<I> : System.Windows.Controls.DataTemplateSelector
        {
            readonly Func<TemplateSelectorArgs<I>, DataTemplate> selectFunction;

            SelectorErrorStrategy ErrorStrategy { get; }

            [Pure]
            DataTemplate GetErrorValue()
            {
                switch (ErrorStrategy)
                {
                    case SelectorErrorStrategy.ReturnNull:
                        return null;

                    case SelectorErrorStrategy.ReturnNewEmptyDataTemplate:
                        return new DataTemplate();

                    default:
                        throw new NotSupportedException();
                }
            }

            internal Selector(
                Func<TemplateSelectorArgs<I>, DataTemplate> selectFunction,
                SelectorErrorStrategy errorStrategy)
            {
                ErrorStrategy = errorStrategy;

                this.selectFunction = selectFunction;
            }

            DataTemplate SelectTemplateInternal(object item, DependencyObject container)
            {
                I inputValue;
                try
                {
                    inputValue = (I)item;
                }
                catch (SystemException e) when (e is InvalidCastException || e is NullReferenceException)
                {
                    EventSource.Log.UnableToCastToItemType(item?.GetType().Name ?? "null", typeof(I).Name, ErrorStrategy.ToString());

                    return GetErrorValue();
                }

                Debug.Assert(selectFunction != null);

                return selectFunction(new TemplateSelectorArgs<I>(inputValue, container));
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (this.selectFunction == null)
                {
                    EventSource.Log.MissingSelectTemplateFunction("selectFunction", ErrorStrategy.ToString());

                    return GetErrorValue();
                }

                return SelectTemplateInternal(item, container);
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
            SelectorErrorStrategy errorStrategy = SelectorErrorStrategy.ReturnNull)
        {
            switch (errorStrategy)
            {
                case SelectorErrorStrategy.ReturnNull:
                case SelectorErrorStrategy.ReturnNewEmptyDataTemplate:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorStrategy));
            }

            return new Selector<I>(selectFunction, errorStrategy);
        }
    }
}