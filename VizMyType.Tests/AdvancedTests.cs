﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ZenSoft.Tools.VizMyTypes;

namespace VizMyType.Tests
{
    [TestFixture()]
    class AdvancedTests
    {
        [Test()]
        public void TestVizMyTypeDLL()
        {
            var graphStr = (string)TypeGraphExplorer
                .FromAssembly(@"..\..\..\VizMyType\bin\Debug\VizMyType.dll")
                .WithTypeFilter(name => true)
                .UsingBuilder(new StringDotGraphBuilder())
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