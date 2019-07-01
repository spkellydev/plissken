using System;

namespace Plissken.CodeAnalysis.Syntax
{
    internal static class SyntaxRules
    {
        /// <summary>
        /// Get the precedence for the unary operator
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        ///     -
        ///     |
        ///     *
        ///    / \
        ///   1   2
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// determine the precedence of the binary expression
        /// </summary>
        /// <param name="kind"></param>
        /// <returns>precedence value</returns>
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.ForwardSlashToken:
                    return 2;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch(text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }
    }
}
