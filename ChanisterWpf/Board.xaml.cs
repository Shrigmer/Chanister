using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChanisterWpf
{
    public partial class Board : UserControl
    {
        public BoardData Data { get; set; }
        public ObservableCollection<Page> SubPages { get; set; } = new();
        private Page page;
        public Page Page
        {
            get { return page; }
            set
            {
                page = value;
                if (page == null)
                {
                    LetterTextBlock.FontWeight = FontWeights.Normal;
                    NameTextBlock.FontWeight = FontWeights.Normal;
                }
                else
                {
                    LetterTextBlock.FontWeight = FontWeights.Bold;
                    NameTextBlock.FontWeight = FontWeights.Bold;
                }
            }
        }

        public Board(BoardData data)
        {
            InitializeComponent();
            Data = data;
            DataContext = this;
            SubPages.CollectionChanged += (sender, e) =>
            {
                if (SubPages.Count > 0)
                {
                    PagesOpenTextBlock.Text = SubPages.Count.ToString();
                }
                else
                {
                    PagesOpenTextBlock.Text = "";
                }
            };
            if (Data.ws_board == 0)
            {
                LetterTextBlock.Foreground = Brushes.DarkRed;
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Page is not null) Page.Close();
            foreach (var subPage in SubPages)
            {
                subPage.Close(false);
            }
            SubPages = new();
            PagesOpenTextBlock.Text = "";
            Page = null;
        }

        private void CloseButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CloseButton.Opacity = 1;
        }

        private void CloseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CloseButton.Opacity = 0;
        }
    }
}
