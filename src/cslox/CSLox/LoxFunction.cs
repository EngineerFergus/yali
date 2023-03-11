using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox
{
    internal class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function _Declaration;

        public LoxFunction(Stmt.Function declaration)
        {
            _Declaration = declaration;
        }

        public int Arity()
        {
            return _Declaration.Params.Count;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new(interpreter.Globals);
            for (int i = 0; i < _Declaration.Params.Count; i++)
            {
                environment.Define(_Declaration.Params[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(_Declaration.Body, environment);
            return null;
        }

        public override string ToString()
        {
            return $"<fn {_Declaration.Name.Lexeme}>";
        }
    }
}
