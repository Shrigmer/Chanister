using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChanisterWpf
{
    public partial class ImageFrame : UserControl, MediaFrame
    {
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int Size { get; set; }
        public ShadowedExtendedPopup Popup { get; set; } = null;
        public Uri Uri { get; set; }
        public Grid MediaFrameGrid { get; set; }
        public Image Image { get; set; }
        public Image PopUpImage { get; set; }
        public TextBlock FileInfo { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }

        public ImageFrame(Uri imageUri, string imagename, string extension, int pixelWidth, int pixelHeight, int size)
        {
            InitializeComponent();
            Uri = imageUri;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            ImageName = imagename;
            Extension = extension;
            Image = mediaFrame.Image;
            FileInfo = mediaFrame.FileInfo;
            ((MediaFrame)mediaFrame).SetBaseImage(imageUri, imagename, extension, pixelWidth, pixelHeight, size);
        }
        public new void MouseMove(object sender, MouseEventArgs e)
        {
            if (Popup is not null) Popup.MoveToCursor(20, -20);
        }
        public void PopOut(object sender, MouseEventArgs e)
        {
            if (Popup is null)
            {
                Image copy = new()
                {
                    Source = MediaFrame.InitImage(Uri, PixelWidth, PixelHeight)
                };
                Popup = new() { AllowsTransparency = true, IsOpen = true };
                Popup.MouseEnter += PopOut;
                Popup.MouseLeave += PopOut;
                Popup.MouseMove += MouseMove;
                Popup.Grid.Children.Add(copy);
                PopUpImage = copy;
                Timer time = new()
                {
                    Interval = 60000
                };
                time.Elapsed += KillPopup;
                time.Start();
            }
            else
            {
                Popup.IsOpen = !Popup.IsOpen;
            }
        }

        private void KillPopup(object sender, EventArgs e)
        {
            Timer time = (Timer)sender;
            time.Stop();
            Dispatcher.Invoke(() =>
            {
                Popup.Grid.Children.Clear();
                Popup = null;
                PopUpImage = null;
            });
        }
        public void Copy(object sender, RoutedEventArgs e)
        {
            if (PopUpImage is not null) Clipboard.SetData(DataFormats.Bitmap, PopUpImage.Source);
        }
        public void Save(object sender, RoutedEventArgs e)
        {
            string savePath = Properties.Settings.Default.ImageFilepath;
            if (savePath != "") SaveJPEG(savePath + ImageName + Extension);
        }
        public void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new()
            {
                Title = "Save picture as ",
                FileName = ImageName + Extension,
                Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp"
            };
            if ((bool)saveDialog.ShowDialog())
            {
                SaveJPEG(saveDialog.FileName);
            }
        }
        public void SaveJPEG(string filePath)
        {
            JpegBitmapEncoder jpg = new();
            if (PopUpImage is not null) jpg.Frames.Add(BitmapFrame.Create((BitmapSource)PopUpImage.Source));
            using Stream stm = File.Create(filePath);
            jpg.Save(stm);
        }
        public void Open(object sender, RoutedEventArgs e)
        {
            string path = Path.GetTempPath() + ImageName + Extension;
            SaveJPEG(path);
            Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
    }
}
