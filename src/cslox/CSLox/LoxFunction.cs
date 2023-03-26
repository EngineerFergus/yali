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
        private readonly bool _IsInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            _Declaration = declaration;
            _Closure = closure;
            _IsInitializer = isInitializer;
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
                if (_IsInitializer)
                {
                    return _Closure.GetAt(0, "this");
                }
                return returnValue.Value;
            }

            if (_IsInitializer)
            {
                return _Closure.GetAt(0, "this");
            }

            return null;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new Environment(_Closure);
            environment.Define("this", instance);
            return new LoxFunction(_Declaration, environment, _IsInitializer);
        }

        public override string ToString()
        {
            return $"<fn {_Declaration.Name.Lexeme}>";
        }
    }
}
