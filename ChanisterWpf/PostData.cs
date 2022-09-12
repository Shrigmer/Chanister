namespace ChanisterWpf
{
    public class PostData
    {
        public int no { get; set; }
        public string now { get; set; }
        public string name { get; set; }
        public string com { get; set; }
        public string filename { get; set; }
        public string ext { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int tn_w { get; set; }
        public int tn_h { get; set; }
        public long tim { get; set; }
        public int time { get; set; }
        public string md5 { get; set; }
        public int fsize { get; set; }
        public int resto { get; set; }
        public int bumplimit { get; set; }
        public int imagelimit { get; set; }
        public string semantic_url { get; set; }
        public int custom_spoiler { get; set; }
        public int replies { get; set; }
        public int images { get; set; }
        public PostData[] last_replies { get; set; }
        public long last_modified { get; set; }
        public string sub { get; set; }
        public int omitted_posts { get; set; }
        public int omitted_images { get; set; }
    }
}
