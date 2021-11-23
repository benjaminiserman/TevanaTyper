namespace TevanaTyper;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

static class Borrowed // Thanks Nyerguds, you're a god!
{
    /// <summary>
    /// Copies the given image to the clipboard as PNG, DIB and standard Bitmap format.
    /// </summary>
    /// <param name="image">Image to put on the clipboard.</param>
    /// <param name="imageNoTr">Optional specifically nontransparent version of the image to put on the clipboard.</param>
    /// <param name="data">Clipboard data object to put the image into. Might already contain other stuff. Leave null to create a new one.</param>
    public static void SetClipboardImage(Bitmap image, Bitmap imageNoTr, DataObject data)
    {
        Clipboard.Clear();
        data ??= new DataObject();
        imageNoTr ??= image;
        using MemoryStream pngMemStream = new();
        using MemoryStream dibMemStream = new();

        // As standard bitmap, without transparency support
        data.SetData(DataFormats.Bitmap, imageNoTr, true);
        // As PNG. Gimp will prefer this over the other two.
        image.Save(pngMemStream, ImageFormat.Png);
        data.SetData("PNG", pngMemStream, false);
        // As DIB. This is (wrongly) accepted as ARGB by many applications.
        byte[] dibData = ConvertToDib(image);
        dibMemStream.Write(dibData, 0, dibData.Length);
        data.SetData(DataFormats.Dib, dibMemStream, false);
        // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
        Clipboard.SetDataObject(data, true);
    }

    /// <summary>
    /// Converts the image to Device Independent Bitmap format of type BITFIELDS.
    /// This is (wrongly) accepted by many applications as containing transparency,
    /// so I'm abusing it for that.
    /// </summary>
    /// <param name="image">Image to convert to DIB</param>
    /// <returns>The image converted to DIB, in bytes.</returns>
    public static byte[] ConvertToDib(Image image)
    {
        byte[] bm32bData;
        int width = image.Width;
        int height = image.Height;
        // Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
        using (Bitmap bm32b = new(image.Width, image.Height, PixelFormat.Format32bppArgb))
        {
            using (Graphics gr = Graphics.FromImage(bm32b))
                gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
            // Bitmap format has its lines reversed.
            bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
            bm32bData = GetImageData(bm32b, out int stride);
        }
        // BITMAPINFOHEADER struct for DIB.
        int hdrSize = 0x28;
        byte[] fullImage = new byte[hdrSize + 12 + bm32bData.Length];
        //Int32 biSize;
        WriteIntToByteArray(fullImage, 0x00, 4, true, (uint)hdrSize);
        //Int32 biWidth;
        WriteIntToByteArray(fullImage, 0x04, 4, true, (uint)width);
        //Int32 biHeight;
        WriteIntToByteArray(fullImage, 0x08, 4, true, (uint)height);
        //Int16 biPlanes;
        WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
        //Int16 biBitCount;
        WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
        //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
        WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
        //Int32 biSizeImage;
        WriteIntToByteArray(fullImage, 0x14, 4, true, (uint)bm32bData.Length);
        // These are all 0. Since .net clears new arrays, don't bother writing them.
        //Int32 biXPelsPerMeter = 0;
        //Int32 biYPelsPerMeter = 0;
        //Int32 biClrUsed = 0;
        //Int32 biClrImportant = 0;

        // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
        WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
        WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
        WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
        Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
        return fullImage;
    }

    public static void WriteIntToByteArray(byte[] data, int startIndex, int bytes, bool littleEndian, uint value)
    {
        int lastByte = bytes - 1;
        if (data.Length < startIndex + bytes)
            throw new ArgumentOutOfRangeException(nameof(startIndex), $"Data array is too small to write a {bytes}-byte value at offset {startIndex}.");
        for (int index = 0; index < bytes; index++)
        {
            int offs = startIndex + (littleEndian ? index : lastByte - index);
            data[offs] = (byte)(value >> (8 * index) & 0xFF);
        }
    }

    public static uint ReadIntFromByteArray(byte[] data, int startIndex, int bytes, bool littleEndian)
    {
        int lastByte = bytes - 1;
        if (data.Length < startIndex + bytes)
            throw new ArgumentOutOfRangeException(nameof(startIndex), $"Data array is too small to read a {bytes}-byte value at offset {startIndex}.");
        uint value = 0;
        for (int index = 0; index < bytes; index++)
        {
            int offs = startIndex + (littleEndian ? index : lastByte - index);
            value += (uint)(data[offs] << (8 * index));
        }

        return value;
    }

    /// <summary>
    /// Gets the raw bytes from an image.
    /// </summary>
    /// <param name="sourceImage">The image to get the bytes from.</param>
    /// <param name="stride">Stride of the retrieved image data.</param>
    /// <returns>The raw bytes of the image</returns>
    public static byte[] GetImageData(Bitmap sourceImage, out int stride)
    {
        BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
        stride = sourceData.Stride;
        byte[] data = new byte[stride * sourceImage.Height];
        Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
        sourceImage.UnlockBits(sourceData);
        return data;
    }
}
