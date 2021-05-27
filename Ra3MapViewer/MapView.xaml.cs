using Ra3MapViewer.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ra3MapViewer
{
    /// <summary>
    /// MapView.xaml 的交互逻辑
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView() => InitializeComponent();

        private void On_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(DataContext is Map map))
                return;

            image.Source = null;
            canvas.Children.Clear();

            var tga = System.IO.Path.Combine(map.FolderPath, map.FileId + "_art.tga");

            if (!TryLoadFrom(tga, out var imageSource))
                return;

            image.Source = imageSource;

            var ratio = Math.Min(Height / imageSource.Height, Width / imageSource.Width);
            canvas.Width = ratio * imageSource.Width;
            canvas.Height = ratio * imageSource.Height;
            var rw = 0.1 / (map.Size.Width - (map.BorderSize * 2)) * canvas.Width;
            var rh = 0.1 / (map.Size.Height - (map.BorderSize * 2)) * canvas.Height;

            foreach(var point in map.PlayerPoints)
            {
                var cir = new Ellipse() { Width = 44, Height = 44, Stroke = Brushes.Blue, Margin = new Thickness(-22), StrokeThickness = 5 };
                Canvas.SetLeft(cir, point.X * rw);
                Canvas.SetBottom(cir, point.Y * rh);
                canvas.Children.Add(cir);
            }
        }

        bool TryLoadFrom(string path, out BitmapSource imageSource)
        {
            imageSource = null;
            try
            {
                imageSource = TgaBitmapSource.LoadFrom(path);
                return true;
            }
            catch (FileNotFoundException) { } 
            catch (ArgumentOutOfRangeException) { } // 不支持的tga
            catch (ArgumentException) // 并不是tga
            {
                try
                {
                    var stream = new MemoryStream(File.ReadAllBytes(path));
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = stream;
                    img.EndInit();
                    imageSource = img;

                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
