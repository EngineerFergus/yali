using System.Text;

namespace GenerateAst
{
    internal class GenerateAst
    {
        public static void MainAst(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: GenerateAst <output_directory>");
                Environment.Exit(64);
            }

            string outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>()
            {
                "Assign   : Token name | Name, Expr value | Value",
                "Binary   : Expr left | Left, Token op | Operator, Expr right | Right",
                "Call     : Expr callee | Callee, Token paren | Paren, List<Expr> arguments | Arguments",
                "Grouping : Expr expression | Expression",
                "Literal  : object? value | Value",
                "Logical  : Expr left | Left, Token op | Operator, Expr right | Right",
                "Unary    : Token op | Operator, Expr right | Right",
                "Variable : Token name | Name",
            });

            DefineAst(outputDir, "Stmt", new List<string>()
            {
                "Block       : List<Stmt?> statements | Statements",
                "Expression  : Expr expr | Expr",
                "Function    : Token name | Name, List<Token> parameters | Params, List<Stmt?> body | Body",
                "IfThen      : Expr condition | Condition, Stmt thenBranch | ThenBranch, Stmt? elseBranch | ElseBranch",
                "Print       : Expr expr | Expr",
                "WhileLoop   : Expr condition | Condition, Stmt body | Body",
                "Var         : Token name | Name, Expr? initializer | Initializer",
            });
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            string path = Path.Combine(outputDir, $"{baseName}.cs");
            StringBuilder builder = new();
            builder.AppendLine($"// Auto-generated code from {nameof(GenerateAst)} tool");

            builder.AppendLine("namespace CSLox");
            builder.AppendLine("{");

            builder.AppendLine($"    public abstract class {baseName}");
            builder.AppendLine("    {");
            builder.AppendLine($"        public abstract T Accept<T>(IVisitor<T> visitor);");

            DefineVisitor(builder, baseName, types);

            foreach (string type in types)
            {
                builder.AppendLine();
                string[] splits = type.Split(':');
                string className = splits[0].Trim();
                string fieldList = splits[1];
                DefineType(builder, baseName, className, fieldList);
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");
            File.WriteAllText(path, builder.ToString());
        }

        private static void DefineVisitor(StringBuilder builder, string baseName, List<string> types)
        {
            builder.AppendLine();
            builder.AppendLine($"        public interface IVisitor<T>");
            builder.AppendLine("        {");

            foreach (string type in types)
            {
                string[] splits = type.Split(':');
                string className = splits[0].Trim();
                builder.AppendLine($"            T Visit{className}{baseName}({className} {className.ToLower()});");
            }

            builder.AppendLine("        }");
        }

        private static void DefineType(StringBuilder builder, string baseName, string className, string fieldList)
        {
            string[] fieldsplits = fieldList.Split(',');
            List<FieldContainer> fields = new();
            StringBuilder initializerList = new StringBuilder();

            foreach(string field in fieldsplits)
            {
                FieldContainer f = FieldContainer.Parse(field);
                fields.Add(f);
                initializerList.Append($"{f.FieldType} {f.ParamName}, ");
            }

            string initList = initializerList.ToString().Trim().Trim(',');

            builder.AppendLine($"        public class {className} : {baseName}");
            builder.AppendLine("        {");

            foreach (FieldContainer f in fields)
            {
                builder.AppendLine($"            public {f.FieldType} {f.MemberName} {{ get; }}");
            }

            builder.AppendLine();
            builder.AppendLine($"            public {className}({initList})");
            builder.AppendLine("            {");

            foreach (FieldContainer field in fields)
            {
                builder.AppendLine($"                {field.MemberName} = {field.ParamName};");
            }

            builder.AppendLine("            }");

            // Visitor pattern
            builder.AppendLine();
            builder.AppendLine($"            public override T Accept<T>(IVisitor<T> visitor)");
            builder.AppendLine("            {");
            builder.AppendLine($"               return visitor.Visit{className}{baseName}(this);");
            builder.AppendLine("            }");
            builder.AppendLine("        }");
        }
    }
}
