namespace MenuV
{
    public static class StringExtension
    {
        public static string FirstCharUpper(this string text)
        {
            return !string.IsNullOrWhiteSpace(text)
                ? $"{text.Substring(0, 1).ToUpper()}{text.Substring(1)}"
                : string.Empty;
        }
    }
}
