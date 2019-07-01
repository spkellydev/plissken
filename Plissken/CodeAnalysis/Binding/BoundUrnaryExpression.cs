using System;

namespace Plissken.CodeAnalysis.Binding
{
    internal sealed class BoundUrnaryExpression : BoundExpression
    {
        public BoundUrnaryExpression(BoundUnaryOperatorKind operatorKind, BoundExpression operand)
        {
            OperatorKind = operatorKind;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public BoundUnaryOperatorKind OperatorKind { get; }
        public BoundExpression Operand { get; }
        public override Type Type => Operand.Type;
    }
}
