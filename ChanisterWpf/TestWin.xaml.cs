using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace ChanisterWpf
{
    /// <summary>
    /// Interaction logic for TestWin.xaml
    /// </summary>
    public partial class TestWin : Window
    {
        PostData PostData = new() { com = "Seethe and cope reddit niggerfaggots", ext = ".webm", no = 12345, now = "mon. 12/3 19:00", w = 1920, h = 1080, filename = "cope_reddit_nigger.webm", fsize = 18000000 };
        public TestWin()
        {
            InitializeComponent();
            Paragraph paragraph = new()
            {
                IsHyphenationEnabled = true,
                FontFamily = new("Segoe UI"),
                FontSize = 12,
                //Cursor = Cursors.Arrow
            };
            paragraph.Inlines.Add(new Run(TagParser.DecodeText(PostData.name)) { Foreground = MainWindow.solidGreen });
            paragraph.Inlines.Add(new Run($"  {PostData.now}"));
            paragraph.Inlines.Add(new Run($"  {PostData.no}  "));
            paragraph.Inlines.Add(new Run(">>9999999"));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());


            string uri = "https://i.4cdn.org/wsg/1658978894133306s.jpg";
            MediaFrame customImage = null;
            if (PostData.ext == ".jpg" || PostData.ext == ".png")
            {
                customImage = new ImageFrame(new Uri(uri), PostData.filename.ToString(), PostData.ext.ToString(), PostData.w, PostData.h, PostData.fsize) { Margin = new Thickness(0) };
            }
            else if (PostData.ext == ".webm")
            {
                customImage = new WEBMFrame(new Uri(uri), PostData.filename.ToString(), PostData.ext.ToString(), PostData.w, PostData.h, PostData.fsize, PostData.tim) { Margin = new Thickness(0) };
            }
            if (customImage is not null)
            {
                BlockUIContainer imageContainer = new((UserControl)customImage) { Margin = new Thickness(0) };
                Figure imagefigure = new(imageContainer) { HorizontalAnchor = FigureHorizontalAnchor.ColumnLeft, Width = new FigureLength(300), Margin = new Thickness(0, 0, 15, 5) };
                paragraph.Inlines.Add(imagefigure);
                paragraph.Inlines.Add(new LineBreak());
            }

            if (PostData.sub != null)
            {
                paragraph.Inlines.Add(new Bold(new Run(Task.Run(() => { return TagParser.DecodeText(PostData.sub); }).Result + "  ")));
                paragraph.Inlines.Add(new LineBreak());
            }
            if (PostData.com != null)
            {
                PostData.com = Task.Run(() => TagParser.DecodeText(PostData.com)).Result;
                TagParser.AddInlines(Task.Run(() => { return TagParser.FindFormattedText(PostData.com); }).Result, paragraph.Inlines, PostData.no, PostData.no);
            }
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Line() { X1 = 0, Y1 = 10, X2 = 9001, Stroke = SystemColors.InactiveBorderBrush, StrokeThickness = 1 });
            flowdoc.Blocks.Add(paragraph);
        }
    }
}
