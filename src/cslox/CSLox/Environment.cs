namespace CSLox
{
    public class Environment
    {
        private readonly Dictionary<string, object?> _Values = new();

        public Environment() { }

        public void Define(string name, object? value)
        {
            if (_Values.ContainsKey(name))
            {
                _Values[name] = value;
                return;
            }

            _Values.Add(name, value);
        }

        public object? Get(Token name)
        {
            if (_Values.ContainsKey(name.Lexeme))
            {
                return _Values[name.Lexeme];
            }

            throw new RuntimeError(name, $"Undefined variable \'{name.Lexeme}\'.");
        }

        public void Assign(Token name, object? value)
        {
            if (_Values.ContainsKey(name.Lexeme))
            {
                _Values[name.Lexeme] = value;
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
