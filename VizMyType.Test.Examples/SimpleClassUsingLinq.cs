using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VizMyType.Test.Examples
{
    public class SimpleClassUsingLinq
    {
        public int IncrInt(int i)
        {
            return i + 1;
        }

        public int ProjectionOverStaticMethod()
        {
            return new int[10].Select( SimpleClass.PublicStaticMethod ).Sum();
        }

        public int ProjectionOverLocalMethod()
        {
            return new int[10].Select( IncrInt ).Sum();
        }
    }
}
