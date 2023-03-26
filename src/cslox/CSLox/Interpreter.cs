namespace CSLox
{
    internal class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<Void>
    {
        public readonly Environment Globals = new();
        private Environment _Environment;
        private readonly Dictionary<Expr, int> _Locals = new();

        public Interpreter()
        {
            Globals.Define("clock", new ClockCallable());
            _Environment = Globals;
        }

        public void Interpret(List<Stmt?> statements)
        {
            try
            {
                foreach (var stmt in statements) 
                {
                    if (stmt == null)
                    {
                        throw new RuntimeError(new Token(TokenType.NIL, "nil", null, 0),
                            "Encountered a null statement in interpreter.");
                    }
                    Execute(stmt);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void Resolve(Expr expr, int depth)
        {
            _Locals.Add(expr, depth);
        }

        public void ExecuteBlock(List<Stmt?> statements, Environment environment)
        {
            Environment previous = _Environment;

            try
            {
                _Environment = environment;

                foreach (var stmt in statements)
                {
                    if (stmt == null)
                    {
                        throw new RuntimeError(new Token(TokenType.NIL, "nil", null, 0),
                            "Encountered a null statement in interpreter.");
                    }

                    Execute(stmt);
                }
            }
            finally
            {
                _Environment = previous;
            }
        }

        public Void VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_Environment));
            return new Void();
        }

        public Void VisitClassStmt(Stmt.Class stmt)
        {
            object? superclass = null;
            LoxClass? sups = null;

            if (stmt.Superclass != null)
            {
                superclass = Evaluate(stmt.Superclass);
                if (superclass is not LoxClass s)
                {
                    throw new RuntimeError(stmt.Superclass.Name, "Superclass must be a class.");
                }
                sups = s;
            }

            _Environment.Define(stmt.Name.Lexeme, null);

            if (stmt.Superclass != null)
            {
                _Environment = new Environment(_Environment);
                _Environment.Define("super", superclass);
            }

            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();

            foreach (var method in stmt.Methods)
            {
                LoxFunction function = new LoxFunction(method, _Environment, 
                    method.Name.Lexeme.Equals("init"));
                methods.Add(method.Name.Lexeme, function);
            }

            LoxClass klass = new(stmt.Name.Lexeme, sups, methods);

            if (superclass != null)
            {
                if (_Environment._Enclosing == null)
                {
                    throw new Exception("encountered a null environment");
                }

                _Environment = _Environment._Enclosing;
            }

            _Environment.Assign(stmt.Name, klass);
            return new Void();
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            object? left = Evaluate(expr.Left);
            object? right = Evaluate(expr.Right);
            double l, r;

            switch (expr.Operator.Type)
            {
                case TokenType.GREATER:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l > r;
                case TokenType.GREATER_EQUAL:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l >= r;
                case TokenType.LESS:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l < r;
                case TokenType.LESS_EQUAL:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l <= r;
                case TokenType.MINUS:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l - r;
                case TokenType.PLUS:
                    if (left is double leftDouble && right is double rightDouble)
                    {
                        return leftDouble + rightDouble;
                    }
                    if (left is string leftStr && right is string rightStr)
                    {
                        return leftStr + rightStr;
                    }
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings");
                case TokenType.SLASH:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l / r;
                case TokenType.STAR:
                    (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l * r;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            // Unreachable
            return null;
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            object? callee = Evaluate(expr.Callee);

            List<object?> arguments = new();

            foreach (Expr argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (callee is not ILoxCallable function)
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} " +
                    $"arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object? VisitGetExpr(Expr.Get expr)
        {
            object? obj = Evaluate(expr.Obj);
            if (obj is LoxInstance instance)
            {
                return instance.Get(expr.Name);
            }

            throw new RuntimeError(expr.Name, "Only instances have properties.");
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            object? left = Evaluate(expr.Left);

            if (expr.Operator.Type == TokenType.OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.Right);
        }

        public object? VisitSetExpr(Expr.Set expr)
        {
            object? obj = Evaluate(expr.Obj);

            if (obj is not LoxInstance instance)
            {
                throw new RuntimeError(expr.Name, "Only instances have fields.");
            }

            object? value = Evaluate(expr.Value);
            instance.Set(expr.Name, value);

            return value;
        }

        public object? VisitSuperExpr(Expr.Super expr)
        {
            int distance = _Locals[expr];

            if (_Environment.GetAt(distance, "super") is not LoxClass superclass)
            {
                throw new Exception("failed to find superclass");
            }

            if (_Environment.GetAt(distance - 1, "this") is not LoxInstance obj)
            {
                throw new Exception("failed to find this instance");
            }

            LoxFunction? method = superclass.FindMethod(expr.Method.Lexeme);

            if (method == null)
            {
                throw new RuntimeError(expr.Method, $"Undefined property '{expr.Method.Lexeme}'.");
            }

            return method.Bind(obj);
        }

        public object? VisitThisExpr(Expr.This expr)
        {
            return LookUpVariable(expr.Keyword, expr);
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            object? right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    double r = CheckNumberOperand(expr.Operator, right);
                    return -r;
            }

            return null;
        }

        private static double CheckNumberOperand(Token op, object? operand)
        {
            if (operand is double d) { return d; }
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private static (double, double) CheckNumberOperands(Token op, object? left, object? right)
        {
            if (left is double l && right is double r)
            {
                return (l, r);
            }

            throw new RuntimeError(op, "Operands must be numbers.");
        }

        private static bool IsTruthy(object? value)
        {
            if (value == null) { return false; }
            if (value is bool boolean) { return boolean; }
            return true;
        }

        private static bool IsEqual(object? a, object? b)
        {
            if (a == null && b == null) { return true; }
            if (a == null) { return false; }

            return a.Equals(b);
        }

        private static string Stringify(object? value)
        {
            if (value == null) { return "nil"; }

            if (value is double d)
            {
                string t = d.ToString();
                if (t.EndsWith(".0"))
                {
                    t = t.Substring(0, t.Length - 2);
                }

                return t;
            }

            return value.ToString() ?? "nil";
        }

        private object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        public Void VisitExpressionStmt(Stmt.Expression stmt)
        {
            _ = Evaluate(stmt.Expr);
            return new Void();
        }

        public Void VisitFunctionStmt(Stmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, _Environment, false);
            _Environment.Define(stmt.Name.Lexeme, function);
            return new Void();
        }

        public Void VisitIfThenStmt(Stmt.IfThen stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }

            return new Void();
        }

        public Void VisitPrintStmt(Stmt.Print printstmt)
        {
            object? value = Evaluate(printstmt.Expr);
            Console.WriteLine(Stringify(value));
            return new Void();
        }

        public Void VisitReturnStmt(Stmt.Return stmt)
        {
            object? value = null;
            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }

            throw new Return(value);
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        public object? LookUpVariable(Token name, Expr expr)
        {
            if (_Locals.ContainsKey(expr))
            {
                int distance = _Locals[expr];
                return _Environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        public Void VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;

            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            _Environment.Define(stmt.Name.Lexeme, value);
            return new Void();
        }

        public Void VisitWhileLoopStmt(Stmt.WhileLoop stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }

            return new Void();
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            object? value = Evaluate(expr.Value);

            if (_Locals.ContainsKey(expr))
            {
                int distance = _Locals[expr];
                _Environment.AssignAt(distance, expr.Name, value);
            }
            else
            {
                Globals.Assign(expr.Name, value);
            }

            return value;
        }
    }
}
