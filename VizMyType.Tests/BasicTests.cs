using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ZenSoft.Tools.VizMyTypes;

namespace VizMyType.Tests
{
    [TestFixture()]
    class BasicTests
    {
        private const String ExamplesAssembly = @"..\..\..\VizMyType.Test.Examples\bin\Debug\VizMyType.Test.Examples.dll";

        [Test()]
        public void TestClassA()
        {
            var graphStr = (string)TypeGraphExplorer
                .FromAssembly(ExamplesAssembly)
                .WithTypeFilter(name => name == "VizMyType.Test.Examples.SimpleClass")
                .UsingBuilder(new StringDotGraphBuilder())
                .BuildGraph();

            Console.WriteLine(graphStr);
            File.WriteAllText(@"..\..\out\SimpleClass.dot", graphStr);
        }

        [Test()]
        public void TestClasses_AB()
        {
            var graphStr = (string)TypeGraphExplorer
                .FromAssembly(ExamplesAssembly)
                .WithTypeFilter(name => true)
                .UsingBuilder(new StringDotGraphBuilder())
                .BuildGraph();

            Console.WriteLine(graphStr);
            File.WriteAllText(@"..\..\out\SimpleClassWithClient.dot", graphStr);
        }

        [Test()]
        public void TestClassesWithLambdas()
        {
            var graphStr = (string)TypeGraphExplorer
                .FromAssembly(ExamplesAssembly)
                //.WithTypeFilter(x => x == "VizMyType.Test.Examples.SimpleClassUsingLinq")
                .WithTypeFilter(x => true)
                .UsingBuilder(new StringDotGraphBuilder())
                .BuildGraph();

            Console.WriteLine(graphStr);
            File.WriteAllText(@"..\..\out\SimpleClassWithLambdas.dot", graphStr);
        }
    }
}
