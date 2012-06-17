using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ZenSoft.Tools.VizMyTypes
{
    public class TypeGraphExplorer
    {
        private readonly string[] _assemblies;
        private Func<String, bool> _typeFilter;
        private IGraphBuilder _graphBuilder;

        #region Syntactic sugar


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

        public TypeGraphExplorer UsingBuilder(IGraphBuilder graphBuilder)
        {
            _graphBuilder = graphBuilder;
            return this;
        }

        public object BuildGraph()
        {
            return DumpAssemblyGraph( _assemblies, _typeFilter, _graphBuilder);
        }
        #endregion

        #region Real work is here!

        public object DumpAssemblyGraph(string[] assemblies, Func<string, bool> isInterestingType, IGraphBuilder builder)
        {
            var types = assemblies.ToList().Select(ModuleDefinition.ReadModule).SelectMany(m => m.Types);

            types.Where( t => isInterestingType(t.FullName)).ToList().ForEach( type =>
                GraphType(type, isInterestingType, builder)
            );

            return builder.BuildGraph();
        }

        private void GraphType(TypeDefinition type, Func<string, bool> isInterestingType, IGraphBuilder builder)
        {
            builder.OpenTypeSubGraph(type);
            SelectInterestingFieldsFrom(type).ForEach(builder.AddField);
            SelectInterestingMethodsFrom(type).ForEach(method =>
            {
                builder.AddMethod(method);
                AddFieldReferences4Method(method, isInterestingType, builder);
                AddMethodReferences4Method(method, isInterestingType, builder);
            });
            builder.CloseTypeSubGraph();
        }

        private void AddFieldReferences4Method(MethodDefinition method, Func<string, bool> isInterestingType, IGraphBuilder builder)
        {
            var fields = FindFieldsUsedBy(method);

            //TODO: feature - Declare sub-types as sub-graphs
            var localFields = fields.Where( 
                 a => isInterestingType(a.DeclaringType.FullName) || IsFieldReferenceToSubType(a, method.DeclaringType)
            ).ToList();

            localFields.ForEach(
                f => builder.AddFieldReference(method, f)
            );

            var externalFields = fields.Except(localFields).ToList();
            externalFields.ForEach(
                f => builder.AddExternalFieldReference(method, f)
            );
        }

        private void AddMethodReferences4Method(MethodDefinition method, Func<string, bool> isInterestingType, IGraphBuilder builder)
        {
            var methods = FindMethodsUsedBy(method);

            var localMethods = methods.Where(m => isInterestingType(m.DeclaringType.FullName)).ToList();
            localMethods.ForEach( 
                m => builder.AddMethodReference(method, m)
            );
        }

        private List<MethodReference> FindMethodsUsedBy(MethodDefinition method)
        {
            var methodInvocationCodes = new List<Code> { Code.Call,  Code.Callvirt, Code.Newobj, Code.Ldftn};

            return method.Body.Instructions
                .Where(inst => methodInvocationCodes.Contains(inst.OpCode.Code))
                .Select(inst => inst.Operand)
                .Distinct()
                .Select(inst => (MethodReference) inst)
                .ToList();
        }

        private bool IsFieldReferenceToSubType(FieldReference fieldReference, TypeReference type)
        {
            return fieldReference.FullName.StartsWith(type.FullName);
        }

        private List<FieldDefinition> SelectInterestingFieldsFrom(TypeDefinition type)
        {
            return type.Fields.ToList();
        }

        private List<MethodDefinition> SelectInterestingMethodsFrom(TypeDefinition type)
        {
            return type.Methods
                .Where(method => method.HasBody)
                .GroupBy(m => m.Name)
                .Select(g => g.First()) // This is while I'm using solely the method's name, not the full signature
                .ToList();
        }
        
        // TODO: feature - Distinguish between accesssing and modifying a field
        private List<FieldReference> FindFieldsUsedBy(MethodDefinition method)
        {
            var ldFieldCodes = new List<Code> { Code.Ldfld, Code.Ldflda, Code.Ldsfld, Code.Ldsflda, Code.Stfld, Code.Stsfld };

            return method.Body.Instructions
                .Where(inst => ldFieldCodes.Contains(inst.OpCode.Code))
                .Select(inst => inst.Operand)
                .Distinct()
                .Select(inst => (FieldReference) inst)
                .ToList();
        }

        #endregion
    }
}