using PlisskenLibrary.CodeAnalysis;
using PlisskenLibrary.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PlisskenLibrary.Tests.CodeAnalysis
{

    public class EvaluatorTest
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("+1", 1)]
        [InlineData("2 + 1", 3)]
        [InlineData("2 - 1", 1)]
        [InlineData("1 - 2", -1)]
        [InlineData("1 * 2", 2)]
        [InlineData("4 / 2", 2)]
        [InlineData("(10)", 10)]
        [InlineData("(9 + 1)", 10)]
        [InlineData("12 == 3", false)]
        [InlineData("99 == 99", true)]
        [InlineData("3 != 2", true)]
        [InlineData("3 != 3", false)]
        [InlineData("true == false", false)]
        [InlineData("true != false", true)]
        [InlineData("false == false", true)]
        [InlineData("false != false", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ var a = 0 (a = 10) * a }", 100)]
        public void EvaluatorText_Expression_Evaluated(string text, object expectedResult)
        {
            AssertValue(text, expectedResult);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            // we should be able to declare a new variable within a new scope, but we should not be able to redeclare a variable within it's current scope
            var text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 20
                    }
                    var [x] = 5
                }
            ";

            var diagnostics = @"
                Variable 'x' is already declared
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_Undefined()
        {
            // we should report diagnostics for when the user is trying to use an undeclared variable
            var text = @"[x] + 10";

            var diagnostics = @"
                Variable 'x' is not declared
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_Undefined()
        {
            Evaluator_NameExpression_Reports_Undefined();
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotAssign()
        {
            // we should report when a user is trying to re-assign an immutable variable
            var text = @"
                {
                    let x = 10
                    {
                        var x = 20
                    }
                    x [=] 5
                }
            ";

            var diagnostics = @"
                Variable 'x' is readonly, cannot re-assign
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotConvert()
        {
            // we should report when a user is trying to re-assign an immutable variable
            var text = @"
                {
                    let x = 10
                    {
                        var x = 20
                        x = [false]
                    }
                }
            ";

            var diagnostics = @"
                Cannot convert type 'System.Boolean' to type System.Int32
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_UnaryExpression_Reports_CannotConvert()
        {
            // we should report when a user is trying to break typing with unary operations
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator '+' is not defined for type 'System.Boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_BinaryExpression_Reports_CannotConvert()
        {
            // we should report when a user is trying to break typing with binary operations
            var text = @"
                {
                    let x = 10
                    x [==] false
                }
            ";

            var diagnostics = @"
                Binary operator '==' is not defined for type 'System.Int32' and 'System.Boolean'
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private static EvaluationResult GetCompilationEvaluation(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);
            return result;
        }

        /// <summary>
        /// Assert that a value is the expected result and there are no diagnostic messages
        /// </summary>
        /// <param name="text"></param>
        /// <param name="expectedResult"></param>
        private static void AssertValue(string text, object expectedResult)
        {
            var result = GetCompilationEvaluation(text);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(result.Value, expectedResult);
        }

        /// <summary>
        /// Assert that a compiled statement should contain the exact diagnostics we are expecting.
        /// </summary>
        /// <param name="text">marked text</param>
        /// <param name="diagnosticText">actual diagnostics</param>
        /// Example text: 
        ///   {
        ///      let x = 20
        ///      [x] = 10 
        ///   }
        /// Example diagnosticText:
        ///   Variable 'x' is readonly, cannot re-assign
        /// The x VariableToken is the expected error text, so we have marked it.
        /// 
        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var result = GetCompilationEvaluation(annotatedText.Text);

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
            {
                throw new Exception("ERROR: Must mark as many spans as there are expected diagnostics");
            }

            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;

                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;

                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
