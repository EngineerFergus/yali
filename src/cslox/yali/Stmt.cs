// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    public interface IStmtVisitor<T>
    {
        T VisitExprStmtStmt(ExprStmt exprstmt);
        T VisitPrintStmtStmt(PrintStmt printstmt);
    }

    public class ExprStmt : Stmt
    {
        public Expr Expr { get; }

        public ExprStmt(Expr expr)
        {
            Expr = expr;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExprStmtStmt(this);
        }
    }

    public class PrintStmt : Stmt
    {
        public Expr Expr { get; }

        public PrintStmt(Expr expr)
        {
            Expr = expr;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmtStmt(this);
        }
    }
}
