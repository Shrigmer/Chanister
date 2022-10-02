using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ChanisterWpf
{
    internal class QuoteLink : Run
    {
        public static readonly RoutedEvent ClosePopout = EventManager.RegisterRoutedEvent("ClosePopout", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(QuoteLink));
        public static readonly RoutedEvent MovePopup = EventManager.RegisterRoutedEvent("MovePopup", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(QuoteLink));
        public static readonly RoutedEvent Quoted = EventManager.RegisterRoutedEvent("Quoted", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(QuoteLink));
        public static readonly RoutedEvent ScrollToPost = EventManager.RegisterRoutedEvent("ScrollToPost", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(QuoteLink));
        public int QuotedByPost { get; set; }
        public int PostQuoted { get; set; }
        public bool EnableEvents { get; set; }
        public QuoteLink(int postQuoted, int quotedByPost, string qoutingTypeDescription, bool enableEvents)
        {
            PostQuoted = postQuoted;
            QuotedByPost = quotedByPost;
            Text += $">>{PostQuoted} {qoutingTypeDescription}";
            Foreground = MainWindow.solidRed;
            Tag = "quotelink";
            Background = new SolidColorBrush(Colors.White);
            EnableEvents = enableEvents;
            if (EnableEvents) Loaded += new(RaiseQuoted);
            MouseLeave += new MouseEventHandler(RaiseClosePopout);
            MouseMove += new MouseEventHandler(RaiseMovePopup);
            MouseLeftButtonDown += new MouseButtonEventHandler(RaiseScrollToPost);
        }
        private void RaiseQuoted(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new(Quoted, this));
        }
        public void RaiseClosePopout(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new(ClosePopout, this));
            TextDecorations = null;
        }
        public void RaiseMovePopup(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Hand;
            TextDecorations = System.Windows.TextDecorations.Underline;
            RaiseEvent(new(MovePopup, this));
        }
        public void RaiseScrollToPost(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new(ScrollToPost, this));
        }
    }
}
