using ChanisterWpf.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ChanisterWpf.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    public partial class CreatePostDialog : Window
    {
        static readonly HttpClient client = new HttpClient();
        private const String CAPTCHAURL = "https://sys.4channel.org/captcha";
        private Captcha captcha = new Captcha();

        public CreatePostDialog()
        {
            InitializeComponent();
        }
        private void okButton_Click(object sender, RoutedEventArgs e) =>
        DialogResult = true;

        public string FixBase64ForImage(string Image)
        {
            StringBuilder sbText = new StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }

        public async Task<string> GetUriAsync(string uri)
        {
            String response = null;

            try
            {
                response = await client.GetStringAsync(uri);

                Debug.WriteLine(response);
                return response;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return response;
            }
        }

        private BitmapImage GetBitmapFromBase64(string ImageText, int width, int height)
        {

            Byte[] binaryData = Convert.FromBase64String(FixBase64ForImage(ImageText));
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelWidth = width;
            bi.DecodePixelHeight = height;
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();

            return bi;
        }

        private void Captcha_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.WriteLine(e.NewValue.ToString());
            Canvas.SetLeft(Captcha_Image_Bg, -e.NewValue * 10);
        }

        private async void GetCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            if (captcha.bg != null) { return; }
            var content = await GetUriAsync(CAPTCHAURL);

            captcha = JsonSerializer.Deserialize<Captcha>(content);
            BitmapImage bitmapImg = GetBitmapFromBase64(captcha.img, captcha.img_width, captcha.img_height);
            BitmapImage bitmapBg = GetBitmapFromBase64(captcha.bg, captcha.bg_width, captcha.img_height);

            Dispatcher.Invoke(() =>
            {
                Captcha_Image_Img.Source = bitmapImg;
                Captcha_Image_Bg.Source = bitmapBg;
            });

        }
    }
}