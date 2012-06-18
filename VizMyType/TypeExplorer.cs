using System;
using System.Collections.Generic;

namespace VizMyType
{
    public class TypeExplorer
    {
        private readonly string[] _assemblies;
        private Func<String, bool> _typeFilter;
        private IDependencyStructureGraphBuilder _dependencyStructureGraphBuilder;

        private TypeExplorer(string assemby)
        {
            _assemblies = new string[] { assemby };
        }

        private TypeExplorer(string[] assemblies)
        {
            _assemblies = assemblies;
        }

        public static TypeExplorer FromAssembly(string assemby)
        {
            return new TypeExplorer(assemby);
        }

        public static TypeExplorer FromAssemblies(string[] assemblies)
        {
            return new TypeExplorer(assemblies);
        }

        public TypeExplorer WithTypeFilter(Func<String, bool> typeFilter)
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

        public TypeExplorer UsingBuilder(IDependencyStructureGraphBuilder dependencyStructureGraphBuilder)
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