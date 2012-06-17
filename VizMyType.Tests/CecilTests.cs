using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;

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
            var method = type.Methods.Where(m => m.Name == "ProjectOverStaticMethod").First();

            method.Body.Instructions.ToList().ForEach( i =>
            {
                Console.WriteLine(i.OpCode + ",\t" + i.Operand);
            });
        }

    }
}
