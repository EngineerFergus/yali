// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Expr
    {
        public Expr? Left { get; init; }
        public Token? Operator { get; init; }
        public Expr? Right { get; init; }
        public object? Value { get; init; }
        public abstract T Accept<T>(IVisitor<T> visitor);
    }

    public interface IVisitor<T>
    {
        T VisitBinaryExpr(Binary binary);
        T VisitGroupingExpr(Grouping grouping);
        T VisitLiteralExpr(Literal literal);
        T VisitUnaryExpr(Unary unary);
    }

    public class Binary : Expr
    {
        public Binary(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            Left = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public Literal(object? value)
        {
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Unary(Token op, Expr right)
        {
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
