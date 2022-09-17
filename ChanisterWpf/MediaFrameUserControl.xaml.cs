using System;
using System.Windows.Controls;

namespace ChanisterWpf
{
    public partial class MediaFrameUserControl : UserControl, MediaFrame
    {
        public MediaFrameUserControl()
        {
            InitializeComponent();
        }
        public MediaFrame Implementor { get => (MediaFrame)Parent; }
        public int PixelWidth { get => Implementor.PixelWidth; set => Implementor.PixelWidth = value; }
        public int PixelHeight { get => Implementor.PixelHeight; set => Implementor.PixelHeight = value; }
        public int Size { get => Implementor.Size; set => Implementor.Size = value; }
        public Uri Uri { get => Implementor.Uri; set => Implementor.Uri = value; }
        public Grid MediaFrameGrid { get { return mediaFrameGrid; } set { mediaFrameGrid = value; } }
        public Image Image { get { return image; } set { image = value; } }
        public TextBlock FileInfo { get { return fileInfo; } set { fileInfo = value; } }
        public ShadowedExtendedPopup Popup { get => Implementor.Popup; set => Implementor.Popup = value; }
        public string ImageName { get => Implementor.ImageName; set => Implementor.ImageName = value; }
        public string Extension { get => Implementor.Extension; set => Implementor.Extension = value; }

        private void Popout(object sender, System.Windows.Input.MouseEventArgs e) { Implementor.PopOut(sender, e); }
        private void Move(object sender, System.Windows.Input.MouseEventArgs e) { Implementor.MouseMove(sender, e); }
        private void Save(object sender, System.Windows.RoutedEventArgs e) { Implementor.Save(sender, e); }
        private void SaveAs(object sender, System.Windows.RoutedEventArgs e) { Implementor.SaveAs(sender, e); }
        private void CopyUrl(object sender, System.Windows.RoutedEventArgs e) { Implementor.CopyUrl(sender, e); }
        private void CopyFileName(object sender, System.Windows.RoutedEventArgs e) { Implementor.CopyFileName(sender, e); }
        private void Copy(object sender, System.Windows.RoutedEventArgs e) { Implementor.Copy(sender, e); }
        private void Open(object sender, System.Windows.RoutedEventArgs e) { Implementor.Open(sender, e); }
    }
}
