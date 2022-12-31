using SkiaSharp;

namespace GT7.ScreenParser.Extensions
{
    public static class SKBitmapExtensions
    {
        /// <summary>
        /// Given a SKBitmap calculate the aspect ratio
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static double CalculateAspectRatio(this SKBitmap bitmap)
        {
            if (bitmap == null) return 0.0f;
            return Math.Round((double)bitmap.Width / (double)bitmap.Height, 2, MidpointRounding.ToZero);
        }

        public static double CalculateConvertionFactor(this SKBitmap bitmap)
        {
            if (bitmap.Width == 3840) return 1.0D;
            return Math.Round((double)3840 / (double)bitmap.Width, 2, MidpointRounding.ToZero);
        }

        /// <summary>
        /// Encode a given SKBitmap to png with 100% quality and returns an array of bytes
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static byte[] GetEncodedBitmap(this SKBitmap bitmap)
        {
            using (var data = bitmap.Encode(SKEncodedImageFormat.Png, 100))
            {
                return data.ToArray();
            }
        }

        /// <summary>
        /// Save a SKBitmap to a given file name as png
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="fileName"></param>
        public static void SaveBitmap(this SKBitmap bitmap, string fileName)
        {
            using (var fStream = new FileStream(fileName, FileMode.Create))
            {
                using (var data = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                {
                    data.SaveTo(fStream);
                }
            }
        }
    }
}
