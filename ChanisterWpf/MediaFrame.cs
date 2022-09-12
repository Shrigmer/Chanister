using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChanisterWpf
{
    public interface MediaFrame
    {
        public static MainWindow MainWindow => (MainWindow)Application.Current.MainWindow;

        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public Uri Uri { get; set; }
        public Grid MediaFrameGrid { get; set; }
        public Image Image { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }
        public TextBlock FileInfo { get; set; }
        public ShadowedExtendedPopup Popup { get; set; }

        public void InitializeComponent();
        public void SetBaseImage(Uri imageUri, string imagename, string extension, int pixelWidth, int pixelHeight, int size)
        {
            Uri = imageUri;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            ImageName = imagename;
            Extension = extension;
            if (pixelWidth < (int)Scale(Image.MaxWidth))
            {
                Image.Width = pixelWidth;
                MediaFrameGrid.Width = pixelWidth;
                Image.Source = InitImage(imageUri, pixelWidth);
            }
            else
            {
                Image.Source = InitImage(imageUri, (int)Scale(Image.MaxWidth));
            }
            string unit = "B";
            switch (size)
            {
                case >= 1048576:
                    size /= 1048576;
                    unit = "MB";
                    break;
                case >= 1024:
                    size /= 1024;
                    unit = "kB";
                    break;
            }
            FileInfo.Text = Task.Run(() => { return $"{TagParser.DecodeText(imagename)}{extension}  {pixelWidth}x{pixelHeight} {size}{unit}"; }).Result;
        }
        public static BitmapImage InitImage(Uri uri, int width = 0, int height = 0)
        {
            try
            {
                BitmapImage image = new();
                image.BeginInit();
                image.UriSource = uri;
                image.DecodePixelWidth = width;
                image.DecodePixelHeight = height;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.DownloadCompleted += (s, e) => { image.Freeze(); };
                return image;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<Stream> DownloadToStreamAsync(string url)
        {
            try
            {
                HttpClient HC = MainWindow.HttpClient;
                byte[] data = await HC.GetByteArrayAsync(url);
                return Task.Run(() => { return new MemoryStream(data); }).Result;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public void PopOut(object sender, MouseEventArgs e) { }
        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (Popup is not null) Popup.MoveToCursor(20, -20);
        }
        public void Save(object sender, RoutedEventArgs e) { }
        public void SaveAs(object sender, RoutedEventArgs e) { }
        public void CopyUrl(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, Uri.OriginalString);
        }
        public void CopyFileName(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, ImageName + Extension);
        }
        public void Copy(object sender, RoutedEventArgs e) { }
        public void Open(object sender, RoutedEventArgs e) { }
        public static double Scale(double numberToScale)
        {
            return numberToScale * ((MainWindow)Application.Current.MainWindow).scale;
        }
    }
}
