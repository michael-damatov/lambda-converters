using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using LambdaConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Shared;

namespace Tests.LambdaConverters.Wpf
{
    [TestClass]
    public sealed class AssemblyInfoTests
    {
        [JetBrains.Annotations.NotNull]
        static Assembly Assembly => typeof(ValueConverter).Assembly;

        [TestMethod]
        public void _AssemblyCopyright() => AssemblyAssert.AreAttributesValid(Assembly);

        [TestMethod]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void NoDependencies()
            =>
                SequenceAssert.DoesNotContain(
                    from assemblyName in Assembly.GetReferencedAssemblies() select assemblyName.Name,
                    "JetBrains.Annotations");

        [TestMethod]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void AssemblyVersions()
        {
            var assembly = Assembly;

            var assemblyVersion = assembly.GetName().Version;
            Assert.AreEqual(0, assemblyVersion.Revision);

            var fileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

            Assert.AreEqual(assemblyVersion, new Version(fileVersionAttribute.Version));
        }
    }
}