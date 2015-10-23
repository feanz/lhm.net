namespace lhm.net
{
    public static class StringExtensions
    {
        public static bool IsSomething(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNothing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string GetLast(this string source, int tailLength)
        {
            return tailLength >= source.Length ?
                source :
                source.Substring(source.Length - tailLength);
        }
    }
}