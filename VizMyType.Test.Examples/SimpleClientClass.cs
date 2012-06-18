using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VizMyType.Test.Examples
{
    public class SimpleClientClass
    {
        public void accessOtherTypesPublicMethod()
        {
            var objectA = new SimpleClass();
            objectA.PublicMethod();
        }

        public void accessOtherTypesStaticMethod()
        {
            SimpleClass.PublicStaticMethod(10);
        }
    }
}
