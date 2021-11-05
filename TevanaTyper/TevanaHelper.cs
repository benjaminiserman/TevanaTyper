using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TevanaTyper
{
    public static class TevanaHelper
    {
        public static int GetLowerSlot(bool beforeUpper, bool nextUpper)
        {
            if (!beforeUpper && !nextUpper) return 30;
            if (beforeUpper && !nextUpper) return 31;
            if (beforeUpper && nextUpper) return 32;
            if (!beforeUpper && nextUpper) return 33;

            throw new ArgumentException("Impossible parameters passed. Are you running this on a quantum computer?");
        }

        public static int GetApostrophe(string block, bool beforeUpper, bool nextUpper)
        {
            int apostropheIndex = '\''.Index();

            if (block.Length == 1) return apostropheIndex;

            if (!beforeUpper && !nextUpper) return apostropheIndex + 1;
            if (!beforeUpper && nextUpper) return apostropheIndex + 2;
            if (beforeUpper && nextUpper) return apostropheIndex + 3;

            return apostropheIndex;
        }

        public static int GetCapital(bool startWord, bool beforeUpper, bool nextUpper)
        {
            int capitalIndex = 'C'.Index();

            if (startWord)
            {
                if (nextUpper) return capitalIndex;
                else return capitalIndex + 2;
            }
            else
            {
                if (beforeUpper) return capitalIndex + 1;
                else return capitalIndex + 3;
            }
        }

        public static int LowerOffset(bool beforeUpper, bool nextUpper)
        {
            if (beforeUpper && !nextUpper) return +2;
            if (!beforeUpper && nextUpper) return -2;

            return 0;
        }

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
