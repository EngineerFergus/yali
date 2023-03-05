using System.Text;

namespace CSLox
{
    internal class ASTPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr? expr)
        {
            if (expr == null)
            {
                return "Null expression encountered!";
            }

            return expr.Accept(this);
        }

        public string VisitAssignExpr(Expr.Assign assign)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpr(Expr.Binary binary)
        {
            return Paranthesize(binary.Operator.Lexeme, binary.Left, binary.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping grouping)
        {
            return Paranthesize("group", grouping.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal literal)
        {
            if (literal.Value == null)
            {
                return "nil";
            }

            return literal.Value.ToString() ?? "nil";
        }

        public string VisitLogicalExpr(Expr.Logical logical)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpr(Expr.Unary unary)
        {
            return Paranthesize(unary.Operator.Lexeme, unary.Right);
        }

        public string VisitVariableExpr(Expr.Variable variable)
        {
            throw new NotImplementedException();
        }

        private string Paranthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new();
            builder.Append('(').Append(name);

            foreach (Expr expr in exprs)
            {
                builder.Append(' ');
                builder.Append(expr.Accept(this));
            }

            builder.Append(')');
            return builder.ToString();
        }
    }
}
