using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TevanaTyper
{
    public static class Transformer
    {
        public static string Transform(string s)
        {
            s = Replace(s);

            string[] subStrings = s.Split('\n');
            string working = string.Empty;
            for (int i = 0; i < subStrings.Length; i++)
            {
                working += TevanaHelper.Match(subStrings[i]);
                if (i != subStrings.Length - 1) working += "\n";
            }

            s = working;

            int vowelCount = 0;
            string t = string.Empty;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].IsAllowed())
                {
                    if (char.IsDigit(s[i]))
                    {
                        if (vowelCount != 0) Reset();
                        t += s[i];

                        if (i != s.Length - 1 && !s[i + 1].IsDelimiter()) t += '/';
                    }
                    else t += s[i];

                    if (s[i].IsVowel())
                    {
                        vowelCount++;

                        if ((vowelCount == 1 && i < s.Length - 2 && s[i + 1].IsVowel() && s[i + 2].IsConsonant()) ||
                            (vowelCount == 2 && i < s.Length - 1 && s[i + 1].IsVowel()))
                        {
                            Reset();
                        }
                    }
                    else if (s[i] == '\'' || s[i].IsConsonant())
                    {
                        vowelCount = 0;

                        if (i != s.Length - 1)
                        {
                            if (s[i + 1] != '\'' && !s[i + 1].IsDelimiter()) t += '/';
                        }
                    }
                    else
                    {
                        vowelCount = 0;
                    }
                }
            }

            return t;

            void Reset()
            {
                t += '/';
                vowelCount = 0;
            }
        }
        public static string IterateBlocks(string s, ref int i)
        {
            string t = string.Empty;

            if (s[i].IsDelimiter())
            {
                if (s[i] == '/') i++;
                else return s[i++].ToString();
            }

            for (int j = i; j < s.Length; j++)
            {
                t += s[j];
                if (j < s.Length - 1 && s[j + 1].IsDelimiter())
                {
                    i = j + 1;
                    return t;
                }
            }

            i = -1;
            return t;
        }

        private static string Replace(string s)
        {
            s = s.ToLower();

            s = s.Replace('/', '÷');
            s = s.Replace("x", "ks");
            s = s.Replace("¡", "!");
            s = s.Replace("¿", "?");
            s = s.Replace(", ", ",");

            return s;
        }
    }
}
