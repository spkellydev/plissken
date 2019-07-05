using System.Collections.Generic;
using System.Linq;

namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken eofToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EOFToken = eofToken;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EOFToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        /// <summary>
        /// ParseTokens will return tokenized text except the EOF token. This method can be useful,
        /// for example, to ensure a given token has a valid identifier
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EOFToken) break;
                yield return token;
            }
        }
    }
}
