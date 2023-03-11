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
        private readonly Environment _Closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            _Declaration = declaration;
            _Closure = closure;
        }

        public int Arity()
        {
            return _Declaration.Params.Count;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new(_Closure);
            for (int i = 0; i < _Declaration.Params.Count; i++)
            {
                environment.Define(_Declaration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_Declaration.Body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_Declaration.Name.Lexeme}>";
        }
    }
}
