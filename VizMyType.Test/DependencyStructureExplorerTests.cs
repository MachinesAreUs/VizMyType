using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;
using VizMyType.Core;
using VizMyType.Test.Examples;

namespace VizMyType.Tests
{
    [TestFixture]
    class DependencyStructureExplorerTests
    {
        private const String ExamplesAssembly = @"..\..\..\VizMyType.Test.Examples\bin\Debug\VizMyType.Test.Examples.dll";

        [Test()]
        public void TestFindMethodsUsedBy_OneReferencedLocalMethod()
        {
            var method = GetMethodReference(typeof(SimpleClass).FullName, "PublicMethod");

            var usedMethods = DependencyStructureExplorer.FindMethodsUsedBy(method);

            Assert.AreEqual(1, usedMethods.Count());
            Assert.AreEqual("PrivateMethod", usedMethods.First().Name);
        }

        [Test]
        public void TestFindMethodsUsedBy_TwoReferencedLocalMethods()
        {
            var method = GetMethodReference(typeof(SimpleClass).FullName, "PublicMethodUsingTwoOtherMethods");

            var usedMethods = DependencyStructureExplorer.FindMethodsUsedBy(method);

            Assert.AreEqual(2, usedMethods.Count());
            Assert.That(usedMethods.Any(m => m.Name == "PrivateMethod"));
            Assert.That(usedMethods.Any(m => m.Name == "ProtectedMethod"));
        }

        [Test]
        public void TestFindMethodsUsedBy_OneLocalMethodAsLambda()
        {
            var method = GetMethodReference(typeof(SimpleClassUsingLinq).FullName, "ProjectionOverLocalMethod");

            var usedMethods = DependencyStructureExplorer.FindMethodsUsedBy(method);

            Assert.That(usedMethods.Any(m => m.Name == "IncrInt"));
        }

        private static MethodDefinition GetMethodReference(string className, string methodName)
        {
            var type = ModuleDefinition.ReadModule(ExamplesAssembly).Types
                .Where(t => t.FullName == className)
                .First();
            Assert.NotNull(type, "Type definition should not be null");

            var method = type.Methods.Where(m => m.Name == methodName).First();
            Assert.NotNull(method, "Method definition should not be null");
            
            return method;
        }
    }
}
