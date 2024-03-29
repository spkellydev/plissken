﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlisskenLibrary.CodeAnalysis.Syntax;
using PlisskenLibrary.CodeAnalysis.Text;

namespace PlisskenLibrary.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics) => _diagnostics.AddRange(diagnostics);

        public void Concat(DiagnosticBag diagnostics) => _diagnostics.Concat(diagnostics);

        private void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"The number {text} isn't valid {type}";
            Report(span, message);
        }

        public void ReportBadCharacter(int position, char character)
        {
            var span = new TextSpan(position, 1);
            var message = $"Bad character input: '{character}'";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorTtext, Type type)
        {
            var message = $"Unary operator '{operatorTtext}' is not defined for type {type}";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string text, Type leftType, Type rightType)
        {
            var message = $"Binary operator '{text}' is not defined for type {leftType} and {rightType}";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Variable '{name}' doesn't exist";
            Report(span, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"Variable '{name}' is already declared";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, Type boundExpressionType, Type variableType)
        {
            var message = $"Cannot convert type'{boundExpressionType}' to type {variableType}";
            Report(span, message);
        }

        internal void ReportCannotAssign(TextSpan span, string name)
        {
            var message = $"Variable '{name}' is readonly, cannot re-assign";
            Report(span, message);
        }
    }
}
