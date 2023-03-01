// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitExpressionStmt(Expression expression);
            T VisitPrintStmt(Print print);
            T VisitVarStmt(Var var);
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
