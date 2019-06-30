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
            if (node is LiteralExpressionSyntax n)
                return (int)n.NumberToken.Value;
            if (node is BinaryExpressionSyntax b)
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

            if (node is ParenExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            throw new Exception($"ERROR: Unexpected node {node.Kind}");
        }
    }
}
