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

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }
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
                    var text = syntaxTree.Text;
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}):");
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
    }

}


