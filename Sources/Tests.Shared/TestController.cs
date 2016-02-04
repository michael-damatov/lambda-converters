using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Shared
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public sealed class TestController
    {
        sealed class AssertListener : TraceListener
        {
            static int isDisabled; // 0 -> false, 1 -> true

            static bool IsActive => Interlocked.CompareExchange(ref isDisabled, 0, 0) == 0;

            public override void Fail(string message)
            {
                if (IsActive)
                {
                    throw new AssertFailedException("In-code assertion failed " + message);
                }
            }

            public override void Fail(string message, string detailMessage)
            {
                if (IsActive)
                {
                    throw new AssertFailedException("In-code assertion failed " + message + Environment.NewLine + Environment.NewLine + detailMessage);
                }
            }

            public override void Write(string message) { }

            public override void WriteLine(string message) { }
        }

        [AssemblyInitialize]
        public static void InitializeAssembly(TestContext context)
        {
            foreach (var listener in Debug.Listeners.OfType<DefaultTraceListener>().ToList())
            {
                Debug.Listeners.Remove(listener);
            }

            Debug.Listeners.Add(new AssertListener());
        }
    }
}