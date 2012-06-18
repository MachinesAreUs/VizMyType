using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VizMyType.Builders;
using VizMyType.Core;

namespace VizMyType.Tests
{
    [TestFixture]
    class GraphVizBuilderTests
    {
        private const String ExamplesAssembly = @"..\..\..\VizMyType.Test.Examples\bin\Debug\VizMyType.Test.Examples.dll";

        [Test]
        public void TestPNGOutput()
        {
            var graphStr = (string)TypeExplorer
                .FromAssembly(ExamplesAssembly)
                .WithTypeFilter(name => name == "VizMyType.Test.Examples.SimpleClass")
                .UsingBuilder(new GraphVizBuilder(@"..\..\out\GraphVizPNG.png"))
                .BuildGraph();

            Assert.AreEqual(@"..\..\out\GraphVizPNG.png", graphStr);
        }

        [Test]
        public void TestPDFOutput()
        {
            var graphStr = (string)TypeExplorer
                .FromAssembly(ExamplesAssembly)
                .WithTypeFilter(name => name == "VizMyType.Test.Examples.SimpleClass")
                .UsingBuilder(new GraphVizBuilder(@"..\..\out\GraphVizPDF.pdf"))
                .BuildGraph();

            Assert.AreEqual(@"..\..\out\GraphVizPDF.pdf", graphStr);
        }
    }
}
