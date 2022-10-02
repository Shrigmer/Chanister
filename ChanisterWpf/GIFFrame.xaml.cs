using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XamlAnimatedGif;

namespace ChanisterWpf
{
    public partial class GIFFrame : UserControl, MediaFrame
    {
        public Uri GIFUri { get; set; }
        public bool isAnimating { get; set; } = false;
        public Animator Animator { get; set; } = null;
        public MediaFrame MFBase { get => mediaFrame; set => mediaFrame = (MediaFrameUserControl)value; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int Size { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }
        public ShadowedExtendedPopup Popup { get; set; }
        public Uri Uri { get => MFBase.Uri; set => MFBase.Uri = value; }
        public Grid MediaFrameGrid { get => MFBase.MediaFrameGrid; set => MFBase.MediaFrameGrid = value; }
        public Image Image { get => MFBase.Image; set => MFBase.Image = value; }
        public TextBlock FileInfo { get => MFBase.FileInfo; set => MFBase.FileInfo = value; }

        public GIFFrame(Uri imageUri, string imagename, string extension, int pixelWidth, int pixelHeight, int size)
        {
            InitializeComponent();
            Image = mediaFrame.Image;
            FileInfo = mediaFrame.FileInfo;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            Size = size;
            ImageName = imagename;
            Extension = extension;
            GIFUri = imageUri;
            AnimationBehavior.SetSourceUri(Image, GIFUri);
            AnimationBehavior.SetAutoStart(Image, false);
            AnimationBehavior.AddLoadedHandler(Image, AnimatorLoadedHandler);
            MFBase.SetFileInfo();
            Image.SizeChanged += SizeChangedHandler;
        }
        private void AnimatorLoadedHandler(object sender, RoutedEventArgs e)
        {
            Animator = AnimationBehavior.GetAnimator(Image);
        }
        bool first = true;
        double firstWidth = 0;
        double firstHeight = 0;
        private void SizeChangedHandler(object sender, EventArgs e) //Prevents the page view from going ballistic, changing the size when scrolling
        {
            if (first)
            {
                first = false;
                firstWidth = ActualWidth;
                firstHeight = ActualHeight;
            }
            else
            {
                Width = firstWidth;
                Height = firstHeight;
            }
        }
        public void PopOut(object sender, MouseEventArgs e)
        {
            if (Animator is not null)
            {
                if (Animator.IsPaused)
                {
                    Animator.Play();
                }
                else
                {
                    Animator.Pause();
                }
            }
        }
        public async void Copy(object sender, RoutedEventArgs e)
        {
            Stream stream = await MediaFrame.DownloadToStreamAsync(GIFUri.OriginalString);
            Clipboard.SetData("Gif", stream);
            stream.Dispose();
        }
        public async void Save(object sender, RoutedEventArgs e)
        {
            string savePath = Properties.Settings.Default.ImageFilepath;
            if (savePath != "") await SaveGIF(savePath + ImageName + Extension);
        }
        public async void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new()
            {
                Title = "Save video as ",
                FileName = ImageName + Extension,
                Filter = "Image Files(*gif)|*gif"
            };
            if ((bool)saveDialog.ShowDialog())
            {
                await SaveGIF(saveDialog.FileName);
            }
        }
        public async Task SaveGIF(string filePath)
        {
            if (!filePath.EndsWith(".gif")) filePath += Extension;
            Stream medStr = await MediaFrame.DownloadToStreamAsync(GIFUri.OriginalString);
            FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
            medStr.Position = 0;
            await medStr.CopyToAsync(fileStream);
            fileStream.Dispose();
        }
        public async void Open(object sender, RoutedEventArgs e)
        {
            string path = Path.GetTempPath() + ImageName + Extension;
            await SaveGIF(path);
            Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
    }
}

