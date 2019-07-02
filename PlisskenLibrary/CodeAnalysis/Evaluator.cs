﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlisskenLibrary.CodeAnalysis.Binding;

namespace PlisskenLibrary.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch (node)
            {
                // LiteralExpression
                case BoundLiteralExpression n:
                    return n.Value;
                // VariableExpression
                case BoundVariableExpression v:
                {
                    return _variables[v.Variable];
                }
                // AssignmentExpression
                case BoundAssignmentExpression a:
                {
                    var value = EvaluateExpression(a.Expression);
                    _variables[a.Variable] = value;
                    return value;
                }
                // UnaryExpression
                case BoundUnaryExpression u:
                {
                    var operand = EvaluateExpression(u.Operand);
                    switch (u.Op.Kind)
                    {
                        case BoundUnaryOperatorKind.Identity:
                            return (int)operand;
                        case BoundUnaryOperatorKind.Negation:
                            return -(int)operand;
                        case BoundUnaryOperatorKind.LogicalNegation:
                            return !(bool)operand;
                        default:
                            throw new Exception($"ERROR: Unexpected unary operator {u.Op.Kind}");
                    }
                }

                // BinaryExpression
                case BoundBinaryExpression b:
                {
                    var left = EvaluateExpression(b.Left);
                    var right = EvaluateExpression(b.Right);

                    switch (b.Op.Kind)
                    {
                        case BoundBinaryOperatorKind.Addition:
                            return (int)left + (int)right;
                        case BoundBinaryOperatorKind.Subtraction:
                            return (int)left - (int)right;
                        case BoundBinaryOperatorKind.Multiplication:
                            return (int)left * (int)right;
                        case BoundBinaryOperatorKind.Division:
                            return (int)left / (int)right;
                        case BoundBinaryOperatorKind.LogicalAnd:
                            return (bool)left && (bool)right;
                        case BoundBinaryOperatorKind.LogicalOr:
                            return (bool)left || (bool)right;
                        case BoundBinaryOperatorKind.Equals:
                            return Equals(left, right);
                        case BoundBinaryOperatorKind.NotEquals:
                            return !Equals(left, right);
                        default:
                            throw new Exception($"ERROR: Unexpected binary operator {b.Op.Kind}");
                    }
                }
                default:
                    throw new Exception($"ERROR: Unexpected node {node.Kind}");
            }
        }
    }
}
