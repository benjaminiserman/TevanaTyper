using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TevanaTyper
{
    static class Program
    {
        const string TILESET_PATH = @"tevana.png";
        const string OUTPUT_PATH = @"output.png";

        [STAThread]
        static void Main()
        {
            Tileset tileset = new(@"tevana.png", 11, 20, 2, 2, 2, 7, 5);

            while (true)
            {
                Console.WriteLine("What would you like to write?");

                string write = string.Empty;

                bool later = false;

                while (true)
                {
                    string temp = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(temp)) break;
                    else
                    {
                        if (later) write += "\n";
                        later = true;

                        write += temp;
                    }
                }

                if (string.IsNullOrWhiteSpace(write)) break;

                Write(write.Trim(), tileset);
            }
        }

        static void Write(string s, Tileset tileset)
        {
            s = Transformer.Transform(s);

            Console.WriteLine(s);

            List<string> blocks = new();

            for (int i = 0; i != -1 && i != s.Length;) blocks.Add(Transformer.IterateBlocks(s, ref i));

            if (string.IsNullOrWhiteSpace(blocks[^1])) blocks.RemoveAt(blocks.Count - 1);

            (int width, int height) = tileset.GetDimensions(blocks);

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

                tileset.DrawBlock(output, x, y, blocks[i], startWord,
                    i != 0 && TevanaHelper.ShouldHigh(blocks[i - 1]),
                    i < blocks.Count - 1 && TevanaHelper.ShouldHigh(blocks[i + 1]));

                if (blocks[i][^1].IsDelimiter()) startWord = true;
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
    }
}
