using System.Diagnostics;
using JetBrains.Annotations;

namespace LambdaConverters
{
    internal static class Diagnostics
    {
        [NotNull]
        internal static readonly TraceSource TraceSource = new TraceSource(nameof(LambdaConverters));
    }
}