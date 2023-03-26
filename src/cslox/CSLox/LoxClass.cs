namespace CSLox
{
    internal class LoxClass : ILoxCallable
    {
        public string Name { get; }

        public LoxClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public int Arity()
        {
            return 0;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            return instance;
        }
    }
}
