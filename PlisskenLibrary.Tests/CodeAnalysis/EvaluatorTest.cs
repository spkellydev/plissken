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
        [InlineData("{ var a = 10 var b = 10 * a }", 100)]
        public void EvaluatorText_Expression_Evaluated(string text, object expectedResult)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(result.Value, expectedResult);
        }
    }
}
