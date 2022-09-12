using ChanisterWpf.Singletons;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChanisterWpf
{
    public partial class WEBMFrame : UserControl, MediaFrame
    {
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public Uri Uri { get; set; }
        public string WEBMUriString { get; set; }
        public Grid MediaFrameGrid { get; set; }
        public Image Image { get; set; }
        public TextBlock FileInfo { get; set; }
        public ShadowedExtendedPopup Popup { get; set; } = null;
        Dictionary<long, Stream> mediaDictionary = null;
        MediaSingleton mediaSing = null;
        Int64 imgNameRef { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }

        int MediaSize;


        public WEBMFrame(Uri imageUri, string imagename, string extension, int pixelWidth, int pixelHeight, int size, long imgRef)
        {
            InitializeComponent();
            Image = mediaFrame.Image;
            FileInfo = mediaFrame.FileInfo;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            ImageName = imagename;
            Extension = extension;
            Uri = new(imageUri.OriginalString.Split(".webm")[0] + "s.jpg");
            MediaSize = size;
            imgNameRef = Convert.ToInt64(imgRef);
            WEBMUriString = imageUri.OriginalString;
            mediaSing = MediaSingleton.Instance;
            mediaDictionary = mediaSing.mediaDictionary;
            ((MediaFrame)mediaFrame).SetBaseImage(Uri, imagename, extension, pixelWidth, pixelHeight, size);
        }

        public async void PopOut(object sender, MouseEventArgs e)
        {
            if (Popup is null)
            {
                Popup = new()
                {
                    IsOpen = true,
                    Width = MediaFrame.Scale(PixelWidth),
                    Height = MediaFrame.Scale(PixelHeight)
                };
                Popup.Grid.Children.Clear();
                Popup.Grid.Children.Add(App.Current.VideoView);
                await OpenWEBM();
            }
            else
            {
                Popup.IsOpen = !Popup.IsOpen;
                if (Popup.IsOpen)
                {
                    Popup.Grid.Children.Clear();
                    Popup.Grid.Children.Add(App.Current.VideoView);
                    await OpenWEBM();
                    App.Current.VideoView.Player.Play();
                }
                else
                {
                    App.Current.VideoView.Player.Pause();
                    Popup.Grid.Children.Clear();
                }
            }
        }
        public async void Copy(object sender, RoutedEventArgs e)
        {
            if (!mediaDictionary.ContainsKey(imgNameRef))
            {
                await DownLoadWEBM();
            }
            Clipboard.SetData("Webm", mediaDictionary[imgNameRef]);
        }
        private async Task OpenWEBM()
        {
            if (mediaDictionary.ContainsKey(imgNameRef))
            {
                Stream medStr = mediaDictionary[imgNameRef];
                App.Current.VideoView.Player.Open(medStr);
            }
            else
            {
                Stream medStr = await MediaFrame.DownloadToStreamAsync(WEBMUriString);
                mediaDictionary.TryAdd(imgNameRef, medStr);
                mediaSing.dictSizeBytes += MediaSize;
                App.Current.VideoView.Player.Open(medStr);
            }
        }
        public async void Save(object sender, RoutedEventArgs e)
        {
            string savePath = Properties.Settings.Default.ImageFilepath;
            if (savePath != "") await SaveWEBM(savePath + ImageName + Extension);
        }
        public async void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new()
            {
                Title = "Save video as ",
                Filter = "Video Files(*webm)|*webm"
            };
            if ((bool)saveDialog.ShowDialog())
            {
                await SaveWEBM(saveDialog.FileName);
            }
        }
        public async Task SaveWEBM(string filePath)
        {
            if (!filePath.EndsWith(".webm")) filePath += Extension;
            if (!mediaDictionary.ContainsKey(imgNameRef))
            {
                await DownLoadWEBM();
            }
            mediaDictionary[imgNameRef].Position = 0;
            FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
            await mediaDictionary[imgNameRef].CopyToAsync(fileStream);
            fileStream.Dispose();
        }

        private async Task DownLoadWEBM()
        {
            Stream medStr = await MediaFrame.DownloadToStreamAsync(WEBMUriString);
            mediaDictionary.TryAdd(imgNameRef, medStr);
            mediaSing.dictSizeBytes += MediaSize;
        }

        public async void Open(object sender, RoutedEventArgs e)
        {
            string path = Path.GetTempPath() + ImageName + Extension;
            await SaveWEBM(path);
            Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
    }
}

