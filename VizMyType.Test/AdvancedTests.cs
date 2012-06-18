using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VizMyType.Tests
{
    [TestFixture]
    class AdvancedTests
    {
        [Test]
        public void TestVizMyTypeDLL()
        {
            const string outputFileName = @"..\..\out\VizMyType.png";

            var graphStr = (string)TypeExplorer
                .FromAssembly(@"..\..\..\VizMyType\bin\Debug\VizMyType.dll")
                .WithTypeFilter(name => true)
                .UsingBuilder(new GraphVizBuilder(outputFileName))
                .BuildGraph();

            Assert.AreEqual(outputFileName, graphStr);
        }
    }
}
