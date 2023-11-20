using System.Drawing;

namespace WebApplication1.Controllers;

public class Images
{
    public static void CreateMatrix(int xj, string folderPath, string build)
    {
        for (int i = 1; i <= xj; i++)
        {
            string imagePath = $"{folderPath}/{i}-{build}.png";
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Image {imagePath} does not exist.");
                continue;
            }

            using (var file = new StreamWriter($"{folderPath}/{i}-{build}-matrix.txt"))
            {
                using (var img = new Bitmap(imagePath))
                {
                    if (img.Width != 256 || img.Height != 256)
                    {
                        Console.WriteLine($"Image {imagePath} is not 256x256 pixels.");
                        continue;
                    }

                    for (int y = 0; y < img.Height; y++)
                    {
                        for (int x = 0; x < img.Width; x++)
                        {
                            Color pixel = img.GetPixel(x, y);
                            if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                            {
                                file.Write("0 ");
                            }
                            else if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0)
                            {
                                file.Write("1 ");
                            }
                            else if ((pixel.R == 63 && pixel.G == 72 && pixel.B == 204))
                            {
                                file.Write("2 ");
                            }
                            else if (pixel.R == 14 && pixel.G == 209 && pixel.B == 69)
                            {
                                file.Write("3 ");
                            }
                            else
                            {
                                file.Write("1 ");
                            }
                        }
                        file.WriteLine();
                    }
                }
            }
        }
    }

    public static Image LoadImage(string path)
    {
        try
        {
            return Image.FromFile(path);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("The file was not found.");
            return null;
        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine("The file is not a valid image file.");
            return null;
        }
    }
    
    public static string ConvertPng2Svg(string fileName, string folderPath)
    {
        using (var file = File.OpenRead($"{fileName}"))
        using (var img = new Bitmap(file))
        {
            var res = string.Empty;

            var width = img.Width;
            var height = img.Height;

            res += "<?xml version = \"1.0\"?>";
            res += $"<svg width=\"{width}\" height=\"{height}\" ";
            res += "xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">";

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = img.GetPixel(x, y);
                    if (color.R != 255 || color.G != 255 || color.B != 255)
                    {
                        var hex = ColorTranslator.ToHtml(color);
                        res += $"<rect x=\"{x}\" y=\"{y}\" width=\"1\" height=\"1\" fill=\"{hex}\" />";
                    }
                }
            }

            res += "</svg>";
            return res;
        }
    }
}