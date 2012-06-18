using System;
using System.Linq;
using Mono.Cecil;

namespace VizMyType.Util
{
    public class Utils
    {
        public static void DumpCILInstructionsFor(MethodDefinition method)
        {
            method.Body.Instructions.ToList().ForEach(
                i => Console.WriteLine(i.OpCode + ",\t" + i.Operand)
            );
        }
    }
}
