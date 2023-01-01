using GT7.ScreenParser.Constantes;
using GT7.ScreenParser.Extensions;
using SkiaSharp;
using System.Diagnostics;
using Tesseract;

namespace GT7.ScreenParser
{
    internal class Program
    {
        private static SKColor RedColor = new SKColor(255, 0, 0);
        private static SKColor BlueColor = new SKColor(0, 0, 255);

        static void Main(string[] args)
        {
            SKBitmap? originalImage = null;
            SKBitmap? dr = null;
            SKBitmap? sr = null;
            SKBitmap? drProgressBar = null;

            try
            {
                var parsedArgs = args.GetConfigurationFromArgs();
                parsedArgs.ValidateConfiguration();
                CheckTesseractAssets();

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                originalImage = DecodeImageFromFile(parsedArgs[ConfigurationKeys.ImagePath]);
                if (originalImage.CalculateAspectRatio() != 1.77D)
                    throw new Exception("Incorret image aspect ratio, It needs to be 16:9");

                var convertionFactor = originalImage.CalculateConvertionFactor();

                dr = originalImage.CropScreenshot(Gt7ImageValues.DrInitialXDistancePosition, Gt7ImageValues.DrInitialYDistancePosition,
                    Gt7ImageValues.CharacterBoxWidth, Gt7ImageValues.CharacterBoxHeight);

                sr = originalImage.CropScreenshot(Gt7ImageValues.SrInitialXDistancePosition, Gt7ImageValues.SrInitialYDistancePosition,
                    Gt7ImageValues.CharacterBoxWidth, Gt7ImageValues.CharacterBoxHeight);

                drProgressBar = originalImage.CropScreenshot(Gt7ImageValues.ProgressBarInitialXDistancePosition, Gt7ImageValues.ProgressBarInitialYDistancePosition,
                    (int)(Gt7ImageValues.ProgressBarBoxWidth / convertionFactor), (int)(Gt7ImageValues.ProgressBarBoxHeight / convertionFactor));

                ChangePixelColor(dr, RedColor, Gt7ImageValues.CharacterRedByteThreshold);
                ChangePixelColor(sr, RedColor, Gt7ImageValues.CharacterRedByteThreshold);
                ChangePixelColor(drProgressBar, RedColor, Gt7ImageValues.ProgressBarRedByteThreshold);
                ChangePixelColor(drProgressBar, BlueColor, 30);

                var extractDr = ExtractImageCharacters(dr.GetEncodedBitmap());
                var extractSr = ExtractImageCharacters(sr.GetEncodedBitmap());

                double completed = drProgressBar.ExtractProgress();
                int drPoints = CalculateDriverRatingPoints(completed, extractDr);

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine($"[{extractDr}/{extractSr}] Progress: {completed}% {drPoints}pts| Total time to finish: {ts.ToString(@"m\:ss\.fff")}");

                SaveExtractImages(dr, sr, drProgressBar, parsedArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                dr?.Dispose();
                sr?.Dispose();
                drProgressBar?.Dispose();
                originalImage?.Dispose();
            }
        }

        public static int CalculateDriverRatingPoints(double currentProgress, string drCharacter)
        {
            int maxRating = 0;
            switch (drCharacter)
            {
                case "A+":
                    maxRating = 75000;
                    break;                    
                case "A":
                    maxRating = 49999;
                    break;
                case "B":
                    maxRating = 29999;
                    break;
                case "C":
                    maxRating = 9999;
                    break;
                case "D":
                    maxRating = 3999;
                    break;
                case "E":
                    maxRating = 1999;
                    break;      
            }

            return (int)(currentProgress * maxRating) / 100;
        }

        /// <summary>
        /// Save current cropped image to files
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="sr"></param>
        /// <param name="parsedArgs"></param>
        public static void SaveExtractImages(SKBitmap? dr, SKBitmap? sr, SKBitmap? drProgressBar, Dictionary<string, string> parsedArgs)
        {
            if (dr == null || sr == null || drProgressBar == null) return;
            if (parsedArgs[ConfigurationKeys.SaveResult].Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                dr.SaveBitmap("dr.png");
                sr.SaveBitmap("sr.png");
                drProgressBar.SaveBitmap("drProgressBar.png");
            }
        }

        /// <summary>
        /// Check if Tesseract assets exists
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void CheckTesseractAssets()
        {
            if (!Directory.Exists(FilePaths.TesseractFiles))
                throw new Exception($"Please, install Tesseract files in this folder: {FilePaths.TesseractFiles}");

            if (!Directory.EnumerateFiles(FilePaths.TesseractFiles).Any())
                throw new Exception($"Please, install Tesseract files in this folder: {FilePaths.TesseractFiles}");
        }

        /// <summary>
        /// Decode an image to SKBitmap
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SKBitmap DecodeImageFromFile(string filePath)
        {
            using (var fStream = new FileStream(filePath, FileMode.Open))
            {
                using (var inputStream = new SKManagedStream(fStream))
                {
                    return SKBitmap.Decode(inputStream);
                }
            }
        }

        /// <summary>
        /// Change image pixel colors
        /// Checks for a threshold in red byte in a pixel and set it to full RED if matches the threshold >=
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        public static void ChangePixelColor(SKBitmap bitmap, SKColor newColor, int threshold)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var currentPixel = bitmap.GetPixel(x, y);
                    if (currentPixel == RedColor) continue;
                    if (currentPixel.Red >= threshold)
                    {
                        bitmap.SetPixel(x, y, newColor);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve characters from an byte array (image in bytes)
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Character or empty</returns>
        public static string ExtractImageCharacters(byte[] data)
        {
            var result = string.Empty;
            try
            {
                using (var engine = new TesseractEngine(FilePaths.TesseractFiles, "eng", EngineMode.Default))
                {
                    engine.DefaultPageSegMode = PageSegMode.SingleLine;

                    using (var img = Pix.LoadFromMemory(data))
                    {
                        using (var page = engine.Process(img))
                        {
                            result = page.GetText();
                            result = result.Replace("\n", string.Empty);
                        }
                    }
                }
            }
            catch { result = string.Empty; }

            return result;
        }
    }
}