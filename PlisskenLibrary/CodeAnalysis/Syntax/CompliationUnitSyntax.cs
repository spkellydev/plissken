namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class CompliationUnitSyntax : SyntaxNode
    {
        public CompliationUnitSyntax(ExpressionSyntax expression, SyntaxToken eofToken)
        {
            Expression = expression;
            EofToken = eofToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CompliationUnit;

        public ExpressionSyntax Expression { get; }
        public SyntaxToken EofToken { get; }
    }
}
