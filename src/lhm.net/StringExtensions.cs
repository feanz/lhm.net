using System;

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

        public static string GetLast(this string value, int tailLength)
        {
            return tailLength >= value.Length ?
                value :
                value.Substring(value.Length - tailLength);
        }

        public static string Truncate(this string value, int maxLength)
        {
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}