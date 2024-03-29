// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitClassStmt(Class stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfThenStmt(IfThen stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitWhileLoopStmt(WhileLoop stmt);
            T VisitVarStmt(Var stmt);
        }

        public class Block : Stmt
        {
            public List<Stmt?> Statements { get; }

            public Block(List<Stmt?> statements)
            {
                Statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitBlockStmt(this);
            }
        }

        public class Class : Stmt
        {
            public Token Name { get; }
            public Expr.Variable? Superclass { get; }
            public List<Stmt.Function> Methods { get; }

            public Class(Token name, Expr.Variable? superclass, List<Stmt.Function> methods)
            {
                Name = name;
                Superclass = superclass;
                Methods = methods;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitClassStmt(this);
            }
        }

        public class Expression : Stmt
        {
            public Expr Expr { get; }

            public Expression(Expr expr)
            {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitExpressionStmt(this);
            }
        }

        public class Function : Stmt
        {
            public Token Name { get; }
            public List<Token> Params { get; }
            public List<Stmt?> Body { get; }

            public Function(Token name, List<Token> parameters, List<Stmt?> body)
            {
                Name = name;
                Params = parameters;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitFunctionStmt(this);
            }
        }

        public class IfThen : Stmt
        {
            public Expr Condition { get; }
            public Stmt ThenBranch { get; }
            public Stmt? ElseBranch { get; }

            public IfThen(Expr condition, Stmt thenBranch, Stmt? elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitIfThenStmt(this);
            }
        }

        public class Print : Stmt
        {
            public Expr Expr { get; }

            public Print(Expr expr)
            {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitPrintStmt(this);
            }
        }

        public class Return : Stmt
        {
            public Token Keyword { get; }
            public Expr? Value { get; }

            public Return(Token keyword, Expr? value)
            {
                Keyword = keyword;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitReturnStmt(this);
            }
        }

        public class WhileLoop : Stmt
        {
            public Expr Condition { get; }
            public Stmt Body { get; }

            public WhileLoop(Expr condition, Stmt body)
            {
                Condition = condition;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitWhileLoopStmt(this);
            }
        }

        public class Var : Stmt
        {
            public Token Name { get; }
            public Expr? Initializer { get; }

            public Var(Token name, Expr? initializer)
            {
                Name = name;
                Initializer = initializer;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitVarStmt(this);
            }
        }
    }
}
