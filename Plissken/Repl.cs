using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlisskenLibrary.CodeAnalysis;
using PlisskenLibrary.CodeAnalysis.Binding;
using PlisskenLibrary.CodeAnalysis.Syntax;

namespace PlisskenLibrary
{
    // 1 + 2 * 3
    //
    //   +
    //  / \
    // 1   *
    //    / \
    //   2   3
    internal static class Repl
    {
        static void Main(string[] args)
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(showTree ? "showing parse trees." : "not showing parse trees.");
                    Console.ResetColor();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var compiler = new Compilation(syntaxTree);
                var result = compiler.Evaluate(variables);
                var diagnostics = result.Diagnostics;

                if (showTree) PrettyPrint(syntaxTree.Root);
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefix = line.Substring(0, diagnostic.Span.Start);
                        var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = line.Substring(diagnostic.Span.End);

                        Console.Write("    ");
                        Console.Write(prefix);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();
                        Console.WriteLine(suffix);
                    }
                    Console.WriteLine();
                }
            }
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
        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var mark = isLast ? "└─" : "├─";
            Console.Write(indent);
            Console.Write(mark);
            Console.Write(node.Kind);
            if (node is SyntaxToken t && t.Value != null)
                Console.Write($" {t.Value}");

            Console.WriteLine();

            var lastChild = node.GetChildren().LastOrDefault();
            indent += isLast ? "   " : "│  ";

            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
            Console.ResetColor();
        }
    }

}


