namespace CSLox
{
    internal class LoxClass : ILoxCallable
    {
        public string Name { get; }
        private readonly Dictionary<string, LoxFunction> _Methods;
        private readonly LoxClass? _Superclass;

        public LoxClass(string name, LoxClass? superclass, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _Methods = methods;
            _Superclass = superclass;
        }

        public LoxFunction? FindMethod(string name)
        {
            if (_Methods.ContainsKey(name))
            {
                return _Methods[name];
            }

            if (_Superclass != null)
            {
                return _Superclass.FindMethod(name);
            }

            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public int Arity()
        {
            LoxFunction? initializer = FindMethod("init");
            if (initializer == null) { return 0; }
            return initializer.Arity();
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction? initializer = FindMethod("init");

            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }
    }
}
