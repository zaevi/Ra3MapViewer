using Pfim;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ra3MapViewer.Utils
{
    public static class TgaBitmapSource
    {
        public static BitmapSource LoadFrom(string path)
        {
            var image = Pfim.Pfim.FromFile(path);
            
            return BitmapSource.Create(image.Width, image.Height, 96.0, 96.0, PixelFormat(image), null, image.Data, image.Stride);
        }

        private static PixelFormat PixelFormat(IImage image)
        {
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    return PixelFormats.Bgr24;
                case ImageFormat.Rgba32:
                    return PixelFormats.Bgra32;
                case ImageFormat.Rgb8:
                    return PixelFormats.Gray8;
                case ImageFormat.R5g5b5a1:
                case ImageFormat.R5g5b5:
                    return PixelFormats.Bgr555;
                case ImageFormat.R5g6b5:
                    return PixelFormats.Bgr565;
                default:
                    throw new Exception($"Unable to convert {image.Format} to WPF PixelFormat");
            }
        }
    }
}
