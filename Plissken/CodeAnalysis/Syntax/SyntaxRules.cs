using System;

namespace PlisskenCompiler.CodeAnalysis.Syntax
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
                case SyntaxKind.BangToken:
                    return 6;
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
                    return 5;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.EqualEqualToken:
                case SyntaxKind.BangEqualToken:
                    return 3;

                case SyntaxKind.AmpersandAmpersandToken:
                    return 2;

                case SyntaxKind.PipePipeToken:
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
