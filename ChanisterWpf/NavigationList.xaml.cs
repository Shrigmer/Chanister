using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChanisterWpf
{
    public partial class NavigationList : UserControl
    {
        readonly List<Board> boardlist = new();
        public static readonly RoutedEvent ClickBoardEvent = EventManager.RegisterRoutedEvent("ClickBoardEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Board));
        private readonly MainWindow MainWindow = ((MainWindow)Application.Current.MainWindow);
        public NavigationList()
        {
            InitializeComponent();
            string JsonDoc = MainWindow.HttpClient.GetStringAsync("https://a.4cdn.org/boards.json").Result;
            JsonDoc = JsonDoc.Remove(0, JsonDoc.IndexOf('['));
            JsonDoc = JsonDoc.Remove(JsonDoc.LastIndexOf(']') + 1);
            Dispatcher.Invoke(() =>
            {
                JsonNode jsonNode = JsonNode.Parse(JsonDoc);
                List<BoardData> boardDataList = jsonNode.Deserialize<List<BoardData>>();
                foreach (BoardData boardData in boardDataList)
                {
                    Board newBoard = new(boardData);
                    listView.Items.Add(newBoard);
                    boardlist.Add(newBoard);
                }
            });
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = e.Source as TextBox;
            listView.Items.Clear();
            foreach (Board navlink in boardlist)
            {
                if (navlink.LetterTextBlock.Text.ToLower() == box.Text.ToLower() ||
                    navlink.LetterTextBlock.Text.ToLower().Contains(box.Text.ToLower()) ||
                    navlink.NameTextBlock.Text.ToLower().Contains(box.Text.ToLower()))
                {
                    listView.Items.Add(navlink);
                }
            }
        }
        public void ToggleView()
        {
            Visibility = Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
        private void listView_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickBoardEvent, sender));
        }
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                foreach (Board board in listView.Items)
                {
                    if (board.LetterTextBlock.Text.ToLower() == (SearchBox.Text.ToLower()))
                    {
                        SearchBox.Clear();
                        listView.Focus();
                        listView.SelectedItem = board;
                        listView.ScrollIntoView(listView.SelectedItem);
                        RaiseEvent(new RoutedEventArgs(ClickBoardEvent, listView));
                        break;
                    }
                }
                e.Handled = true;
            }
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                listView.Focus();
                listView.SelectedIndex = 0;
                listView.ScrollIntoView(listView.SelectedItem);
                e.Handled = true;
            }
        }

        private void listView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RaiseEvent(new RoutedEventArgs(ClickBoardEvent, listView));
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (listView.SelectedItem == listView.Items[0])
                {
                    SearchBox.Focus();
                }
                else
                {
                    listView.Items.MoveCurrentToPrevious();
                    listView.ScrollIntoView(listView.SelectedItem);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (listView.Items.CurrentItem != listView.Items[listView.Items.Count - 1])
                {
                    listView.Items.MoveCurrentToNext();
                }
                listView.ScrollIntoView(listView.SelectedItem);
                e.Handled = true;
            }
        }
    }
}
