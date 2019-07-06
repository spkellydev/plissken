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
        OpenBraceToken,  // {
        CloseBraceToken, // }
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
        
        // units
        CompliationUnit,
        
        // Statements
        BlockStatement,
        ExpressionStatement,

        // expressions
        LiteralExpression,
        BinaryExpression,
        ParenExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,
    }
}
