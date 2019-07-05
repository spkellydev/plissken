using PlisskenLibrary.CodeAnalysis.Syntax;
using System.Collections.Generic;
using Xunit;

namespace PlisskenLibrary.Tests.CodeAnalysis.Syntax
{
    public class ParserTest
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorsPairsData))]
        public void Parser_BinaryExpression_HonorsPrecedence(SyntaxKind op1, SyntaxKind op2)
        {
            var op1Precedence = SyntaxRules.GetBinaryOperatorPrecedence(op1);
            var op2Precedence = SyntaxRules.GetBinaryOperatorPrecedence(op2);
            var op1Text = SyntaxRules.GetText(op1);
            var op2Text = SyntaxRules.GetText(op2);
            var text = $"a {op1Text} b {op2Text} c";

            var expression = SyntaxTree.Parse(text).Root;

            if (op1Precedence >= op2Precedence)
            {
                //    op2
                //    /  \
                //   op1  c
                //  /  \
                // a    b
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                        e.AssertNode(SyntaxKind.BinaryExpression);
                            e.AssertNode(SyntaxKind.NameExpression);
                                e.AssertToken(SyntaxKind.IdentifierToken, "a");
                            e.AssertToken(op1, op1Text);
                                e.AssertNode(SyntaxKind.NameExpression);
                                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                                e.AssertToken(op2, op2Text);
                                    e.AssertNode(SyntaxKind.NameExpression);
                                        e.AssertToken(SyntaxKind.IdentifierToken, "c");

                }
            }
            else
            {
                //     op1
                //    /  \
                //   a    op2
                //        /  \
                //       b    c
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                        e.AssertNode(SyntaxKind.NameExpression);
                            e.AssertToken(SyntaxKind.IdentifierToken, "a");
                        e.AssertToken(op1, op1Text);
                            e.AssertNode(SyntaxKind.BinaryExpression);
                                e.AssertNode(SyntaxKind.NameExpression);
                                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                                e.AssertToken(op2, op2Text);
                                    e.AssertNode(SyntaxKind.NameExpression);
                                        e.AssertToken(SyntaxKind.IdentifierToken, "c");
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorsPairsData()
        {
            foreach (var op1 in SyntaxRules.GetBinaryOperatorKinds())
            {
                foreach(var op2 in SyntaxRules.GetBinaryOperatorKinds())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }
    }
}
