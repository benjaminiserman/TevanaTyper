using System;

namespace TevanaTyper
{
    public static class TevanaHelper
    {
        /// <summary>
        /// Gets the correct line character to fit the surrounding conditions.
        /// </summary>
        /// <param name="beforeUpper">Whether or not it is after an upper-lined character.</param>
        /// <param name="nextUpper">Whether or not is before an upper-lined character. </param>
        /// <returns>The index of the correct character.</returns>
        public static int GetLowerSlot(bool beforeUpper, bool nextUpper)
        {
            if (!beforeUpper && !nextUpper) return 30;
            if (beforeUpper && !nextUpper) return 31;
            if (beforeUpper && nextUpper) return 32;
            if (!beforeUpper && nextUpper) return 33;

            throw new ArgumentException("Impossible parameters passed. Are you running this on a quantum computer?");
        }

        /// <summary>
        /// Gets the correct apostrophe character to fit the surrounding conditions.
        /// </summary>
        /// <param name="block">The given tevana block.</param>
        /// <param name="beforeUpper">Whether or not it is after an upper-lined character.</param>
        /// <param name="nextUpper">Whether or not is before an upper-lined character. </param>
        /// <returns>The index of the correct character.</returns>
        public static int GetApostrophe(string block, bool beforeUpper, bool nextUpper)
        {
            int apostropheIndex = '\''.Index();

            if (block.Length == 1) return apostropheIndex;

            if (!beforeUpper && !nextUpper) return apostropheIndex + 1;
            if (!beforeUpper && nextUpper) return apostropheIndex + 2;
            if (beforeUpper && nextUpper) return apostropheIndex + 3;

            return apostropheIndex;
        }
        
        /// <summary>
        /// Gets the correct capital character to fit the surrounding conditions.
        /// </summary>
        /// <param name="startWord">Whether or not it is the start of a word.</param>
        /// <param name="beforeUpper">Whether or not it is after an upper-lined character.</param>
        /// <param name="nextUpper">Whether or not is before an upper-lined character. </param>
        /// <returns>The index of the correct character.</returns>
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

        /// <summary>
        /// Gets the tevana character associated with the digit <paramref name="x"/>.
        /// </summary>
        /// <param name="x">The given digit.</param>
        /// <returns>The tevana character associated with <paramref name="x"/>.</returns>
        public static int GetNumberSlot(int x) => x == 0 ? 9 : x - 1;

        /// <summary>
        /// Adds @ to the end of words beginning with @ within the string <paramref name="s"/>.
        /// </summary>
        /// <param name="s">The given string.</param>
        /// <returns>A reference to parameter <paramref name="s"/>.</returns>
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

        /// <summary>
        /// Determines whether or not the given <paramref name="block"/> should be upper or lower-lined.
        /// </summary>
        /// <param name="block">The given block.</param>
        /// <returns>Whether or not the given <paramref name="block"/> should be upper or lower-lined.</returns>
        public static bool ShouldHigh(string block) => block[^1].IsConsonant() || (block.Length > 1 && block[^1] == '\'' && block[^2].IsConsonant()) || char.IsDigit(block[0]);
    }
}
