using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Shared
{
    [ExcludeFromCodeCoverage]
    internal static class SequenceAssert
    {
        [Pure]
        static bool AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> equalityComparer, [NotNull] out string reason)
        {
            if (ReferenceEquals(expected, actual))
            {
                reason = "Both collection references point to the same collection object.";
                return true;
            }

            if (expected == null || actual == null)
            {
                reason = string.Format("Expected: ({0}null). Actual: ({1}null).", expected != null ? "not " : "", actual != null ? "not " : "");
                return false;
            }

            var expectedList = expected as IList<T> ?? expected.ToList();
            var actualList = actual as IList<T> ?? actual.ToList();

            if (expectedList.Count != actualList.Count)
            {
                reason = "Different number of elements.";
                return false;
            }

            if (equalityComparer == null)
            {
                equalityComparer = EqualityComparer<T>.Default.Equals;
            }

            using (var expectedEnumerator = expectedList.GetEnumerator())
            {
                using (var actualEnumerator = actualList.GetEnumerator())
                {
                    var index = 0;
                    while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
                    {
                        if (!equalityComparer(expectedEnumerator.Current, actualEnumerator.Current))
                        {
                            reason = string.Format("Elements at index {0} do not match.", index);
                            return false;
                        }
                        index++;
                    }
                }
            }

            reason = "Both collections contain same elements.";
            return true;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static void DoesNotContain<T>([NotNull] IEnumerable<T> sequence, T item, IEqualityComparer<T> equalityComparer = null)
        {
            if (sequence.Contains(item, equalityComparer))
            {
                throw new AssertFailedException(string.Format("{0} failed.", nameof(DoesNotContain)));
            }
        }

        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> equalityComparer = null)
        {
            string reason;
            if (!AreEqual(expected, actual, equalityComparer, out reason))
            {
                throw new AssertFailedException(string.Format("{0} failed. {1}", nameof(AreEqual), reason));
            }
        }
    }
}