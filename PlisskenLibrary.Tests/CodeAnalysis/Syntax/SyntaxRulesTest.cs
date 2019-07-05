﻿using PlisskenLibrary.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PlisskenLibrary.Tests.CodeAnalysis.Syntax
{
    public class SyntaxRulesTest
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxRule_GetText_RoundTrips(SyntaxKind kind)
        {
            var text = SyntaxRules.GetText(kind);
            if (text == null) return;

            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);

            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var kinds = (SyntaxKind[])System.Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in kinds)
            {
                yield return new object[] { kind };
            }
        }
    }
}
