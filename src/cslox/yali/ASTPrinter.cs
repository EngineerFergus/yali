using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox
{
    internal class ASTPrinter : IExprVisitor<string>
    {
        public string Print(Expr? expr)
        {
            if (expr == null)
            {
                return "Null expression encountered!";
            }

            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary binary)
        {
            return Paranthesize(binary.Operator.Lexeme, binary.Left, binary.Right);
        }

        public string VisitGroupingExpr(Grouping grouping)
        {
            return Paranthesize("group", grouping.Expression);
        }

        public string VisitLiteralExpr(Literal literal)
        {
            if (literal.Value == null)
            {
                return "nil";
            }

            return literal.Value.ToString();
        }

        public string VisitUnaryExpr(Unary unary)
        {
            return Paranthesize(unary.Operator.Lexeme, unary.Right);
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
