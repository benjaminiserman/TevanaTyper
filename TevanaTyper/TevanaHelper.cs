using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TevanaTyper
{
    public static class TevanaHelper
    {
        public static int GetLowerSlot(bool beforeUpper, bool nextUpper) => (beforeUpper, nextUpper) switch
        {
            (false, false) => 30,
            (true, false) => 31,
            (true, true) => 32,
            (false, true) => 33,
        };

        public static int GetApostrophe(string block, bool beforeUpper, bool nextUpper)
        {
            int apostropheIndex = '\''.Index();

            return block.Length == 1
                ? apostropheIndex
                : (beforeUpper, nextUpper) switch
                {
                    (false, false) => apostropheIndex + 1,
                    (false, true) => apostropheIndex + 2,
                    (true, true) => apostropheIndex + 3,
                    (true, false) => apostropheIndex,
                };
        }

        public static int GetCapital(bool startWord, bool beforeUpper, bool nextUpper)
        {
            int capitalIndex = 'C'.Index();

            return startWord 
                ? nextUpper 
                    ? capitalIndex 
                    : capitalIndex + 2 
                : beforeUpper 
                    ? capitalIndex + 1 
                    : capitalIndex + 3;
        }

        public static int LowerOffset(bool beforeUpper, bool nextUpper) => (beforeUpper, nextUpper) switch
        {
            (true, false) => +2,
            (false, true) => -2,
            _ => 0
        };

        public static int GetNumberSlot(int x) => x == 0 ? 9 : x - 1;

        public static string Match(string s)
        {
            bool wrapping = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '@' && i != s.Length - 1 && !s[i + 1].IsDelimiter())
                {
                    wrapping = true;
                }
                else if (wrapping && (s[i].IsDelimiter() || i == s.Length - 1))
                {
                    wrapping = false;
                    if (s[i] == '@') continue;

                    if (i == s.Length - 1 && !s[i].IsDelimiter()) i++;
                    s = s.Insert(i, "@");
                }
            }

            return s;
        }

        public static bool ShouldHigh(string block) => block[^1].IsConsonant() || (block.Length > 1 && block[^1] == '\'' && block[^2].IsConsonant()) || char.IsDigit(block[0]);
    }
}
