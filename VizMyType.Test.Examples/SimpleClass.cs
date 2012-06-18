using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VizMyType.Test.Examples
{
    public class SimpleClass
    {
        public int _publicField;
        protected int _protectedField;
        private int _privateField;
        private static int _staticPrivateField;

        public SimpleClass()
        {
            _publicField = 0;
            _privateField = 0;
            _protectedField = 0;
        }

        public void PublicMethod()
        {
            _privateField++;
            PrivateMethod();
        }
        protected void ProtectedMethod()
        {
            _protectedField++;
            PrivateMethod();
        }

        private void PrivateMethod()
        {
            _privateField++;
        }

        public static int PublicStaticMethod(int i)
        {
            return (_staticPrivateField += i);
        }

        public void PublicMethodUsingTwoOtherMethods()
        {
            PrivateMethod();
            ProtectedMethod();
        }
    }
}
