namespace TevanaTyper;
using System.Collections.Generic;
using System.Drawing;

public static class Drawer // actually making the characters is handled in Tileset.cs
{
    /// <summary>
    /// Copies a rectangle <paramref name="sourcePos"/> from <paramref name="source"/> and copies it onto the rectangle <paramref name="destinationPos"/> on <paramref name="destination"/>.
    /// </summary>
    /// <param name="destination">The bitmap to copy to.</param>
    /// <param name="source">The bitmap to copy from.</param>
    /// <param name="sourcePos">The bounds of the rectangle to take from <paramref name="source"/>.</param>
    /// <param name="destinationPos">The bounds of the rectangle to place on <paramref name="destination"/>.</param>
    /// <param name="alpha">Whether or not to consider pixel alpha when overwriting <paramref name="destination"/>.</param>
    public static void Apply(this Bitmap destination, Bitmap source, Rectangle sourcePos, Rectangle destinationPos, bool alpha)
    {
        for (int x = 0; x < sourcePos.Width; x++)
        {
            for (int y = 0; y < sourcePos.Height; y++)
            {
                if (!alpha || source.GetPixel(x + sourcePos.X, y + sourcePos.Y).A > destination.GetPixel(x + destinationPos.X, y + destinationPos.Y).A)
                {
                    destination.SetPixel(x + destinationPos.X, y + destinationPos.Y, source.GetPixel(x + sourcePos.X, y + sourcePos.Y));
                }
            }
        }
    }

    /// <summary>
    /// Changes all of <paramref name="source"/>'s colors found in <paramref name="converts"/> keys with their respective values.
    /// </summary>
    /// <param name="source">The image to tint.</param>
    /// <param name="converts">A dictionary defining which colors to change into which other colors.</param>
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
