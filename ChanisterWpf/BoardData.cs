namespace ChanisterWpf
{
    public class BoardData
    {
        public string board { get; set; }
        public string title { get; set; }
        public int ws_board { get; set; }
        public int per_page { get; set; }
        public int pages { get; set; }
        public int max_filesize { get; set; }
        public int max_webm_filesize { get; set; }
        public int max_comment_chars { get; set; }
        public int max_webm_duration { get; set; }
        public int bump_limit { get; set; }
        public int image_limit { get; set; }
        //public Cooldowns cooldowns { get; set; }
        public string meta_description { get; set; }
        public int is_archived { get; set; }
        public int spoilers { get; set; }
        public int custom_spoilers { get; set; }
        public int forced_anon { get; set; }
        public int user_ids { get; set; }
        public int country_flags { get; set; }
        public int code_tags { get; set; }
        public int webm_audio { get; set; }
        public int min_image_width { get; set; }
        public int min_image_height { get; set; }
        public int oekaki { get; set; }
        public int sjis_tags { get; set; }
        public int text_only { get; set; }
        public int require_subject { get; set; }
        public int troll_flags { get; set; }
        public int math_tags { get; set; }
    }
}
