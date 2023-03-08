namespace CSLox
{
    internal class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<Void>
    {
        private Environment _Environment = new();

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

        private void ExecuteBlock(List<Stmt?> statements, Environment environment)
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

        public object? VisitBinaryExpr(Expr.Binary binary)
        {
            object? left = Evaluate(binary.Left);
            object? right = Evaluate(binary.Right);
            double l, r;

            switch (binary.Operator.Type)
            {
                case TokenType.GREATER:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
                    return l > r;
                case TokenType.GREATER_EQUAL:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
                    return l >= r;
                case TokenType.LESS:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
                    return l < r;
                case TokenType.LESS_EQUAL:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
                    return l <= r;
                case TokenType.MINUS:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
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
                    throw new RuntimeError(binary.Operator, "Operands must be two numbers or two strings");
                case TokenType.SLASH:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
                    return l / r;
                case TokenType.STAR:
                    (l, r) = CheckNumberOperands(binary.Operator, left, right);
                    return l * r;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            // Unreachable
            return null;
        }

        public object? VisitCallExpr(Expr.Call call)
        {
            object? callee = Evaluate(call.Callee);

            List<object?> arguments = new();

            foreach (Expr argument in call.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (callee is not ILoxCallable function)
            {
                throw new RuntimeError(call.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(call.Paren, $"Expected {function.Arity()} " +
                    $"arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object? VisitGroupingExpr(Expr.Grouping grouping)
        {
            return Evaluate(grouping.Expression);
        }

        public object? VisitLiteralExpr(Expr.Literal literal)
        {
            return literal.Value;
        }

        public object? VisitLogicalExpr(Expr.Logical logical)
        {
            object? left = Evaluate(logical.Left);

            if (logical.Operator.Type == TokenType.OR)
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

            return Evaluate(logical.Right);
        }

        public object? VisitUnaryExpr(Expr.Unary unary)
        {
            object? right = Evaluate(unary.Right);

            switch (unary.Operator.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    double r = CheckNumberOperand(unary.Operator, right);
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

        public Void VisitExpressionStmt(Stmt.Expression exprstmt)
        {
            _ = Evaluate(exprstmt.Expr);
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

        public object? VisitVariableExpr(Expr.Variable variable)
        {
            return _Environment.Get(variable.Name);
        }

        public Void VisitVarStmt(Stmt.Var var)
        {
            object? value = null;

            if (var.Initializer != null)
            {
                value = Evaluate(var.Initializer);
            }

            _Environment.Define(var.Name.Lexeme, value);
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
            _Environment.Assign(expr.Name, value);
            return value;
        }
    }
}
