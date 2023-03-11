namespace CSLox
{
    internal class Resolver : Expr.IVisitor<Void>, Stmt.IVisitor<Void>
    {
        private readonly Interpreter _Interpreter;
        private readonly Stack<Dictionary<string, bool>> _Scopes = new();

        public Resolver(Interpreter interpreter)
        {
            _Interpreter = interpreter;
        }

        public void Resolve(List<Stmt?> stmts)
        {
            foreach (var stmt in stmts)
            {
                Resolve(stmt);
            }
        }

        private void BeginScope()
        {
            _Scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            _Scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (_Scopes.Count == 0) { return; }

            Dictionary<string, bool> scope = _Scopes.Peek();
            scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (_Scopes.Count == 0) { return; }
            var scope = _Scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                scope[name.Lexeme] = true;
            }
            else
            {
                scope.Add(name.Lexeme, true);
            }
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = _Scopes.Count - 1; i >= 0; i--)
            {
                if (_Scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _Interpreter.Resolve(expr, _Scopes.Count - 1 - i);
                    return;
                }
            }
        }

        public Void VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return new Void();
        }

        public Void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
            return new Void();
        }

        public Void VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            ResolveFunction(stmt);
            return new Void();
        }

        public Void VisitIfThenStmt(Stmt.IfThen stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) { Resolve(stmt.ElseBranch); }
            return new Void();
        }

        public Void VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
            return new Void();
        }

        public Void VisitReturnStmt(Stmt.Return stmt)
        {
            if (stmt.Value != null)
            {
                Resolve(stmt.Value);
            }

            return new Void();
        }

        public Void VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
            return new Void();
        }

        public Void VisitWhileLoopStmt(Stmt.WhileLoop stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return new Void();
        }

        public Void VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return new Void();
        }

        public Void VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return new Void();
        }

        public Void VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }

            return new Void();
        }

        public Void VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return new Void();
        }

        public Void VisitLiteralExpr(Expr.Literal expr)
        {
            return new Void();
        }

        public Void VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return new Void();
        }

        public Void VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return new Void();
        }

        public Void VisitVariableExpr(Expr.Variable expr)
        {
            if (_Scopes.Count > 0 && _Scopes.Peek()[expr.Name.Lexeme] == false)
            {
                Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return new Void();
        }

        private void Resolve(Stmt? stmt)
        {
            stmt?.Accept(this);
        }

        private void Resolve(Expr? expr)
        {
            expr?.Accept(this);
        }

        private void ResolveFunction(Stmt.Function function)
        {
            BeginScope();
            foreach (Token param in function.Params)
            {
                Declare(param);
                Define(param);
            }

            Resolve(function.Body);
            EndScope();
        }
    }
}
