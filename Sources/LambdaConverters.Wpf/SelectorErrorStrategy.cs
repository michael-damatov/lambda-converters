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
        ReturnNull,

        /// <summary>
        /// A new and empty data template is returned.
        /// </summary>
        ReturnNewEmptyDataTemplate
    }
}