using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChanisterWpf
{
    public partial class OP : UserControl
    {
        public static readonly RoutedEvent MouseLeftUpOPEvent = EventManager.RegisterRoutedEvent("MouseLeftUpOPEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OP));
        private readonly Board Board;

        public PostData PostData { get; set; }

        public OP(PostData data, Board board)
        {
            InitializeComponent();
            PostData = data;
            Board = board;
            ThreadImage.Source = MediaFrame.InitImage(new Uri($"https://i.4cdn.org/{Board.Data.board}/{PostData.tim}s.jpg"), (int)MediaFrame.Scale(ThreadImage.MaxWidth));
            ThreadNumber.Text = $"Post {PostData.no}";
            ThreadReplies.Text = $"{PostData.replies} Replies";
            ThreadImages.Text = $"{PostData.images} Images";
            ThreadSubject.Text = "";
            ThreadComment.Text = "";
            if (PostData.sub is null)
            {
                SubjectRow.Height = new GridLength(0);
            }
            else
            {
                ThreadSubject.Inlines.Add(Task.Run(() => { return TagParser.DecodeText(PostData.sub); }).Result);
            }
            if (PostData.com is null)
            {
                CommentRow.Height = new GridLength(10);
            }
            else
            {
                PostData.com = Task.Run(() => TagParser.DecodeText(PostData.com)).Result;
                TagParser.AddInlines(Task.Run(() => { return TagParser.FindFormattedText(PostData.com); }).Result, ThreadComment.Inlines, PostData.no, PostData.no);
            }
        }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e) => HighLightBorder.Opacity = 100;
        private void UserControl_MouseLeave(object sender, MouseEventArgs e) => HighLightBorder.Opacity = 0;
        public void UserControl_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(MouseLeftUpOPEvent, sender));
        }
    }
}
