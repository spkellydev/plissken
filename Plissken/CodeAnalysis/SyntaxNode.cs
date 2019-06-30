using System.Collections.Generic;

namespace Plissken.CodeAnalysis
{
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
