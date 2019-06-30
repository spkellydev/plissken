using System;
using System.Text;
using System.Threading.Tasks;

namespace Plissken.CodeAnalysis
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            // BinaryExpression
            // NumberExpression
            switch (node)
            {
                case LiteralExpressionSyntax n:
                    return (int)n.NumberToken.Value;
                case UnaryExpressionSyntax u:
                    {
                        var operand = EvaluateExpression(u.Operand);
                        switch (u.OperatorToken.Kind)
                        {
                            case SyntaxKind.PlusToken:
                                return operand;
                            case SyntaxKind.MinusToken:
                                return -operand;
                            default:
                                throw new Exception($"ERROR: Unexpected unary operator {u.OperatorToken.Kind}");
                        }
                    }

                case BinaryExpressionSyntax b:
                    {
                        var left = EvaluateExpression(b.Left);
                        var right = EvaluateExpression(b.Right);

                        if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                            return left + right;
                        else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                            return left - right;
                        else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                            return left * right;
                        else if (b.OperatorToken.Kind == SyntaxKind.ForwardSlashToken)
                            return left / right;
                        else
                            throw new Exception($"ERROR: Unexpected binary operator {b.OperatorToken.Kind}");
                    }

                case ParenExpressionSyntax p:
                    return EvaluateExpression(p.Expression);
                default:
                    throw new Exception($"ERROR: Unexpected node {node.Kind}");
            }
        }
    }
}
