namespace PlisskenLibrary.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // statements
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,

        // expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
    }
}
