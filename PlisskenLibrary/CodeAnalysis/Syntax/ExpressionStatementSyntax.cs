namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        // c-like valid expression statements
        // initialization, assigment, expression...
        public ExpressionStatementSyntax(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

        public ExpressionSyntax Expression { get; }
    }
}
