namespace CSLox
{
    public class Environment
    {
        private readonly Environment? _Enclosing;
        private readonly Dictionary<string, object?> _Values = new();

        public Environment()
        {
            _Enclosing = null;
        }

        public Environment(Environment Enclosing)
        {
            _Enclosing = Enclosing;
        }

        public void Define(string name, object? value)
        {
            if (_Values.ContainsKey(name))
            {
                _Values[name] = value;
                return;
            }

            _Values.Add(name, value);
        }

        public Environment? Ancestor(int distance)
        {
            Environment? environment = this;
            for (int i = 0; i < distance; i++)
            {
                if (environment == null)
                {
                    throw new Exception("Encountered a null environment while navigating to ancestor.");
                }
                environment = environment._Enclosing;
            }

            return environment;
        }

        public object? GetAt(int distance, string name)
        {
            return Ancestor(distance)?._Values[name];
        }

        public void AssignAt(int distance, Token name, object? value)
        {
            Ancestor(distance)?._Values.Add(name.Lexeme, value);
        }

        public object? Get(Token name)
        {
            if (_Values.ContainsKey(name.Lexeme))
            {
                return _Values[name.Lexeme];
            }

            if (_Enclosing!= null)
            {
                return _Enclosing.Get(name);
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

            if (_Enclosing!= null)
            {
                _Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
