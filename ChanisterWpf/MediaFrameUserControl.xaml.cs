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

        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public Uri Uri { get; set; }
        public Grid MediaFrameGrid { get { return mediaFrameGrid; } set { mediaFrameGrid = value; } }
        public Image Image { get { return image; } set { image = value; } }
        public TextBlock FileInfo { get { return fileInfo; } set { fileInfo = value; } }
        public ShadowedExtendedPopup Popup { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }

        private void Popout(object sender, System.Windows.Input.MouseEventArgs e) { ((MediaFrame)Parent).PopOut(sender, e); }
        private void Move(object sender, System.Windows.Input.MouseEventArgs e) { ((MediaFrame)Parent).MouseMove(sender, e); }
        private void Save(object sender, System.Windows.RoutedEventArgs e) { ((MediaFrame)Parent).Save(sender, e); }
        private void SaveAs(object sender, System.Windows.RoutedEventArgs e) { ((MediaFrame)Parent).SaveAs(sender, e); }
        private void CopyUrl(object sender, System.Windows.RoutedEventArgs e) { ((MediaFrame)Parent).CopyUrl(sender, e); }
        private void CopyFileName(object sender, System.Windows.RoutedEventArgs e) { ((MediaFrame)Parent).CopyFileName(sender, e); }
        private void Copy(object sender, System.Windows.RoutedEventArgs e) { ((MediaFrame)Parent).Copy(sender, e); }
        private void Open(object sender, System.Windows.RoutedEventArgs e) { ((MediaFrame)Parent).Open(sender, e); }
    }
}
