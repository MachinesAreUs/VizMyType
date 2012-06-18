using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;
using VizMyType.ZenSoft.Tools.VizMyType;

namespace VizMyType.Tests
{
    [TestFixture()]
    class CecilTests
    {
        private const String ExamplesAssembly = @"..\..\..\VizMyType.Test.Examples\bin\Debug\VizMyType.Test.Examples.dll";

        [Test]
        public void TestSimpleLambdaIL()
        {
            var type = 
                ModuleDefinition.ReadModule(ExamplesAssembly)
                .Types.Where(t => t.Name == "SimpleClassUsingLinq").First();
            var method = type.Methods.Where(m => m.Name == "ProjectionOverLocalMethod").First();

            Utils.DumpCILInstructionsFor(method);
        }
    }
}
