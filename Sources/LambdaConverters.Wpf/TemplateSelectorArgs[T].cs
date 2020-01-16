using System.Windows;

namespace LambdaConverters
{
    /// <summary>
    /// Provides data for selecting data templates.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public partial struct TemplateSelectorArgs<T>
    {
        internal TemplateSelectorArgs(T item, DependencyObject? container)
        {
            Item = item;
            Container = container;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public DependencyObject? Container { get; }
    }
}