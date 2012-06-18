using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace VizMyType.ZenSoft.Tools.VizMyType
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
