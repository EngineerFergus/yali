using System.Text;

namespace CSLox
{
    public static class Lox
    {
        private static bool _HadError = false;

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
            Scanner scanner = new Scanner(source);
            List<Tokens> tokens = scanner.ScanTokens();

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        private static void Error(int line, string message)
        {
            Report(line, string.Empty, message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            _HadError = true;
        }
    }
}
