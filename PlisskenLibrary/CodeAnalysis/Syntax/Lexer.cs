using System.Collections.Generic;

namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;
        private int _start;
        private object _value;
        private SyntaxKind _kind;

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        public SyntaxToken Lex()
        {
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            // <operator> + - / ( )
            switch (Current)
            {
                case '+':
                {
                    _kind = SyntaxKind.PlusToken;
                    _position++;
                    break;
                }
                case '-':
                {
                    _kind = SyntaxKind.MinusToken;
                    _position++;
                    break;
                }
                case '*':
                {
                    _kind = SyntaxKind.StarToken;
                    _position++;
                    break;
                }

                case '/':
                {
                    _kind = SyntaxKind.ForwardSlashToken;
                    _position++;
                    break;
                }

                case '(':
                {
                    _kind = SyntaxKind.OpenParenToken;
                    _position++;
                    break;
                }
                case ')':
                {
                    _kind = SyntaxKind.CloseParenToken;
                    _position++;
                    break;
                }
                case '&':
                {
                    if (Lookahead == '&')
                    {
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                        _position += 2;
                    }
                    break;
                }
                case '|':
                {
                    if (Lookahead == '|')
                    {
                        _kind = SyntaxKind.PipePipeToken;
                        _position += 2;
                    }
                    break;
                }
                case '=':
                {
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.EqualToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.EqualEqualToken;
                        _position++;
                    }
                    break;
                }
                case '!':
                {
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.BangToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.BangEqualToken;
                        _position++;
                    }
                    break;
                }
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                {
                    ReadNumberToken();
                    break;
                }
                case ' ':  case '\t': case '\n': case '\r':
                {
                    ReadWhiteSpaceToken();
                    break;
                }
                case '\0':
                {
                    _kind = SyntaxKind.EOFToken;
                    break;
                }
                default:
                {
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    // <whitespace>
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpaceToken();
                    }
                    else
                    {
                        _diagnostics.ReportBadCharacter(_position, Current);
                        _position++;
                    }
                    break;
                }
            }

            var text = SyntaxRules.GetText(_kind);
            var length = _position - _start;
            if (text == null)
            {
                text = _text.Substring(_start, length);
            }
            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadWhiteSpaceToken()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _text.Substring(_start, length);
            if (!int.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), _text, typeof(int));

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                _position++;

            var length = _position - _start;
            var text = _text.Substring(_start, length);
            _kind = SyntaxRules.GetKeywordKind(text);
        }
    }
}
