using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Cecil;

namespace VizMyType
{
    public class DotDependencyStructureGraphBuilder : IDependencyStructureGraphBuilder
    {
        private readonly StringWriter _writer = new StringWriter();
        private int _clusterIdx = 0;

        #region Styling

        private readonly Dictionary<FieldAttributes, string> _fieldColors = new Dictionary<FieldAttributes, string>
        {
            {FieldAttributes.Public, "\"#D4FFD4\""},
            {FieldAttributes.Private, "cornflowerblue"},
            {FieldAttributes.Assembly, "lightblue"},
            {FieldAttributes.Family, "lightblue"},
            {FieldAttributes.FamANDAssem, "lightgray"},
            {FieldAttributes.FamORAssem, "lightgray"}
        };
        private readonly Dictionary<MethodAttributes, string> _methodColors = new Dictionary<MethodAttributes, string>
        {
            {MethodAttributes.Public, "\"#D4FFD4\""},
            {MethodAttributes.Private, "cornflowerblue"},
            {MethodAttributes.Assembly, "lightblue"},
            {MethodAttributes.Family, "lightblue"},
            {MethodAttributes.FamANDAssem, "lightgray"},
            {MethodAttributes.FamORAssem, "lightgray"}
        };

        private const string FieldShape = "record";
        private const string MethodShape = "oval";

        private const string IdentUnit = "\t";

        #endregion

        private Dictionary<MethodDefinition, FieldReference> _externalFieldReferences = new Dictionary<MethodDefinition, FieldReference>();

        public DotDependencyStructureGraphBuilder()
        {
            StartGraph();
        }

        private void StartGraph()
        {
            _writer.WriteLine("digraph G {");
            _writer.WriteLine(Ident(1) + "rankdir=LR;");
        }

        public void EndGraph()
        {
            _writer.WriteLine("}");
        }

        public void OpenTypeSubGraph(TypeDefinition type)
        {
            _writer.WriteLine(Ident(1) + "subgraph cluster_" + (_clusterIdx++) + " {");
            _writer.WriteLine(Ident(2) + "fontsize=24; color=\"blue\"");
            _writer.WriteLine(Ident(2) + "label=\"" + type.FullName + "\";");
        }

        public void CloseTypeSubGraph()
        {
            _writer.WriteLine(Ident(1) + "}");
        }

        public void AddField(FieldDefinition field)
        {
            _writer.WriteLine(Ident(2) + "\"" + field.FullName + "\" " + FormatField(field) + ";");
        }

        public void AddMethod(MethodDefinition method)
        {
            _writer.WriteLine(Ident(2) + "\"" + method.FullName + "\" " + FormatMethod(method) + ";");
        }

        public void AddFieldReference(MethodDefinition method, FieldReference attr)
        {
            _writer.WriteLine(Ident(2) + "\"" + method.FullName + "\" -> \"" + attr.FullName + "\";");
        }

        public void AddMethodReference(MethodReference from, MethodReference to)
        {
            _writer.WriteLine(Ident(2) + "\"" + from.FullName + "\" -> \"" + to.FullName + "\";");
        }

        public void AddExternalFieldReference(MethodDefinition method, FieldReference attr)
        {
            // TODO: Why duplication? =\
            if (!_externalFieldReferences.ContainsKey(method))
            {
                _externalFieldReferences.Add(method, attr);                
            }
        }

        public object BuildGraph()
        {
            AddExternalFieldREferences();
            EndGraph();
            return _writer.ToString();
        }

        private void AddExternalFieldREferences()
        {
            foreach (var fieldRef in _externalFieldReferences)
            {
                _writer.WriteLine(Ident(2) + "\"" + fieldRef.Key.FullName + "\" -> \"" + fieldRef.Value.FullName + "\";");
                _writer.WriteLine(Ident(2) + "\"" + fieldRef.Value.FullName + "\" " + FormatExternalField(fieldRef.Value) + ";");
            }
        }

        private string FormatExternalField(FieldReference field)
        {
            return "[" +
                   "label=\"" + field.Name + "\"," + 
                   "shape=" + FieldShape + "," +
                   "style=filled," +
                   "fillcolor=white" + "]";
        }

        private string FormatField(FieldDefinition field)
        {
            return "[" +
                   "label=\"" + field.Name + "\"," +  // TODO Some field names are empty... Sheisse!
                   "shape=" + FieldShape + "," +
                   "style=filled," +
                   "fillcolor=" + _fieldColors[field.Attributes & FieldAttributes.FieldAccessMask] + "]";
        }

        private string FormatMethod(MethodDefinition method)
        {
            return "[" +
                   "label=\"" + method.Name + "\"," +
                   "shape=" + MethodShape + "," +
                   "style=filled," +
                   "fillcolor=" + _methodColors[method.Attributes & MethodAttributes.MemberAccessMask] + "]";
        }

        private string Ident(int i)
        {
            var sb = new StringBuilder();
            for (var x = 1; x <= i; x++)
            {
                sb.Append(IdentUnit);
            }
            return sb.ToString();
        }
    }
}