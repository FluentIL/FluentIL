using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageProcessing.Util
{
    public static class BitmapExtensions
    {
        public static bool IsCompatible(this Bitmap bmp)
        {
            if (bmp.PixelFormat == PixelFormat.Format24bppRgb) return true;
            if (bmp.PixelFormat == PixelFormat.Format32bppArgb) return true;
            if (bmp.PixelFormat == PixelFormat.Format32bppPArgb) return true;
            if (bmp.PixelFormat == PixelFormat.Format32bppRgb) return true;
            return false;
        }

        public static byte[] GetBytes(this Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect,
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            // coloca os bytes da imagem em uma matriz
            int bytesCount = data.Stride * data.Height;
            byte[] result = new byte[bytesCount];
            Marshal.Copy(data.Scan0, result, 0, bytesCount);
            bmp.UnlockBits(data);

            return result;
        }

        public static int GetStride(this Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect,
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            // coloca os bytes da imagem em uma matriz
            var result = Math.Abs(data.Stride);
            bmp.UnlockBits(data);

            return result;
        }

        public static void SetBytes(this Bitmap bmp, byte[] bytes)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            int bytesCount = data.Stride * data.Height;
            Marshal.Copy(bytes, 0, data.Scan0, bytesCount);
            bmp.UnlockBits(data);
        }

        public static void ExecuteTransformAction(this Bitmap bmp, Action<byte[], byte[]> action)
        {
            var bytesSource = bmp.GetBytes();
            byte[] bytesDest = new byte[bytesSource.Length];

            action(bytesSource, bytesDest);

            bmp.SetBytes(bytesDest);
        }
    }
}
