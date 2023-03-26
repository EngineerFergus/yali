namespace CSLox
{
    internal class LoxInstance
    {
        private LoxClass _Klass;
        private readonly Dictionary<string, object?> _Fields = new Dictionary<string, object?>();

        public LoxInstance(LoxClass klass)
        {
            _Klass = klass;
        }

        public object? Get(Token name)
        {
            if (_Fields.ContainsKey(name.Lexeme))
            {
                return _Fields[name.Lexeme];
            }

            LoxFunction? method = _Klass.FindMethod(name.Lexeme);
            if (method != null) { return method.Bind(this); }

            throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
        }

        public void Set(Token name, object? value)
        {
            if (_Fields.ContainsKey(name.Lexeme))
            {
                _Fields[name.Lexeme] = value;
            }
            else
            {
                _Fields.Add(name.Lexeme, value);
            }
        }

        public override string ToString()
        {
            return $"{_Klass.Name} instance";
        }
    }
}
