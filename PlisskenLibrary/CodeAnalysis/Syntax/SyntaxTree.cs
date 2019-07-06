using PlisskenLibrary.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompliationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Diagnostics = diagnostics.ToImmutableArray();
            Text = text;
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompliationUnitSyntax Root { get; }
        public SyntaxToken EOFToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        /// <summary>
        /// ParseTokens will return tokenized text except the EOF token. This method can be useful,
        /// for example, to ensure a given token has a valid identifier
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
        {
            var lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EOFToken) break;
                yield return token;
            }
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }
    }
}
