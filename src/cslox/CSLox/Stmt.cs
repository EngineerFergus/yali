// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitBlockStmt(Block block);
            T VisitExpressionStmt(Expression expression);
            T VisitIfThenStmt(IfThen ifthen);
            T VisitPrintStmt(Print print);
            T VisitVarStmt(Var var);
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
