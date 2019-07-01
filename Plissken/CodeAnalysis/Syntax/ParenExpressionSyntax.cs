using System.Collections.Generic;

namespace PlisskenCompiler.CodeAnalysis.Syntax
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

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenToken;
            yield return Expression;
            yield return CloseParenToken;
        }
    }
}
