using System;
using System.Drawing;
using ImageProcessing.Util;

namespace ImageProcessing
{
    public class CsApplier : IFilterApplier
    {
        public void Apply(Filter filter, Bitmap target)
        {
            Action<byte[], byte[]> action = (bytesSource, bytesDest) =>
            {
                var stride = target.GetStride();
                Run(bytesSource,
                    bytesDest, stride,
                    stride / target.Width,
                    filter.Matrix, filter.Size, filter.Bias);
            };

            target.ExecuteTransformAction(action);
        }

        static void Run(byte[] src, byte[] dst,
            int stride, int bytesPerPixel, double[] filter, 
                int filterWidth, int bias)
        {
            int srcBytesCount = src.Length;
            int filterCount = filter.Length;
            int filterHeight = filter.Length / filterWidth;

            for (int iDst = 0; iDst < srcBytesCount; iDst++)
            {
                double pixelsAccum = 0;
                double filterAccum = 0;

                for (int i = 0; i < filterCount; i++)
                {
                    int yFilter = i / filterHeight;
                    int xFilter = i % filterWidth;

                    int iSrc = iDst + stride * (yFilter - filterHeight / 2) +
                                        bytesPerPixel * (xFilter - filterWidth / 2);

                    if (iSrc >= 0 && iSrc < srcBytesCount)
                    {
                        pixelsAccum += filter[i] * src[iSrc];
                        filterAccum += filter[i];
                    }
                }

                if (filterAccum != 0)
                    pixelsAccum /= Math.Abs(filterAccum);

                pixelsAccum += bias;
                dst[iDst] = pixelsAccum < 0 ? (byte)0 : (pixelsAccum > 255 ?
                                                (byte)255 : (byte)pixelsAccum);
            }
        }

    }
}
