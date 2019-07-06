using PlisskenLibrary.CodeAnalysis.Text;
using Xunit;

namespace PlisskenLibrary.Tests.CodeAnalysis.Syntax
{
    public class SourceTextTest
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int expectedLastLine)
        {
            var sourceText = SourceText.From(text);
            Assert.Equal(expectedLastLine, sourceText.Lines.Length);
        }
    }
}
