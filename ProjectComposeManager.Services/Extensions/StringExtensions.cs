namespace ProjectComposeManager.Services.Extensions
{
    public static class StringExtensions
    {
        private const string tabChar = "  ";

        public static string RemoveDoubleAndSingleQuotes(this string input)
        {
            return input.Trim().Replace("\'", string.Empty).Replace("\"", string.Empty);
        }

        public static bool HasIndentationOf(this string input, int indentation)
        {
            string indentString = GetIndentString(indentation);

            return !string.IsNullOrWhiteSpace(input) && input.StartsWith(indentString) && !input.StartsWith($"{tabChar}{indentString}");
        }

        public static bool HasIndentationOfAtLeast(this string input, int indentation)
        {
            string indentString = GetIndentString(indentation);

            return !string.IsNullOrWhiteSpace(input) && input.StartsWith(indentString);
        }

        private static string GetIndentString(int indentation)
        {
            string indentString = string.Empty;

            for (int i = 0; i < indentation; i++)
            {
                indentString += tabChar;
            }
            
            return indentString;
        }
    }
}
