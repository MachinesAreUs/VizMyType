Welcome!


VizMyType is a tool for visualizing your type's internal structure and dependencies.
It's inspired by what Michael Feathers called 'Feature Diagrams' in
his landmark book [Working Effectively with Legacy Code](http://www.amazon.com/Working-Effectively-Legacy-Michael-Feathers/dp/0131177052).

![SimpleClass](./VizMyType/raw/master/VizMyType.Tests/out/SimpleClass.png)

Since there's [another kind of software diagrams](http://en.wikipedia.org/wiki/Feature_model) wich hold the name "Feature Diagrams" and my 
thought is that it is much more appropiate for them, I'll call the former: Dependency Structure Diagrams.

Basically, a dependency structure diagram is a graph depicting the dependencies between methods and attributes from a set of classes or types.
They are very usefull for exploring the internals of a system, specially legacy ones.

For example, the above diagram was generated from this class:

    class SimpleClass
    {
        public int _publicField;
        protected int _protectedField;
        private int _privateField;
        private static int _staticPrivateField;

        public SimpleClass()
        {
            _publicField = _privateField = 0;
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
    }


and it was done with something as simple as

    TypeGraphExplorer.FromAssembly("MyAssembly")
                     .WithTypeFilter(name => name == "VizMyType.Test.Examples.SimpleClass")
                     .UsingBuilder(new GraphVizBuilder("SimpleClass.png"))
                     .BuildGraph();
					 
¿Want to know more? Checkout the [wiki](https://github.com/MachinesAreUs/VizMyType/wiki)!
