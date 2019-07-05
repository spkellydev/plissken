using System.Collections.Generic;

namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class ParenExpressionSyntax : ExpressionSyntax
    {
        public ParenExpressionSyntax(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            OpenParenToken = openParenToken;
            Expression = expression;
            CloseParenToken = closeParenToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenExpression;

        public SyntaxToken OpenParenToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenToken { get; }
    }
}
