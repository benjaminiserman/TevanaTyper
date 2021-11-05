using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TevanaTyper
{
    public static class Drawer // actually making the characters is handled in Tileset.cs
    {
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
    }
}
