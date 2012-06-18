using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using VizMyType.ZenSoft.Tools.VizMyType;

namespace ZenSoft.Tools.VizMyTypes
{
    public class TypeGraphExplorer
    {
        private readonly string[] _assemblies;
        private Func<String, bool> _typeFilter;
        private IDependencyStructureGraphBuilder _dependencyStructureGraphBuilder;

        private TypeGraphExplorer(string assemby)
        {
            _assemblies = new string[] { assemby };
        }

        private TypeGraphExplorer(string[] assemblies)
        {
            _assemblies = assemblies;
        }

        public static TypeGraphExplorer FromAssembly(string assemby)
        {
            return new TypeGraphExplorer(assemby);
        }

        public static TypeGraphExplorer FromAssemblies(string[] assemblies)
        {
            return new TypeGraphExplorer(assemblies);
        }

        public TypeGraphExplorer WithTypeFilter(Func<String, bool> typeFilter)
        {
            _typeFilter = CompositeTypeFilter(typeFilter);
            return this;
        }

        private Func<string, bool> CompositeTypeFilter(Func<string, bool> typeFilter)
        {
            return name => typeFilter(name) && !IsInMyBlackList(name);
        }

        private bool IsInMyBlackList(string name)
        {
            return new List<string>
            {
                "System.",
                "Mono.Cecil"
            }.Exists(name.StartsWith);
        }

        public TypeGraphExplorer UsingBuilder(IDependencyStructureGraphBuilder dependencyStructureGraphBuilder)
        {
            _dependencyStructureGraphBuilder = dependencyStructureGraphBuilder;
            return this;
        }

        public object BuildGraph()
        {
            return DependencyStructureExplorer.DumpAssemblyGraph( _assemblies, _typeFilter, _dependencyStructureGraphBuilder);
        }
    }
}