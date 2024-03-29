﻿namespace CSLox
{
    internal class Resolver : Expr.IVisitor<Void>, Stmt.IVisitor<Void>
    {
        private enum FunctionType
        {
            NONE,
            METHOD,
            FUNCTION,
            INITIALIZER,
        }

        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS,
        }

        private readonly Interpreter _Interpreter;
        private readonly Stack<Dictionary<string, bool>> _Scopes = new();
        private FunctionType _CurrentFunction = FunctionType.NONE;
        private ClassType _CurrentClass = ClassType.NONE;

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

            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Already a variable with this name in this scope");
            }

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
            for (int i = 0; i < _Scopes.Count; i++)
            {
                if (_Scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _Interpreter.Resolve(expr, i);
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

        public Void VisitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = _CurrentClass;
            _CurrentClass = ClassType.CLASS;

            Declare(stmt.Name);
            Define(stmt.Name);

            if (stmt.Superclass != null && stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme))
            {
                Lox.Error(stmt.Superclass.Name, "A class can't inherit from itself.");
            }

            if (stmt.Superclass != null)
            {
                _CurrentClass = ClassType.SUBCLASS;
                Resolve(stmt.Superclass);
            }

            if (stmt.Superclass != null)
            {
                BeginScope();
                _Scopes.Peek().Add("super", true);
            }

            BeginScope();
            _Scopes.Peek().Add("this", true);

            foreach (Stmt.Function method in stmt.Methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if (method.Name.Lexeme.Equals("init"))
                {
                    declaration = FunctionType.INITIALIZER;
                }
                ResolveFunction(method, declaration);
            }

            EndScope();

            if (stmt.Superclass != null) { EndScope(); }

            _CurrentClass = enclosingClass;
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
            ResolveFunction(stmt, FunctionType.FUNCTION);
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
            if (_CurrentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.Keyword, "Can't return from top-level code.");
            }

            if (stmt.Value != null)
            {
                if (_CurrentFunction == FunctionType.INITIALIZER)
                {
                    Lox.Error(stmt.Keyword, "Can't return from an initializer.");
                }
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

        public Void VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Obj);
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

        public Void VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Obj);
            return new Void();
        }

        public Void VisitSuperExpr(Expr.Super expr)
        {
            if (_CurrentClass == ClassType.NONE)
            {
                Lox.Error(expr.Keyword, "Can't use 'super' outside of a class.");
            }
            else if (_CurrentClass != ClassType.SUBCLASS)
            {
                Lox.Error(expr.Keyword, "Can't use 'super' in a class with no superclass.");
            }

            ResolveLocal(expr, expr.Keyword);
            return new Void();
        }

        public Void VisitThisExpr(Expr.This expr)
        {
            if (_CurrentClass == ClassType.NONE)
            {
                Lox.Error(expr.Keyword, "Can't use 'this' outside of class.");
                return new Void();
            }

            ResolveLocal(expr, expr.Keyword);
            return new Void();
        }

        public Void VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return new Void();
        }

        public Void VisitVariableExpr(Expr.Variable expr)
        {
            if (_Scopes.Count > 0)
            {
                var scope = _Scopes.Peek();
                if (scope.ContainsKey(expr.Name.Lexeme) && scope[expr.Name.Lexeme] == false)
                {
                    Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
                }
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

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = _CurrentFunction;
            _CurrentFunction = type;

            BeginScope();
            foreach (Token param in function.Params)
            {
                Declare(param);
                Define(param);
            }

            Resolve(function.Body);
            EndScope();
            _CurrentFunction = enclosingFunction;
        }
    }
}
