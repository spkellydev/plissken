namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // tokens
        BadToken,
        EOFToken,
        WhitespaceToken,
        NumberToken,
        PlusToken, // +
        MinusToken, // -
        StarToken, // *
        ForwardSlashToken, // /
        OpenParenToken, // (
        CloseParenToken, // )
        IdentifierToken,
        BangToken, // !
        AmpersandAmpersandToken, // &&
        PipePipeToken, // ||
        EqualToken, // =
        EqualEqualToken, // ==
        BangEqualToken, // !=
        // keywords
        FalseKeyword, // false
        TrueKeyword, // true
        // expressions
        LiteralExpression,
        BinaryExpression,
        ParenExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,
    }
}
