using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Shared
{
    [ExcludeFromCodeCoverage]
    internal static class AssemblyAssert
    {
        static string GetAttributeName<A>() where A : Attribute
        {
            var name = typeof(A).Name;

            return string.Format(
                "[{0}]",
                name.EndsWith(nameof(Attribute), StringComparison.Ordinal) ? name.Remove(name.Length - nameof(Attribute).Length) : name);
        }

        public static void AreAttributesValid(Assembly assembly)
        {
            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            if (copyrightAttribute == null)
            {
                throw new AssertFailedException(
                    string.Format(
                        "Missing the {0} attribute in the {1} assembly.",
                        GetAttributeName<AssemblyCopyrightAttribute>(),
                        Path.GetFileName(assembly.Location)));
            }

            if (copyrightAttribute.Copyright == null)
            {
                throw new AssertFailedException(
                    string.Format(
                        "The '{0}' property of the {1} attribute in the {2} assembly is null.",
                        nameof(AssemblyCopyrightAttribute.Copyright),
                        GetAttributeName<AssemblyCopyrightAttribute>(),
                        Path.GetFileName(assembly.Location)));
            }

            if (copyrightAttribute.Copyright.IndexOf(DateTime.Today.Year.ToString(), StringComparison.CurrentCultureIgnoreCase) == -1)
            {
                throw new AssertFailedException(
                    string.Format(
                        "The '{0}' property of the {1} attribute in the {2} assembly does not contain the current year.",
                        nameof(AssemblyCopyrightAttribute.Copyright),
                        GetAttributeName<AssemblyCopyrightAttribute>(),
                        Path.GetFileName(assembly.Location)));
            }
        }
    }
}