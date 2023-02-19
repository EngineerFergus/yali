using System.Text;

namespace CSLox
{
    public static class Lox
    {
        private static readonly Interpreter _Interpreter = new();
        private static bool _HadError = false;
        private static bool _HadRuntimeError = false;

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));
            if (_HadError)
            {
                Environment.Exit(65);
            }

            if (_HadRuntimeError)
            {
                Environment.Exit(70);
            }
        }

        private static void RunPrompt()
        {
            while (true)
            {
                string? line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }

                Run(line);
                _HadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new(tokens);
            Expr? expr = parser.Parse();

            if (_HadError) { return; }

            _Interpreter.Interpret(expr);
        }

        public static void Error(int line, string message)
        {
            Report(line, string.Empty, message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at \'{token.Lexeme}\'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n [line {error.Token.Line}]");
            _HadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            _HadError = true;
        }
    }
}
