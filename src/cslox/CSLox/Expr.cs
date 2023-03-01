// Auto-generated code from GenerateAst tool
namespace CSLox
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitAssignExpr(Assign assign);
            T VisitBinaryExpr(Binary binary);
            T VisitGroupingExpr(Grouping grouping);
            T VisitLiteralExpr(Literal literal);
            T VisitUnaryExpr(Unary unary);
            T VisitVariableExpr(Variable variable);
        }

        public class Assign : Expr
        {
            public Token Name { get; }
            public Expr Value { get; }

            public Assign(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitAssignExpr(this);
            }
        }

        public class Binary : Expr
        {
            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }

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
            public Expr Expression { get; }

            public Grouping(Expr expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public object? Value { get; }

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
            public Token Operator { get; }
            public Expr Right { get; }

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

        public class Variable : Expr
        {
            public Token Name { get; }

            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
               return visitor.VisitVariableExpr(this);
            }
        }
    }
}
