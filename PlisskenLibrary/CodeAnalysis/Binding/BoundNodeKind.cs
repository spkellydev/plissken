namespace PlisskenLibrary.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // statements
        BlockStatement,
        ExpressionStatement,
        // expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
    }
}
