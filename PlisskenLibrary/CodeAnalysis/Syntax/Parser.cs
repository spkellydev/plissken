using PlisskenLibrary.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    internal sealed class Parser
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly SourceText _text;
        private readonly ImmutableArray<SyntaxToken> _tokens;

        private int _position;

        public Parser(SourceText text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EOFToken);

            _text = text;
            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            return _tokens[index];
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public CompliationUnitSyntax ParseCompliationUnit()
        {
            var statement = ParseStatement();
            var eofToken = MatchToken(SyntaxKind.EOFToken);
            return new CompliationUnitSyntax(statement, eofToken);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclaration();
            }
            return ParseExpressionStatement();
        }

        private VariableDeclarationSyntax ParseVariableDeclaration()
        {
            var expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equals = MatchToken(SyntaxKind.EqualToken);
            var initializer = ParseExpression();
            return new VariableDeclarationSyntax(keyword, identifier, equals, initializer);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

            while(Current.Kind != SyntaxKind.EOFToken &&
                  Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            // a = 10
            // a + b + 5             a = b = 5
            // [No]  +               [Yes]  =
            //      / \                    / \
            //     +   5                  a   =
            //    / \                        / \
            //   a   b                      b   5
            //

            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }
            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            // a = 10           a = 2
            // b = 5            b = 4
            // a + b            c = 5
            //                  a + (b * c)
            //
            //   +                 +
            //  / \               / \
            // a   b             a   *
            //                      / \
            //                     b   c
            //
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression();
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while(true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;
                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenToken:
                    return ParseParenthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteralExpression();

                case SyntaxKind.NumberToken:
                    return ParseNumberLiteralExpression();
                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameExpression();
            }
            
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenToken);
            return new ParenExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanLiteralExpression()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var boolKeyword = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(boolKeyword, isTrue);
        }

        private ExpressionSyntax ParseNumberLiteralExpression()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }
    }
}
