// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Expr
    {
        public Expr? Left { get; init; }
        public Token? Operator { get; init; }
        public Expr? Right { get; init; }
        public object? Value { get; init; }
    }

    public class Binary : Expr
    {
        public Binary(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }

    public class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            Left = expression;
        }
    }

    public class Literal : Expr
    {
        public Literal(object value)
        {
            Value = value;
        }
    }

    public class Unary : Expr
    {
        public Unary(Token op, Expr right)
        {
            Operator = op;
            Right = right;
        }
    }
}
