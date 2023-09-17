namespace ProjectComposeManager.Services
{
    using System.Linq;
    using System.Text;

    internal static class StringBuilderExtensions
    {
        const string TabChar = "  ";

        internal static StringBuilder AppendLineWithIndentation(this StringBuilder stringBuilder, int indentaionLevel, string line)
        {
            return stringBuilder.AppendLine($"{string.Concat(Enumerable.Repeat(TabChar, indentaionLevel))}{line}");
        }
    }
}
