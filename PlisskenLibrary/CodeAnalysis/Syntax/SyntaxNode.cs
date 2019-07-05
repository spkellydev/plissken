using PlisskenLibrary.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PlisskenLibrary.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    yield return (SyntaxNode)property.GetValue(this);
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                    foreach(var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        /// <summary>
        /// PrettyPrint intended to write a tree similar to unix tree command
        /// </summary>
        /// <param name="node">current node</param>
        /// <param name="indent">indent level</param>
        /// path/to/folder/
        /// ├── a-first.html
        /// ├── b-second.txt
        /// ├── subfolder
        /// │   ├── readme.txt
        /// │   ├── code.cpp
        /// │   └── code.h
        /// └── z-last-file.txt
        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var mark = isLast ? "└─" : "├─";
            writer.Write(indent);
            writer.Write(mark);
            writer.Write(node.Kind);
            if (node is SyntaxToken t && t.Value != null)
                writer.Write($" {t.Value}");

            writer.WriteLine();

            var lastChild = node.GetChildren().LastOrDefault();
            indent += isLast ? "   " : "│  ";

            foreach (var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
