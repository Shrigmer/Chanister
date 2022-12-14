using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ChanisterWpf
{
    internal class ThreadLink : Run
    {
        private string Board;
        private int Thread;
        public int PostQuoted;
        public static readonly RoutedEvent ScrollToPost = EventManager.RegisterRoutedEvent("ScrollToPost", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ThreadLink));

        public ThreadLink(string board, int thread, int post)
        {
            Board = board;
            Thread = thread;
            PostQuoted = post;
            Text += $">>>/{board}/{thread}/{post}";
            Foreground = MainWindow.solidRed;
            Tag = "threadlink";
            MouseMove += new MouseEventHandler(OnHover);
            MouseLeave += new MouseEventHandler(OnHoverEnd);
            MouseLeftButtonDown += new MouseButtonEventHandler(OpenThread);
        }

        private void OnHoverEnd(object sender, MouseEventArgs e)
        {
            TextDecorations = null;
        }

        private void OnHover(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            TextDecorations = System.Windows.TextDecorations.Underline;
        }

        private void OpenThread(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).DisplayThreadInPage(new() { no = Thread });
            RaiseEvent(new(ScrollToPost, this));
        }
    }
}
