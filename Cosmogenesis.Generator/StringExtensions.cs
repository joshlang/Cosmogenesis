using System.Linq;

namespace Cosmogenesis.Generator
{
    static class StringExtensions
    {
        public static string AddSuffix(this string s, string suffix) =>
            s.EndsWith(suffix) 
            ? s 
            : s + suffix;

        public static string Parameterify(this string s) =>
            string.IsNullOrEmpty(s)
            ? s
            : char.ToLower(s[0]) + s.Substring(1);

        public static string CSharpify(this string s) =>
            string.IsNullOrEmpty(s)
            ? s
            : char.ToUpper(s[0]) + s.Substring(1);

        static readonly string[] PluralEndings = new[] { "s", "sh", "ch", "x", "z" };
        public static string Pluralize(this string singular) => PluralEndings.Any(singular.EndsWith)
            ? $"{singular}es"
            : $"{singular}s"; // This is 100% correct for all English words in existance forever without any exception, definitely for sure.

    }
}
