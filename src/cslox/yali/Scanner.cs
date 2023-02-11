namespace CSLox
{
    public class Scanner
    {
        private readonly string _Source;
        private readonly List<Token> _Tokens = new();
        private int _Start = 0;
        private int _Current = 0;
        private int _Line = 1;

        private readonly Dictionary<string, TokenType> _Keywords = new()
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE },
        };

        public Scanner(string source)
        {
            _Source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _Start = _Current;
                ScanToken();
            }

            _Tokens.Add(new Token(TokenType.EOF, "", null, _Line));
            return _Tokens;
        }

        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        while(Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespaces
                    break;
                case '\n':
                    _Line++;
                    break;
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(_Line, $"Unexpected character {c}");
                    }

                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            string text = _Source.Substring(_Start, _Current - _Start);
            TokenType type = TokenType.IDENTIFIER;

            if (_Keywords.ContainsKey(text))
            {
                type = _Keywords[text];
            }

            AddToken(type);
        }

        private void Number()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenType.NUMBER, double.Parse(_Source.Substring(_Start, _Current - _Start)));
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _Line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_Line, "Unterminated string.");
                return;
            }

            Advance(); // Get the closing " quotation

            string value = _Source.Substring(_Start + 1, (_Current - _Start) - 2);
            AddToken(TokenType.STRING, value);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) { return false; }
            if (_Source[_Current] != expected) { return false; }

            _Current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) { return '\0'; }
            return _Source[_Current];
        }

        private char PeekNext()
        {
            if (_Current + 1 >= _Source.Length) { return '\0'; }
            return _Source[_Current + 1];
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAtEnd()
        {
            return _Current >= _Source.Length;
        }

        private char Advance()
        {
            char c = _Source[_Current];
            _Current++;
            return c;
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _Source.Substring(_Start, _Current - _Start);
            _Tokens.Add(new Token(type, text, literal, _Line));
        }
    }
}
