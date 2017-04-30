using System.Windows.Controls;

namespace LambdaConverters
{
    /// <summary>
    /// Defines the validation rule error strategy.
    /// </summary>
    public enum RuleErrorStrategy
    {
        /// <summary>
        /// The default of <see cref="ValidationRule" /> is returned.
        /// </summary>
        ReturnDefaultValue,

        /// <summary>
        /// An invalid validation result is returned.
        /// </summary>
        ReturnInvalid,

        /// <summary>
        /// A valid validation result is returned.
        /// </summary>
        ReturnValid
    }
}