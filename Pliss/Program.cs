using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlisskenLibrary.CodeAnalysis;
using PlisskenLibrary.CodeAnalysis.Binding;
using PlisskenLibrary.CodeAnalysis.Syntax;
using PlisskenLibrary.CodeAnalysis.Text;

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
            var textBuilder = new StringBuilder();
            Compilation previous = null;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                if (textBuilder.Length == 0)
                    Console.Write("» ");
                else
                    Console.Write("· ");
                Console.ResetColor();

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank) break;
                    else if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine(showTree ? "showing parse trees." : "not showing parse trees.");
                        Console.ResetColor();
                        continue;
                    }
                    else if (input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "#reset")
                    {
                        previous = null;
                        continue;
                    }
                }
                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();
                var syntaxTree = SyntaxTree.Parse(text);
                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compliation = previous == null
                                      ? new Compilation(syntaxTree) :
                                      previous.ContinueWith(syntaxTree);
                
                var result = compliation.Evaluate(variables);
                var diagnostics = result.Diagnostics;

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }


                if (!diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("⌠»⌡ ");
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                    // only remember successful attempts for now until diagnostics are more well developed
                    previous = compliation;
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var character = diagnostic.Span.Start - line.Start + 1;

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}):");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnostic.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

                        Console.Write("    ");
                        Console.Write(prefix);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();
                        Console.WriteLine(suffix);
                    }
                    Console.WriteLine();
                }

                textBuilder.Clear();
            }
        }
    }

}


