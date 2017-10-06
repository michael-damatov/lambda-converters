namespace LambdaConverters
{
    /// <summary>
    /// Defines the validation rule error strategy.
    /// </summary>
    public enum RuleErrorStrategy
    {
        /// <summary>
        /// Null is returned.
        /// </summary>
        ReturnNull,

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