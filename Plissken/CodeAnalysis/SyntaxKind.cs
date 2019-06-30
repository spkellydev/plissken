namespace Plissken.CodeAnalysis
{
    public enum SyntaxKind
    {
        // tokens
        BadToken,
        EOFToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        ForwardSlashToken,
        OpenParenToken,
        CloseParenToken,
        // expressions
        NumberExpression,
        BinaryExpression,
        ParenExpression
    }
}
