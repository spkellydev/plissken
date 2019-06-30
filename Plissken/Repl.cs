using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plissken.CodeAnalysis;

namespace Plissken
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

                if (showTree) PrettyPrint(syntaxTree.Root);
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                if (!syntaxTree.Diagnostics.Any())
                {
                    var evalutor = new Evaluator(syntaxTree.Root);
                    var result = evalutor.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach (var diagnostic in syntaxTree.Diagnostics)
                        Console.WriteLine(diagnostic);
                    Console.ResetColor();
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


