using System;
using System.Text;
using System.Threading.Tasks;
using Plissken.CodeAnalysis.Syntax;

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
            switch (node)
            {
                // LiteralExpression
                case LiteralExpressionSyntax n:
                    return (int)n.NumberToken.Value;
                // UnaryExpression
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

                // BinaryExpression
                case BinaryExpressionSyntax b:
                    {
                        var left = EvaluateExpression(b.Left);
                        var right = EvaluateExpression(b.Right);

                        switch (b.OperatorToken.Kind)
                        {
                            case SyntaxKind.PlusToken:
                                return left + right;
                            case SyntaxKind.MinusToken:
                                return left - right;
                            case SyntaxKind.StarToken:
                                return left * right;
                            case SyntaxKind.ForwardSlashToken:
                                return left / right;
                            default:
                                throw new Exception($"ERROR: Unexpected binary operator {b.OperatorToken.Kind}");
                        }
                    }

                // ParenExpression
                case ParenExpressionSyntax p:
                    return EvaluateExpression(p.Expression);
                default:
                    throw new Exception($"ERROR: Unexpected node {node.Kind}");
            }
        }
    }
}
