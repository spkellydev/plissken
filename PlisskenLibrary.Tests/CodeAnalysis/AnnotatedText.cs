using PlisskenLibrary.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace PlisskenLibrary.Tests.CodeAnalysis
{
    /// <summary>
    /// [<error_token>] We should expect certain errors in source text. For instance `let` is immutable, so we should not be able to reassign it
    /// </summary>
    /// text: 
    ///   {
    ///      let x = 20
    ///      x [=] 10 
    ///   }
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        //var text = @"
        //    {
        //        var c = 10
        //        let x = 2
        //    }
        //";
        public static AnnotatedText Parse (string text)
        {
            text = UnindentString(text);
            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            var position = 0;
            foreach(var c in text)
            {
                if  (c == '[')
                {
                    startStack.Push(position);
                }
                else if (c == ']')
                {
                    if (startStack.Count == 0)
                    {
                        throw new ArgumentException("Too many ']' in text", nameof(text));
                    }

                    var start = startStack.Pop();
                    var end = position;
                    var span = TextSpan.FromBounds(start, end);
                    spanBuilder.Add(span);
                }
                else
                {
                    position++;
                    textBuilder.Append(c);
                }
            }

            if (startStack.Count != 0)
                throw new ArgumentException("Too few '[' in text", nameof(text));

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        private static string UnindentString(string indentedText)
        {
            var lines = UnindentLines(indentedText);
            return string.Join(Environment.NewLine, lines);
        }

        public static string[] UnindentLines(string indentedText)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(indentedText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            var minIndentation = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                var indentation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                {
                    continue;
                }
                lines[i] = lines[i].Substring(minIndentation);
            }

            while (lines.Count > 0 && lines[0].Length == 0)
            {
                lines.RemoveAt(0);
            }

            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines.ToArray();
        }
    }
}
