using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TevanaTyper
{
    static class Program
    {
        const string consonants = "bcdfghjklmnpqrstvwyzBCDFGHKLMNPQRSTVWYZ";
        const string vowels = "aeiouAEIOU";
        const string allowed = "abcdefghijklmnopqrstuvwyzABCDEFGHIJKLMNOPQRSTUVWYZ.,+-÷*='!?1234567890\n \"@{}|~[]()<>:^%$&";
        const string delimiters = "/+-*÷=!?,.\n \"@{}|~[]()<>:^%$&";
        const string indexers = "bcdfghjklmnpqrstvwyzaeiou#'_______C___ ,.;?!}{+*-÷=\"|~[]()<>:^%$&";
        const string wrappingPunctuation = ".;!?";
        const string TILESET_PATH = @"tevana.png";
        const string OUTPUT_PATH = @"output.png";
        const int charWidth = 11, charHeight = 20, charSpacing = 2;
        const int vowelStartX = 2, vowelWidth = 7, vowelHeight = 5, vowelOffsetX = 2;

        private static Bitmap tileset;

        [STAThread]
        static void Main(string[] args)
        {
            tileset = new(TILESET_PATH);
            tileset.MakeTransparent();

            while (true)
            {
                Console.WriteLine("What would you like to write?");

                string write = string.Empty, temp;

                bool later = false;

                while (true)
                {
                    temp = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(temp)) break;
                    else
                    {
                        if (later) write += "\n";
                        later = true;

                        write += temp;
                    }
                }

                if (string.IsNullOrWhiteSpace(write)) break;

                Write(write.Trim());
            }
        }

        static void Write(string s)
        {
            s = Transform(s);

            Console.WriteLine(s);

            List<string> blocks = new();

            for (int i = 0; i != -1 && i != s.Length;) blocks.Add(IterateBlocks(s, ref i));

            if (string.IsNullOrWhiteSpace(blocks[^1])) blocks.RemoveAt(blocks.Count - 1);

            (int width, int height) = GetDimensions(blocks);

            Bitmap output = new(width, height);

            bool startWord = true;

            int x = 0, y = 0;

            for (int i = 0; i < blocks.Count; i++, x++)
            {
                if (blocks[i] == "\n")
                {
                    x = -1;
                    y++;
                    continue;
                }

                if (char.IsLetterOrDigit(blocks[i][0])) startWord = false;

                DrawBlock(output, x, y, blocks[i], startWord,
                    i != 0 && ShouldHigh(blocks[i - 1]),
                    i < blocks.Count - 1 && ShouldHigh(blocks[i + 1]));

                if (delimiters.Contains(blocks[i][^1])) startWord = true;
            }

            Bitmap padded = new(width + 4, height + 4);

            padded.Apply(output, new Rectangle(0, 0, output.Width, output.Height), new Rectangle(2, 2, output.Width, output.Height), false);

            padded.Tint(new Dictionary<int, Color>()
            {
                { Color.Black.ToArgb(), Color.FromArgb(220, 221, 222) },
            });

            padded.Save(OUTPUT_PATH);

            Borrowed.SetClipboardImage(padded, null, null);
        }

        static string Transform(string s)
        {
            s = Replace(s);

            string[] subStrings = s.Split('\n');
            string working = string.Empty;
            for (int i = 0; i < subStrings.Length; i++)
            {
                working += Match(subStrings[i]);
                if (i != subStrings.Length - 1) working += "\n";
            }
            s = working;
            

            int vowelCount = 0;

            string t = string.Empty;

            for (int i = 0; i < s.Length; i++)
            {
                if (allowed.Contains(s[i]))
                {
                    if (char.IsDigit(s[i]))
                    {
                        if (vowelCount != 0) Reset();
                        t += s[i];

                        if (i != s.Length - 1 && !delimiters.Contains(s[i + 1])) t += '/';
                    }
                    else t += s[i];

                    if (vowels.Contains(s[i]))
                    {
                        vowelCount++;

                        if (vowelCount == 1 && i < s.Length - 2 && vowels.Contains(s[i + 1]) && consonants.Contains(s[i + 2]))
                        {
                            Reset();
                        }
                        if (vowelCount == 2 && i < s.Length - 1 && vowels.Contains(s[i + 1]))
                        {
                            Reset();
                        }
                    }
                    else if (s[i] == '\'' || consonants.Contains(s[i]))
                    {
                        vowelCount = 0;

                        if (i != s.Length - 1)
                        {
                            if (s[i + 1] != '\'' && !delimiters.Contains(s[i + 1])) t += '/';
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

        static string Replace(string s)
        {
            s = s.ToLower();

            s = s.Replace('/', '÷');
            s = s.Replace("x", "ks");
            s = s.Replace("¡", "!");
            s = s.Replace("¿", "?");
            s = s.Replace(", ", ",");

            return s;
        }

        static string Match(string s)
        {
            bool wrapping = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '@' && i != s.Length - 1 && !delimiters.Contains(s[i + 1]))
                {
                    wrapping = true;
                }
                else if (wrapping && (delimiters.Contains(s[i]) || i == s.Length - 1))
                {
                    wrapping = false;
                    if (s[i] == '@') continue;

                    if (i == s.Length - 1 && !delimiters.Contains(s[i])) i++;
                    s = s.Insert(i, "@");
                }
            }

            return s;
        }

        static string IterateBlocks(string s, ref int i)
        {
            string t = string.Empty;

            if (delimiters.Contains(s[i]))
            {
                if (s[i] == '/') i++;
                else return s[i++].ToString();
            }

            for (int j = i; j < s.Length; j++)
            {
                t += s[j];
                if (j < s.Length - 1 && delimiters.Contains(s[j + 1]))
                {
                    i = j + 1;
                    return t;
                }
            }

            i = -1;
            return t;
        }

        static bool ShouldHigh(string block) => consonants.Contains(block[^1]) || (block.Length > 1 && block[^1] == '\'' && consonants.Contains(block[^2])) || char.IsDigit(block[0]);

        static int GetTilemapSlotX(int x) => x * (charWidth + charSpacing);

        static (int, int) GetDimensions(List<string> blocks)
        {
            int rows = blocks.Count(x => x == "\n") + 1;

            int currentRow = 0, maxRowLength = 0, currentRowLength = 0;
            foreach (string block in blocks)
            {
                currentRowLength++;

                if (block == "\n")
                {
                    currentRow++;
                    if (currentRowLength - 1 > maxRowLength) maxRowLength = currentRowLength - 1;
                    currentRowLength = 0;
                }
            }

            if (currentRowLength > maxRowLength) maxRowLength = currentRowLength;

            int columns = maxRowLength;

            int width = columns * charWidth;
            int height = rows * (charHeight + 2);

            return (width, height);
        }

        static void DrawBlock(Bitmap bitmap, int x, int y, string block, bool startWord, bool beforeUpper, bool nextUpper)
        {
            x *= charWidth;
            y *= (charHeight + 2);

            Bitmap thisBlock = new(charWidth, charHeight);

            if (delimiters.Contains(block))
            {
                if (block[0] == '@') bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(GetCapital(startWord, beforeUpper, nextUpper)), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);
                else bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf(block)), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);
            }
            else if (consonants.Contains(block[^1]))
            {
                bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^1]))), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);
                if (block.Length > 1) bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^2]))), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), true);
            }
            else if (block[^1] == '\'')
            {
                if (block.Length > 1 && consonants.Contains(block[^2]))
                {
                    bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf('\'')), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);

                    bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^2]))), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), true);
                    if (block.Length > 2) bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^3]))), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), true);
                }
                else
                {
                    bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(GetApostrophe(block, beforeUpper, nextUpper)), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);

                    if (block.Length == 2)
                    {
                        bitmap.Apply(tileset,
                        new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^2]))) + vowelStartX, 0, vowelWidth, vowelHeight),
                        new Rectangle(x + vowelOffsetX + LowerOffset(beforeUpper, nextUpper), y + 10, vowelWidth, vowelHeight), true);
                    }
                    else if (block.Length == 3)
                    {
                        bitmap.Apply(tileset,
                            new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^3]))) + vowelStartX, 0, vowelWidth, vowelHeight),
                            new Rectangle(x + vowelOffsetX + LowerOffset(beforeUpper, nextUpper), y + 6, vowelWidth, vowelHeight), true);

                        bitmap.Apply(tileset,
                            new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^2]))) + vowelStartX, 0, vowelWidth, vowelHeight),
                            new Rectangle(x + vowelOffsetX + LowerOffset(beforeUpper, nextUpper), y + 12, vowelWidth, vowelHeight), true);
                    }
                }

            }
            else if (char.IsDigit(block[0]))
            {
                bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(GetNumberSlot(int.Parse(block))), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);
                bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(indexers.IndexOf('#')), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), true);
            }
            else
            {
                bitmap.Apply(tileset, new Rectangle(GetTilemapSlotX(GetLowerSlot(beforeUpper, nextUpper)), 0, charWidth, charHeight), new Rectangle(x, y, charWidth, charHeight), false);

                if (block.Length == 1)
                {
                    bitmap.Apply(tileset,
                        new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^1]))) + vowelStartX, 0, vowelWidth, vowelHeight),
                        new Rectangle(x + vowelOffsetX + LowerOffset(beforeUpper, nextUpper), y + 10, vowelWidth, vowelHeight), true);
                }
                else if (block.Length == 2)
                {
                    bitmap.Apply(tileset,
                        new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^2]))) + vowelStartX, 0, vowelWidth, vowelHeight),
                        new Rectangle(x + vowelOffsetX + LowerOffset(beforeUpper, nextUpper), y + 6, vowelWidth, vowelHeight), true);

                    bitmap.Apply(tileset,
                        new Rectangle(GetTilemapSlotX(indexers.IndexOf(char.ToLower(block[^1]))) + vowelStartX, 0, vowelWidth, vowelHeight),
                        new Rectangle(x + vowelOffsetX + LowerOffset(beforeUpper, nextUpper), y + 12, vowelWidth, vowelHeight), true);
                }
            }
        }

        public static void Apply(this Bitmap destination, Bitmap source, Rectangle sourcePos, Rectangle destinationPos, bool alpha)
        {
            for (int x = 0; x < sourcePos.Width; x++)
            {
                for (int y = 0; y < sourcePos.Height; y++)
                {
                    if (!alpha || source.GetPixel(x + sourcePos.X, y + sourcePos.Y).A > destination.GetPixel(x + destinationPos.X, y + destinationPos.Y).A) destination.SetPixel(x + destinationPos.X, y + destinationPos.Y, source.GetPixel(x + sourcePos.X, y + sourcePos.Y));
                }
            }
        }

        public static void Tint(this Bitmap source, Dictionary<int, Color> converts)
        {
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    int key = source.GetPixel(x, y).ToArgb();

                    if (converts.ContainsKey(key))
                    {
                        source.SetPixel(x, y, converts[key]);
                    }
                }
            }
        }

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
            int apostropheIndex = indexers.IndexOf('\'');

            if (block.Length == 1) return apostropheIndex;

            if (!beforeUpper && !nextUpper) return apostropheIndex + 1;
            if (!beforeUpper && nextUpper) return apostropheIndex + 2;
            if (beforeUpper && nextUpper) return apostropheIndex + 3;

            return apostropheIndex;

            //throw new ArgumentException("Improper apostrophe usage detected.");
        }

        public static int GetCapital(bool startWord, bool beforeUpper, bool nextUpper)
        {
            int capitalIndex = indexers.IndexOf('C');

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
    }
}
