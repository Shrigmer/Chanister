namespace ChanisterWpf.Model
{
    internal class Captcha
    {
        public string challenge { get; set; }
        public int ttl { get; set; }
        public int cd { get; set; }
        public string img { get; set; }
        public int img_width { get; set; }
        public int img_height { get; set; }
        public string bg { get; set; }
        public int bg_width { get; set; }
    }
}