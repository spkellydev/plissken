namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class CompliationUnitSyntax : SyntaxNode
    {
        public CompliationUnitSyntax(StatementSyntax statement, SyntaxToken eofToken)
        {
            Statement = statement;
            EofToken = eofToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CompliationUnit;

        public StatementSyntax Statement { get; }
        public SyntaxToken EofToken { get; }
    }
}
