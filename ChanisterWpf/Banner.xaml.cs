using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ChanisterWpf
{
    /// <summary>
    /// Interaction logic for Banner.xaml
    /// </summary>
    public partial class Banner : UserControl
    {
        private int Retries { get; set; } = 3;
        public Banner()
        {
            InitializeComponent();
            SetNew();
        }
        async void SetNew()
        {
            try
            {
                string[] knownBanners = File.ReadAllText("Resources/KnownBanners.txt").Split(',');
                Random random = new Random();
                int imageIndex = random.Next(0, knownBanners.Length - 1);
                Uri imageUri = new Uri("https://s.4cdn.org/image/title/" + knownBanners[imageIndex]);
                Dispatcher.Invoke(() =>
                {
                    image.Source = MediaFrame.InitImage(imageUri, (int)BannerButton.MaxWidth);
                });

            }
            catch (Exception)
            {
                if (true)
                {
                    Retries--;
                    SetNew();
                }
            }
        }

        private void BannerButton_Click(object sender, RoutedEventArgs e)
        {
            SetNew();
        }
    }
}
