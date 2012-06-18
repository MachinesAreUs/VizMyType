using Mono.Cecil;

namespace VizMyType
{
    public interface IDependencyStructureGraphBuilder
    {
        void OpenTypeSubGraph(TypeDefinition type);

        void CloseTypeSubGraph();

        void AddField(FieldDefinition field);

        void AddMethod(MethodDefinition method);

        void AddFieldReference(MethodDefinition method, FieldReference attr);

        void AddExternalFieldReference(MethodDefinition method, FieldReference attr);

        void EndGraph();

        object BuildGraph();

        void AddMethodReference(MethodReference from, MethodReference to);
    }
}