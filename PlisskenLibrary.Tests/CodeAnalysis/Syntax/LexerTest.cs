﻿using PlisskenLibrary.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PlisskenLibrary.Tests.CodeAnalysis.Syntax
{
    public class LexerTest
    {
        [Fact]
        public void Lexer_Tests_AllTokens()
        {
            var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
                    .Cast<SyntaxKind>()
                    .Where(k => k.ToString().EndsWith("Keyword") || k.ToString().EndsWith("Token"))
                    .ToList();
            var testedTokenKinds = GetTokens().Concat(GetSeparators()).Select(t => t.kind);

            var untestTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
            untestTokenKinds.Remove(SyntaxKind.EOFToken);
            untestTokenKinds.Remove(SyntaxKind.BadToken);
            untestTokenKinds.ExceptWith(testedTokenKinds);
            Assert.Empty(untestTokenKinds);
        }

        [Theory]
        [MemberData(nameof(GetTokenData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_Token_Pairs(SyntaxKind t1kind, string t1text,
                                            SyntaxKind t2kind, string t2text)
        {
            var text = t1text + t2text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);

            Assert.Equal(tokens[0].Kind, t1kind);
            Assert.Equal(tokens[0].Text, t1text);
            Assert.Equal(tokens[1].Kind, t2kind);
            Assert.Equal(tokens[1].Text, t2text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeparatorData))]
        public void Lexer_Lexes_Token_Pairs_With_Separator(SyntaxKind t1kind, string t1text,
                                    SyntaxKind separatorKind, string separatorText,
                                    SyntaxKind t2kind, string t2text)
        {
            var text = t1text + separatorText + t2text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);

            Assert.Equal(tokens[0].Kind, t1kind);
            Assert.Equal(tokens[0].Text, t1text);
            Assert.Equal(tokens[1].Kind, separatorKind);
            Assert.Equal(tokens[1].Text, separatorText);
            Assert.Equal(tokens[2].Kind, t2kind);
            Assert.Equal(tokens[2].Text, t2text);
        }

        public static IEnumerable<object[]> GetTokenData()
        {
            foreach (var t in GetTokens().Concat(GetSeparators()))
                yield return new object[] { t.kind, t.text };
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
                yield return new object[] { t.t1kind, t.t1text, t.t2kind, t.t2text };
        }

        public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
                yield return new object[] { t.t1kind, t.t1text, t.separatorKind, t.separatorText, t.t2kind, t.t2text };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            var fixedTokens = Enum.GetValues(typeof(SyntaxKind))
                                .Cast<SyntaxKind>()
                                .Select(k => (kind: k, text: SyntaxRules.GetText(k)))
                                .Where(t => t.text != null);

            var dynamicTokens = new []
            {
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "123"),
            };

            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
        {
            return new[]
            {
                (SyntaxKind.WhitespaceToken, " "),
                (SyntaxKind.WhitespaceToken, "  "),
                (SyntaxKind.WhitespaceToken, "\r"),
                (SyntaxKind.WhitespaceToken, "\n"),
                (SyntaxKind.WhitespaceToken, "\r\n"),
            };
        }

        private static bool RequiresSeparator(SyntaxKind t1kind, SyntaxKind t2kind)
        {
            var t1IsKeyword = t1kind.ToString().EndsWith("Keyword");
            var t2IsKeyword = t2kind.ToString().EndsWith("Keyword");

            if (t1kind == SyntaxKind.IdentifierToken && t2kind == SyntaxKind.IdentifierToken)
                return true;
            if (t1IsKeyword && t2IsKeyword)
                return true;
            if (t1IsKeyword && t2kind == SyntaxKind.IdentifierToken)
                return true;
            if (t1kind == SyntaxKind.IdentifierToken && t2IsKeyword)
                return true;

            if (t1kind == SyntaxKind.NumberToken && t2kind == SyntaxKind.NumberToken)
                return true;
            if (t1kind == SyntaxKind.EqualToken && t2kind == SyntaxKind.EqualToken)
                return true;
            if (t1kind == SyntaxKind.EqualToken && t2kind == SyntaxKind.EqualEqualToken)
                return true;
            if (t1kind == SyntaxKind.BangToken && t2kind == SyntaxKind.EqualEqualToken)
                return true;
            if (t1kind == SyntaxKind.BangToken && t2kind == SyntaxKind.EqualToken)
                return true;
            if (t1kind == SyntaxKind.BangEqualToken && t2kind == SyntaxKind.EqualEqualToken)
                return true;
            // TODO: More cases
            return false;
        }

        private static IEnumerable<(SyntaxKind t1kind, string t1text, SyntaxKind t2kind, string t2text)> GetTokenPairs()
        {
            foreach(var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.kind, t2.kind))
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                }
            }
        }

        private static IEnumerable<(SyntaxKind t1kind, string t1text,
                                    SyntaxKind separatorKind, string separatorText,
                                    SyntaxKind t2kind, string t2text)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach(var s in GetSeparators())
                        {
                            yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                        }
                    }
                }
            }
        }
    }
}
