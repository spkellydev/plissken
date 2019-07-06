namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        // var x = 10 -- mut
        // let x = 10 -- nomut
        public VariableDeclarationSyntax(SyntaxToken keyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax initializer)
        {
            Keyword = keyword;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;

        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
    }
}
