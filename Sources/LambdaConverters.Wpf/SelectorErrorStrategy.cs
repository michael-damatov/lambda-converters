using System.Windows.Controls;
using System.Windows.Data;

namespace LambdaConverters
{
    /// <summary>
    /// Defines the selector error strategy.
    /// </summary>
    public enum SelectorErrorStrategy
    {
        /// <summary>
        /// Null is returned.
        /// </summary>
        ReturnDefaultValue,

        /// <summary>
        /// A new and empty data template is returned.
        /// </summary>
        ReturnNewEmptyDataTemplate
    }
}