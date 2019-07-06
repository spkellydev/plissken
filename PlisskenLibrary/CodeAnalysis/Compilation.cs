using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using PlisskenLibrary.CodeAnalysis.Binding;
using PlisskenLibrary.CodeAnalysis.Syntax;

namespace PlisskenLibrary.CodeAnalysis
{
    public class Compilation
    {
        private BoundGlobalScope _globalScope;
        public Compilation(SyntaxTree syntaxTree)
            : this(null, syntaxTree)
        {}

        private Compilation(Compilation previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }
        public Compilation Previous { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    // ensure thread safety
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var boundExpression = GlobalScope.Statement;

            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToArray();
            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics.ToImmutableArray(), null);
            }
            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}
