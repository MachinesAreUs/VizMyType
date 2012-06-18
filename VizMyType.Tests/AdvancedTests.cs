using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VizMyType.Tests
{
    [TestFixture()]
    class AdvancedTests
    {
        [Test()]
        public void TestVizMyTypeDLL()
        {
            var graphStr = (string)TypeExplorer
                .FromAssembly(@"..\..\..\VizMyType\bin\Debug\VizMyType.dll")
                .WithTypeFilter(name => true)
                .UsingBuilder(new DotDependencyStructureGraphBuilder())
                .BuildGraph();

            Console.WriteLine(graphStr);
            File.WriteAllText(@"..\..\out\VizMyType.dot", graphStr);
        }

        private static Func<string, bool> NullTypeFilter()
        {
            return name => true;
        }
    }
}
