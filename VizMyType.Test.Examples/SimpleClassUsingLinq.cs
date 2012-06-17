using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VizMyType.Test.Examples
{
    class SimpleClassUsingLinq
    {
        public int IncrInt(int i)
        {
            return i + 1;
        }

        public int ProjectOverStaticMethod()
        {
            return new int[10].Select( SimpleClass.PublicStaticMethod ).Sum();
        }

        public int ProjectOverLocalMethod()
        {
            return new int[10].Select( IncrInt ).Sum();
        }

    }
}
