using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace VizMyType.Core
{
    public class DependencyStructureExplorer
    {
        public static object DumpAssemblyGraph(string[] assemblies, Func<string, bool> isInterestingType, IDependencyStructureGraphBuilder builder)
        {
            var types = assemblies.ToList().Select(ModuleDefinition.ReadModule).SelectMany(m => m.Types);

            types.Where(t => isInterestingType(t.FullName)).ToList().ForEach(type =>
                GraphType(type, isInterestingType, builder)
            );

            return builder.BuildGraph();
        }

        private static void GraphType(TypeDefinition type, Func<string, bool> isInterestingType, IDependencyStructureGraphBuilder builder)
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

        public static void AddFieldReferences4Method(MethodDefinition method, Func<string, bool> isInterestingType, IDependencyStructureGraphBuilder builder)
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

        public static void AddMethodReferences4Method(MethodDefinition method, Func<string, bool> isInterestingType, IDependencyStructureGraphBuilder builder)
        {
            var methods = FindMethodsUsedBy(method);

            var localMethods = methods.Where(m => isInterestingType(m.DeclaringType.FullName)).ToList();
            localMethods.ForEach(
                m => builder.AddMethodReference(method, m)
            );
        }

        public static List<MethodReference> FindMethodsUsedBy(MethodDefinition method)
        {
            var methodInvocationCodes = new List<Code> { Code.Call, Code.Callvirt, Code.Newobj, Code.Ldftn };

            return method.Body.Instructions
                .Where(inst => methodInvocationCodes.Contains(inst.OpCode.Code))
                .Select(inst => inst.Operand)
                .Distinct()
                .Select(inst => (MethodReference)inst)
                .ToList();
        }

        public static bool IsFieldReferenceToSubType(FieldReference fieldReference, TypeReference type)
        {
            return fieldReference.FullName.StartsWith(type.FullName);
        }

        public static List<FieldDefinition> SelectInterestingFieldsFrom(TypeDefinition type)
        {
            return type.Fields.ToList();
        }

        public static List<MethodDefinition> SelectInterestingMethodsFrom(TypeDefinition type)
        {
            return type.Methods
                .Where(method => method.HasBody)
                .GroupBy(m => m.Name)
                .Select(g => g.First()) // This is while I'm using solely the method's name, not the full signature
                .ToList();
        }

        // TODO: feature - Distinguish between accesssing and modifying a field
        public static List<FieldReference> FindFieldsUsedBy(MethodDefinition method)
        {
            var ldFieldCodes = new List<Code> { Code.Ldfld, Code.Ldflda, Code.Ldsfld, Code.Ldsflda, Code.Stfld, Code.Stsfld };

            return method.Body.Instructions
                .Where(inst => ldFieldCodes.Contains(inst.OpCode.Code))
                .Select(inst => inst.Operand)
                .Distinct()
                .Select(inst => (FieldReference)inst)
                .ToList();
        }
    }
}
