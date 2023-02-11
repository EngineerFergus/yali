using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Tests
{
    [TestClass]
    public class ScannerTests
    {
        private Scanner InitScanner(string source)
        {
            return new Scanner(source);
        }

        [DataTestMethod]
        [DataRow("(", TokenType.LEFT_PAREN)]
        [DataRow(")", TokenType.RIGHT_PAREN)]
        [DataRow("{", TokenType.LEFT_BRACE)]
        [DataRow("}", TokenType.RIGHT_BRACE)]
        [DataRow(",", TokenType.COMMA)]
        [DataRow(".", TokenType.DOT)]
        [DataRow("-", TokenType.MINUS)]
        [DataRow("+", TokenType.PLUS)]
        [DataRow(";", TokenType.SEMICOLON)]
        [DataRow("/", TokenType.SLASH)]
        [DataRow("*", TokenType.STAR)]
        [DataRow("!", TokenType.BANG)]
        [DataRow("!=", TokenType.BANG_EQUAL)]
        [DataRow("=", TokenType.EQUAL)]
        [DataRow("==", TokenType.EQUAL_EQUAL)]
        [DataRow(">", TokenType.GREATER)]
        [DataRow(">=", TokenType.GREATER_EQUAL)]
        [DataRow("<", TokenType.LESS)]
        [DataRow("<=", TokenType.LESS_EQUAL)]
        [DataRow("myVar", TokenType.IDENTIFIER)]
        [DataRow("_myVar", TokenType.IDENTIFIER)]
        [DataRow("my_Var", TokenType.IDENTIFIER)]
        [DataRow("myVar_", TokenType.IDENTIFIER)]
        [DataRow("myVar_", TokenType.IDENTIFIER)]
        [DataRow("myVar_", TokenType.IDENTIFIER)]
        [DataRow("myVar_1", TokenType.IDENTIFIER)]
        [DataRow("_", TokenType.IDENTIFIER)]
        [DataRow("123", TokenType.NUMBER)]
        [DataRow("123.123", TokenType.NUMBER)]
        [DataRow("and", TokenType.AND)]
        [DataRow("class", TokenType.CLASS)]
        [DataRow("else", TokenType.ELSE)]
        [DataRow("false", TokenType.FALSE)]
        [DataRow("fun", TokenType.FUN)]
        [DataRow("for", TokenType.FOR)]
        [DataRow("if", TokenType.IF)]
        [DataRow("nil", TokenType.NIL)]
        [DataRow("or", TokenType.OR)]
        [DataRow("print", TokenType.PRINT)]
        [DataRow("return", TokenType.RETURN)]
        [DataRow("super", TokenType.SUPER)]
        [DataRow("this", TokenType.THIS)]
        [DataRow("true", TokenType.TRUE)]
        [DataRow("var", TokenType.VAR)]
        [DataRow("while", TokenType.WHILE)]
        public void SingleTokens(string source, TokenType expectedType)
        {
            var scanner = InitScanner(source);
            var tokens = scanner.ScanTokens();
            Assert.AreEqual(2, tokens.Count, "token count not one plus EOF");
            Assert.AreEqual(expectedType, tokens[0].Type, "token type did not matchs");
            Assert.AreEqual(source, tokens[0].Lexeme, "token lexeme did not match");
        }

        private string WrapInQuotes(string source)
        {
            return $"\"{source}\"";
        }

        [DataTestMethod]
        [DataRow("this is a string")]
        [DataRow("oh boy this has some numbers 1.2.3.4")]
        public void Strings(string source)
        {
            source = WrapInQuotes(source);
            var scanner = InitScanner(source);
            var tokens = scanner.ScanTokens();
            Assert.AreEqual(2, tokens.Count, "improper number of tokens");
            Assert.AreEqual(TokenType.STRING, tokens[0].Type, "token type was not string");
            Assert.AreEqual(source, tokens[0].Lexeme, "string contents were incorrect");
        }

        [DataTestMethod]
        [DataRow("123", 123)]
        [DataRow("123.123", 123.123)]
        public void Numbers(string source, double expectedValue)
        {
            var scanner = InitScanner(source);
            var tokens = scanner.ScanTokens();
            Assert.AreEqual(2, tokens.Count, "improper number of tokens");
            Assert.AreEqual(TokenType.NUMBER, tokens[0].Type, "token type was not number");
            if (tokens[0].Literal is not double numberVal)
            {
                throw new Exception("token literal was not a double");
            }
            Assert.AreEqual(expectedValue, numberVal, 0.001 * expectedValue, "value was not equal");
        }

        [DataTestMethod]
        [DataRow("( )", new TokenType[] {TokenType.LEFT_PAREN, TokenType.RIGHT_PAREN})]
        [DataRow("( ) { }", new TokenType[] {TokenType.LEFT_PAREN, TokenType.RIGHT_PAREN, TokenType.LEFT_BRACE, TokenType.RIGHT_BRACE})]
        [DataRow("(myVar)", new TokenType[] {TokenType.LEFT_PAREN, TokenType.IDENTIFIER, TokenType.RIGHT_PAREN})]
        [DataRow("+ = - >=", new TokenType[] {TokenType.PLUS, TokenType.EQUAL, TokenType.MINUS, TokenType.GREATER_EQUAL})]
        public void MultiTokens(string source, TokenType[] expectedTypes)
        {
            var scanner = InitScanner(source);
            var tokens = scanner.ScanTokens();
            Assert.AreEqual(expectedTypes.Length + 1, tokens.Count, "token counts did not match");

            for (int i = 0; i < expectedTypes.Length; i++)
            {
                Assert.AreEqual(expectedTypes[i], tokens[i].Type, $"token {i} did not match");
            }
        }
    }
}
