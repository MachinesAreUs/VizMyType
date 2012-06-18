using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace VizMyType
{
    public class GraphVizBuilder : IDependencyStructureGraphBuilder
    {
        private DotDependencyStructureGraphBuilder _delegate = new DotDependencyStructureGraphBuilder();
        private readonly string _outputFileName;

        public GraphVizBuilder(string outputFileName)
        {
            ValidateFileNameExtension(outputFileName);
            _outputFileName = outputFileName;
        }

        private void ValidateFileNameExtension(string outputFileName)
        {
            var validExts = new[] {"png", "pdf", "gif", "ps"}; //TODO Complete file extension list
            if (!validExts.Contains(getExtension(outputFileName)))
            {
                throw new ArgumentException("Invalid fileneme extension");
            }
        }

        private string getExtension(string outputFileName)
        {
            return outputFileName.Substring(outputFileName.LastIndexOf(".")+1).ToLower();
        }

        public void OpenTypeSubGraph(TypeDefinition type)
        {
            _delegate.OpenTypeSubGraph(type);
        }

        public void CloseTypeSubGraph()
        {
            _delegate.CloseTypeSubGraph();
        }

        public void AddField(FieldDefinition field)
        {
            _delegate.AddField(field);
        }

        public void AddMethod(MethodDefinition method)
        {
            _delegate.AddMethod(method);
        }

        public void AddFieldReference(MethodDefinition method, FieldReference field)
        {
            _delegate.AddFieldReference(method, field);
        }

        public void AddExternalFieldReference(MethodDefinition method, FieldReference attr)
        {
           _delegate.AddExternalFieldReference(method, attr);
        }

        public void AddMethodReference(MethodReference from, MethodReference to)
        {
            _delegate.AddMethodReference(from, to);
        }

        public void EndGraph()
        {
            _delegate.EndGraph();
        }

        public object BuildGraph()
        {
            var graphStr = (string)_delegate.BuildGraph();
            const string temporalFilePath = @".\temp.dot";  //TODO Use a real temporal file
            
            File.WriteAllText(temporalFilePath, graphStr);

            var process = Process.Start( new ProcessStartInfo
            {
                FileName = "dot.exe",
                UseShellExecute = false,
                Arguments = "-T" + getExtension(_outputFileName)+ " -o " + _outputFileName + " " + temporalFilePath
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception("GraphViz did not completed successfully. Exit code: " + process.ExitCode + 
                                    ". Maybe you should double check it is on your PATH");
            }

            return _outputFileName;
        }
    }
}
