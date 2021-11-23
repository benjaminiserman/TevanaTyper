using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TevanaTyper
{
    class Tileset
    {
        public string TilesetPath { get; init; }

        public int CharWidth { get; init; }
        public int CharHeight { get; init; }
        public int CharSpacing { get; init; }

        public int VowelStartX { get; init; }
        public int VowelOffsetX { get; init; }
        public int VowelWidth { get; init; }
        public int VowelHeight { get; init; }

        private readonly Bitmap _raw;

        public Tileset(string tilesetPath, int charWidth, int charHeight, int charSpacing, int vowelStartX, int vowelOffsetX, int vowelWidth, int vowelHeight)
        {
            TilesetPath = tilesetPath;
            _raw = new(TilesetPath);
            _raw.MakeTransparent();

            CharWidth = charWidth;
            CharHeight = charHeight;
            CharSpacing = charSpacing;

            VowelStartX = vowelStartX;
            VowelOffsetX = vowelOffsetX;
            VowelWidth = vowelWidth;
            VowelHeight = vowelHeight;
        }

        private int GetTilemapSlotX(int x) => x * (CharWidth + CharSpacing);

        public (int, int) GetDimensions(List<string> blocks)
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

            int width = columns * CharWidth;
            int height = rows * (CharHeight + 2);

            return (width, height);
        }

        public void DrawBlock(Bitmap bitmap, int x, int y, string block, bool startWord, bool beforeUpper, bool nextUpper)
        {
            x *= CharWidth;
            y *= CharHeight + 2;

            Bitmap thisBlock = new(CharWidth, CharHeight);

            if (block[0].IsDelimiter())
            {
                if (block[0] == '@') bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(TevanaHelper.GetCapital(startWord, beforeUpper, nextUpper)), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);
                else bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(block[0].Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);
            }
            else if (block[^1].IsConsonant())
            {
                bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(block[^1].Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);
                if (block.Length > 1) bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(char.ToLower(block[^2]).Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), true);
            }
            else if (block[^1] == '\'')
            {
                if (block.Length > 1 && block[^2].IsConsonant())
                {
                    bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX('\''.Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);

                    bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(char.ToLower(block[^2]).Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), true);
                    if (block.Length > 2) bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(char.ToLower(block[^3]).Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), true);
                }
                else
                {
                    bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(TevanaHelper.GetApostrophe(block, beforeUpper, nextUpper)), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);

                    if (block.Length == 2)
                    {
                        bitmap.Apply(_raw,
                        new Rectangle(GetTilemapSlotX(char.ToLower(block[^2]).Index()) + VowelStartX, 0, VowelWidth, VowelHeight),
                        new Rectangle(x + VowelOffsetX + TevanaHelper.LowerOffset(beforeUpper, nextUpper), y + 10, VowelWidth, VowelHeight), true);
                    }
                    else if (block.Length == 3)
                    {
                        bitmap.Apply(_raw,
                            new Rectangle(GetTilemapSlotX(char.ToLower(block[^3]).Index()) + VowelStartX, 0, VowelWidth, VowelHeight),
                            new Rectangle(x + VowelOffsetX + TevanaHelper.LowerOffset(beforeUpper, nextUpper), y + 6, VowelWidth, VowelHeight), true);

                        bitmap.Apply(_raw,
                            new Rectangle(GetTilemapSlotX(char.ToLower(block[^2]).Index()) + VowelStartX, 0, VowelWidth, VowelHeight),
                            new Rectangle(x + VowelOffsetX + TevanaHelper.LowerOffset(beforeUpper, nextUpper), y + 12, VowelWidth, VowelHeight), true);
                    }
                }
            }
            else if (char.IsDigit(block[0]))
            {
                bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(TevanaHelper.GetNumberSlot(int.Parse(block))), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);
                bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX('#'.Index()), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), true);
            }
            else
            {
                bitmap.Apply(_raw, new Rectangle(GetTilemapSlotX(TevanaHelper.GetLowerSlot(beforeUpper, nextUpper)), 0, CharWidth, CharHeight), new Rectangle(x, y, CharWidth, CharHeight), false);

                if (block.Length == 1)
                {
                    bitmap.Apply(_raw,
                        new Rectangle(GetTilemapSlotX(char.ToLower(block[^1]).Index()) + VowelStartX, 0, VowelWidth, VowelHeight),
                        new Rectangle(x + VowelOffsetX + TevanaHelper.LowerOffset(beforeUpper, nextUpper), y + 10, VowelWidth, VowelHeight), true);
                }
                else if (block.Length == 2)
                {
                    bitmap.Apply(_raw,
                        new Rectangle(GetTilemapSlotX(char.ToLower(block[^2]).Index()) + VowelStartX, 0, VowelWidth, VowelHeight),
                        new Rectangle(x + VowelOffsetX + TevanaHelper.LowerOffset(beforeUpper, nextUpper), y + 6, VowelWidth, VowelHeight), true);

                    bitmap.Apply(_raw,
                        new Rectangle(GetTilemapSlotX(char.ToLower(block[^1]).Index()) + VowelStartX, 0, VowelWidth, VowelHeight),
                        new Rectangle(x + VowelOffsetX + TevanaHelper.LowerOffset(beforeUpper, nextUpper), y + 12, VowelWidth, VowelHeight), true);
                }
            }
        }
    }
}
