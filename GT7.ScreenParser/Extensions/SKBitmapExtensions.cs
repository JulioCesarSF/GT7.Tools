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

        /// <summary>
        /// Crop a given bitmap 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="xDistancePercent"></param>
        /// <param name="yDistancePercent"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static SKBitmap CropScreenshot(this SKBitmap bitmap, float xDistancePercent, float yDistancePercent, int w, int h)
        {
            int xSize = (int)(bitmap.Width * xDistancePercent);
            int ySize = (int)(bitmap.Height * yDistancePercent);

            var croppedImage = new SKBitmap(w, h);
            var rect = new SKRectI(xSize, ySize, xSize + w, ySize + h);
            if (bitmap.ExtractSubset(croppedImage, rect))
                return croppedImage;

            throw new Exception("It was not possible to crop the screenshot");
        }

        /// <summary>
        /// Extract percentage progress from DR bar
        /// </summary>
        /// <param name="drProgressBar"></param>
        /// <returns></returns>
        public static double ExtractProgress(this SKBitmap drProgressBar)
        {
            if (drProgressBar == null) return 0.0f;
            var redColor = new SKColor(255, 0, 0);

            var countRed = 0;
            var countBlue = 0;
            for (var x = 1/* ignore first column*/; x < drProgressBar.Width; x++)
            {
                var currentPixel = drProgressBar.GetPixel(x, 1); //ignore first line
                if (currentPixel.Equals(redColor))
                {
                    countRed++;
                }
                if (currentPixel.Blue == 255)
                    countBlue++;
            }

            double completed = (countRed * 100) / (countBlue + countRed);
            return completed;
        }
    }
}
