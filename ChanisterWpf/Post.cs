using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace ChanisterWpf
{
    public class Post
    {
        public PostData PostData { get; set; }
        public Board Board { get; set; }
        public Paragraph Paragraph { get; set; }
        private ShadowedExtendedPopup Popup { get; set; } = null;
        public List<Inline> QuotedBy { get; set; }
        public Inline LastQuote { get; set; }
        public int OPNumber { get; set; }
        public Post(in PostData postData, in Board board, int opNumber)
        {
            PostData = postData;
            Board = board;
            OPNumber = opNumber;
            QuotedBy = new() { new Run("") };
            Paragraph = ConstructParagraph();
        }
        public Paragraph ConstructParagraph(bool endLine = true, int quotedByPost = 0)
        {
            Paragraph paragraph = new()
            {
                IsHyphenationEnabled = true,
                FontFamily = new("Segoe UI"),
                FontSize = 12,
            };
            paragraph.Inlines.Add(new Run(TagParser.DecodeText(PostData.name)) { Foreground = MainWindow.solidGreen });
            paragraph.Inlines.Add(new Run($"  {PostData.now}"));
            paragraph.Inlines.Add(new Run($"  {PostData.no}  "));
            paragraph.Inlines.AddRange(QuotedBy);
            LastQuote = QuotedBy[^1];
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Line() { X1 = 0, Y1 = 3, X2 = 9001 });
            string uri = $"https://i.4cdn.org/{Board.Data.board}/{PostData.tim}{PostData.ext}";
            MediaFrame customImage = null;
            switch (PostData.ext)
            {
                case ".jpg":
                case ".png":
                    customImage = new ImageFrame(new Uri(uri), PostData.filename.ToString(), PostData.ext.ToString(), PostData.w, PostData.h, PostData.fsize) { Margin = new Thickness(0) };
                    break;
                case ".webm":
                    customImage = new WEBMFrame(new Uri(uri), PostData.filename.ToString(), PostData.ext.ToString(), PostData.w, PostData.h, PostData.fsize, PostData.tim) { Margin = new Thickness(0) };
                    break;
                case ".gif":
                    customImage = new GIFFrame(new Uri(uri), PostData.filename.ToString(), PostData.ext.ToString(), PostData.w, PostData.h, PostData.fsize) { Margin = new Thickness(0) };
                    break;
            }
            if (customImage is not null)
            {
                BlockUIContainer imageContainer = new((UserControl)customImage) { Margin = new Thickness(0) };
                Figure imagefigure = new(imageContainer)
                {
                    HorizontalAnchor = FigureHorizontalAnchor.ColumnLeft,
                    Width = new FigureLength(customImage.Image.MaxWidth + 20),
                    Margin = new Thickness(0, 0, 15, 0)
                };
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(imagefigure);
            }
            if (PostData.sub is not null)
            {
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(new Bold(new Run(Task.Run(() => { return TagParser.DecodeText(PostData.sub); }).Result + "  ")));
                paragraph.Inlines.Add(new LineBreak());
            }
            if (PostData.com is not null)
            {
                if (customImage is not null)
                {
                    paragraph.Inlines.Add(new LineBreak());
                }
                PostData.com = Task.Run(() => TagParser.DecodeText(PostData.com)).Result;
                TagParser.AddInlines(Task.Run(() => { return TagParser.FindFormattedText(PostData.com); }).Result, paragraph.Inlines, PostData.no, OPNumber, quotedByPost);
            }
            if (endLine)
            {
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(new Line() { X1 = 0, Y1 = 5, X2 = 9001, Stroke = SystemColors.InactiveBorderBrush, StrokeThickness = 1 });
            }
            return paragraph;
        }
        public void ClosePopup()
        {
            if (Popup is not null)
            {
                Popup.IsOpen = false;
            }
        }
        internal void HostInPopout(int quotedByPost, QuoteLink quoteLink)
        {
            if (Popup is null)
            {
                Popup = new();
                FlowDocumentScrollViewer scrollViewer = new()
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Document = new FlowDocument(ConstructParagraph(false, quotedByPost))
                };
                Popup.MouseLeave += quoteLink.RaiseClosePopout;
                Popup.MouseMove += quoteLink.RaiseMovePopup;
                Popup.Grid.Children.Add(scrollViewer);
                Timer time = new()
                {
                    Interval = 60000
                };
                time.Elapsed += KillPopup;
                time.Start();
            }
            Popup.IsOpen = true;
            Popup.MoveToCursor(20, 10);
        }
        private void KillPopup(object sender, EventArgs e)
        {
            Timer time = (Timer)sender;
            time.Stop();
            Popup.Dispatcher.Invoke(() =>
            {
                Popup.Grid.Children.Clear();
                Popup = null;
            });
        }
    }
}
