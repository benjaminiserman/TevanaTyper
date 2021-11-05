namespace TevanaTyper
{
    public static class CharExtensions
    {
        public static bool IsConsonant(this char c) => "bcdfghjklmnpqrstvwyzBCDFGHKLMNPQRSTVWYZ".Contains(c);
        public static bool IsVowel(this char c) => "aeiouAEIOU".Contains(c);
        public static bool IsAllowed(this char c) => "abcdefghijklmnopqrstuvwyzABCDEFGHIJKLMNOPQRSTUVWYZ.,+-÷*='!?1234567890\n \"@{}|~[]()<>:^%$&".Contains(c);
        public static bool IsDelimiter(this char c) => "/+-*÷=!?,.\n \"@{}|~[]()<>:^%$&".Contains(c);
        public static int Index(this char c) => "bcdfghjklmnpqrstvwyzaeiou#'_______C___ ,.;?!}{+*-÷=\"|~[]()<>:^%$&".IndexOf(c);
        public static bool IsWrappingPunctuation(this char c) => ".;!?".Contains(c);
    }
}
