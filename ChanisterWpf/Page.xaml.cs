using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ChanisterWpf
{
    public partial class Page : UserControl
    {
        public Board Board { get; set; }
        public PostData OPData { get; set; }
        public CustomTabItem RelatedTab { get; set; }
        public Grid RelatedGrid { get; set; }
        public Dictionary<int, Post> Posts = new();
        public bool BoardPage { get; set; } = true;
        public string Key { get; set; }
        public FlowDocument FlowDocument { get; set; } = null;
        public FlowDocumentReader FlowDocumentReader { get; set; } = null;
        public ScrollViewer Scroll { get; set; } = null;
        private readonly MainWindow MainWindow = ((MainWindow)Application.Current.MainWindow);
        public Page(Board board, Grid parentGrid)
        {
            InitializeComponent();
            Board = board;
            RelatedGrid = parentGrid;
            Scroll = new();
            grid.Children.Add(Scroll);
            WrapPanel wrapPanel = new() { };
            Scroll.Content = wrapPanel;
            Task.Run(() => { FetchThreads(wrapPanel); });
            Key = Board.Data.board;
            AddToMainWindowDict(Key); ;
            Board.Page = this;
        }
        public Page(PostData data, Board board, Grid parentGrid)
        {
            InitializeComponent();
            AddHandler(QuoteLink.ClosePopout, new RoutedEventHandler(TogglePopupHandler));
            AddHandler(QuoteLink.MovePopup, new RoutedEventHandler(MovePopupHandler));
            AddHandler(QuoteLink.Quoted, new RoutedEventHandler(QuotedHandler));
            AddHandler(QuoteLink.ScrollToPost, new RoutedEventHandler(ScrollToPostHandler));
            AddHandler(ThreadLink.ScrollToPost, new RoutedEventHandler(ScrollToPostHandler));
            OPData = data;
            BoardPage = false;
            Board = board;
            Board.SubPages.Add(this);
            RelatedGrid = parentGrid;
            RelatedTab = new CustomTabItem(this, RelatedGrid);
            FlowDocument = new() { Cursor = Cursors.Arrow, ColumnWidth = 400, ColumnGap = 25 };
            FlowDocumentReader = new() { ViewingMode = FlowDocumentReaderViewingMode.Scroll };
            FlowDocumentReader.Document = FlowDocument;
            FlowDocumentReader.PreviewMouseWheel += ScrollSack.HandleScrollSpeed;
            grid.Children.Add(FlowDocumentReader);
            FetchPosts();
            Key = RelatedTab.title.Text;
            AddToMainWindowDict(Key);
        }
        public void AddToMainWindowDict(string key) //Add page to mainwindow dictionary
        {
            MainWindow.Pages[key] = this;
            MainWindow.UpdatePageStatus();
        }
        void FetchThreads(WrapPanel wrapPanel)
        {
            JsonNode jsonNode = JsonNode.Parse(MainWindow.HttpClient.GetStringAsync($"https://a.4cdn.org/{Board.Data.board}/catalog.json").Result);
            foreach (JsonObject page in jsonNode.AsArray())
            {
                foreach (JsonObject thread in page["threads"].AsArray())
                {
                    PostData postData = thread.Deserialize<PostData>();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        wrapPanel.Children.Add(new OP(postData, Board));
                    });
                }
            }
        }
        public async void FetchPosts()
        {
            await Task.Run(async () =>
            {
                List<PostData> posts = new();
                try
                {
                    string JsonDoc = await MainWindow.HttpClient.GetStringAsync($"https://a.4cdn.org/{Board.Data.board}/thread/{OPData.no}.json");
                    JsonDoc = JsonDoc.Remove(0, JsonDoc.IndexOf('['));
                    JsonDoc = JsonDoc.Remove(JsonDoc.LastIndexOf(']') + 1);
                    JsonNode jsonNode = JsonNode.Parse(JsonDoc);
                    posts = jsonNode.Deserialize<List<PostData>>();
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Nigger", "Anyways...", MessageBoxButton.OK);
                }
                Dispatcher.Invoke(() =>
                {
                    foreach (PostData postData in posts)
                    {
                        if (Posts.ContainsKey(postData.no)) continue;

                        Post post = new(postData, Board, OPData.no);
                        Posts[post.PostData.no] = post;
                        FlowDocument.Blocks.Add(post.Paragraph);

                    }
                });
            });
        }
        public void Refresh()
        {
            if (BoardPage)
            {
                RelatedGrid.Children.Remove(this);
                RelatedGrid.Children.Add(new Page(Board, RelatedGrid));
            }
            else
            {
                FetchPosts();
            }
        }
        public void Close(bool removeSubpage = true)
        {
            RelatedGrid.Children.Remove(this);
            if (removeSubpage) Board.SubPages.Remove(this);
            MainWindow.LastPages = MainWindow.LastPages.Remove(this);
            MainWindow.NextPages = MainWindow.NextPages.Remove(this);
            if (MainWindow.CurrentPage == this)
            {
                MainWindow.CurrentPage = null;
            }
            MainWindow.Backward();
            MainWindow.Pages.Remove(Key);
            MainWindow.UpdatePageStatus();
            MainWindow.Tabs.Stack.Children.Remove(RelatedTab);
            if (RelatedTab is not null && RelatedTab.RelatedPage is not null) RelatedTab.RelatedPage = null; RelatedTab = null;
        }
        private void TogglePopupHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is QuoteLink)
            {
                QuoteLink quoteLink = (QuoteLink)e.OriginalSource;

                if (Posts.ContainsKey(quoteLink.PostQuoted))
                {
                    Posts[quoteLink.PostQuoted].ClosePopup();
                }
            }
        }
        private void MovePopupHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is QuoteLink)
            {
                QuoteLink quoteLink = (QuoteLink)e.OriginalSource;

                if (Posts.ContainsKey(quoteLink.PostQuoted))
                {
                    Posts[quoteLink.PostQuoted].HostInPopout();
                }
            }
        }
        private void QuotedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is QuoteLink)
            {
                QuoteLink quotelink = (QuoteLink)e.OriginalSource;
                try
                {
                    if (Posts.ContainsKey(quotelink.PostQuoted))
                    {
                        Post post = Posts[quotelink.PostQuoted];
                        post.Paragraph.Inlines.InsertAfter(post.LastQuote, new QuoteLink(quotelink.QuotedByPost, post.PostData.no, "", false));
                        post.QuotedBy.Add(new QuoteLink(quotelink.QuotedByPost, post.PostData.no, "", false));
                        post.LastQuote = post.LastQuote.NextInline;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private void ScrollToPostHandler(object sender, RoutedEventArgs e)
        {
            int postQuoted;
            if (e.OriginalSource is QuoteLink quotelink)
            {
                postQuoted = quotelink.PostQuoted;
            }
            else if (e.OriginalSource is ThreadLink threadLink)
            {
                postQuoted = threadLink.PostQuoted;
            }
            else
            {
                return;
            }
            try
            {
                if (Posts.ContainsKey(postQuoted))
                {
                    Post post = Posts[postQuoted];
                    Scroll = (ScrollViewer)FlowDocumentReader.GetScrollViewer();
                    double offsetPost = post.Paragraph.ElementStart.GetCharacterRect(LogicalDirection.Forward).Top;
                    double topMargin = 20;
                    if (offsetPost + Scroll.VerticalOffset - topMargin < 0)
                    {
                        Scroll.ScrollToVerticalOffset(0);
                    }
                    else
                    {
                        Scroll.ScrollToVerticalOffset(offsetPost + Scroll.VerticalOffset - topMargin);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

