using ChanisterWpf.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
// May the Sneed be with (you).
namespace ChanisterWpf
{

    public enum Skin { Classic_red, Classic_blue, White_is_tight, Sakura_purple }
    public partial class MainWindow : Window
    {
        public HttpClient HttpClient { get; set; } = new();
        public static readonly SolidColorBrush solidGreen = new((Color)ColorConverter.ConvertFromString("#789922"));
        public static readonly SolidColorBrush solidRed = new((Color)ColorConverter.ConvertFromString("#d00"));
        public Page CurrentPage = null;
        public Dictionary<string, Page> Pages = new();
        public double scale = 1;
        public Timer OneSecondPulse { get; set; }
        public ExtendedStack LastPages { get; set; } = new();
        public ExtendedStack NextPages { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();
            AddHandler(OP.MouseLeftUpOPEvent, new RoutedEventHandler(MouseLeftUpOP));
            AddHandler(NavigationList.ClickBoardEvent, new RoutedEventHandler(ClickBoard));
            AddHandler(CustomTabItem.ClickTabEvent, new RoutedEventHandler(ClickTabEvent));
            navList.SearchBox.Focus();
            OneSecondPulse = new()
            {
                Interval = 1000
            };
            OneSecondPulse.Elapsed += UpdateMemoryStatus;
            OneSecondPulse.Start();
        }
        private void ClickBoard(object sender, RoutedEventArgs e)
        {
            ListBox listbox = e.OriginalSource as ListBox;
            Board board = (Board)listbox.SelectedItem;
            DisplayBoardInPage(board);
        }
        public void DisplayBoardInPage(Board board)
        {
            if (Pages.ContainsKey(board.Data.board))
            {
                SetCurrentPage(Pages[board.Data.board]);
            }
            else
            {
                SetCurrentPage(new(board, contentGrid));
                contentGrid.Children.Add(CurrentPage);
            }
            foreach (KeyValuePair<string, Page> page in Pages)
            {
                if (page.Value.RelatedTab is not null)
                {
                    page.Value.RelatedTab.Visibility = (Visibility)VisibleBool(page.Value.Board.Data.board == board.Data.board);
                }
            }
        }
        private void MouseLeftUpOP(object sender, RoutedEventArgs e)
        {
            OP op = e.OriginalSource as OP;
            DisplayThreadInPage(op.PostData);
        }
        public void DisplayThreadInPage(PostData opData)
        {
            if (opData.sub != null && Pages.ContainsKey(opData.sub))
            {
                SetCurrentPage(Pages[opData.sub]);
            }
            else if (Pages.ContainsKey(opData.no.ToString()))
            {
                SetCurrentPage(Pages[opData.no.ToString()]);
            }
            else
            {
                SetCurrentPage(new(opData, CurrentPage.Board, contentGrid));
                contentGrid.Children.Add(CurrentPage);
            }
        }
        public void SetCurrentPage(Page newPage, bool pushToLastPages = true)
        {

            if (CurrentPage is not null)
            {
                CurrentPage.Visibility = Visibility.Collapsed;
                if (CurrentPage.RelatedTab is not null)
                {
                    Thickness thickness = CurrentPage.RelatedTab.BorderThickness;
                    CurrentPage.RelatedTab.BorderThickness = new(thickness.Left, thickness.Top, thickness.Right, thickness.Top);
                }
                LastPages.Remove(CurrentPage);
                NextPages.Remove(CurrentPage);
                if (pushToLastPages) LastPages.Push(CurrentPage);
            }
            CurrentPage = newPage;
            CurrentPage.Visibility = Visibility.Visible;
            if (CurrentPage.RelatedTab is not null)
            {
                Thickness thickness = CurrentPage.RelatedTab.BorderThickness;
                CurrentPage.RelatedTab.BorderThickness = new(thickness.Left, thickness.Top, thickness.Right, 0);
            }
            navList.listView.SelectedValue = CurrentPage.Board;
            navList.listView.ScrollIntoView(navList.listView.SelectedItem);

        }
        private void ClickTabEvent(object sender, RoutedEventArgs e)
        {
            CustomTabItem customTabItem = e.OriginalSource as CustomTabItem;
            SetCurrentPage(customTabItem.RelatedPage);
        }
        private void BannerToggler_Click(object sender, RoutedEventArgs e)
        {
            banner.Visibility = (Visibility)VisibleBool(BannerToggler.IsChecked);
            CollapseNavUIToggle();
        }
        private void ToggleNavigationList_Click(object sender, RoutedEventArgs e)
        {
            navList.Visibility = (Visibility)VisibleBool(NavigationListToggler.IsChecked);
            CollapseNavUIToggle();
        }
        private void CollapseNavUIToggle()
        {
            bool visible = NavigationListToggler.IsChecked || BannerToggler.IsChecked;
            navGrid.Visibility = (Visibility)VisibleBool(visible);
            if (visible)
            {
                Grid.SetColumn(contentGrid, 1);
                Grid.SetColumnSpan(contentGrid, 2);
                Tabs.Margin = new(navGrid.ActualWidth - dock.ActualWidth + 5.5, 0, 0, -1);
            }
            else
            {
                Grid.SetColumn(contentGrid, 0);
                Grid.SetColumnSpan(contentGrid, 3);
                Tabs.Margin = new(10, 0, 0, -1);
            }
        }
        public static int VisibleBool(bool visible)
        {
            return visible ? 0 : 2; //true -> 0 = Visible, false -> 2 = Collapsed
        }
        private void ChangeTheme(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scale = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
            CollapseNavUIToggle();
        }
        private void CreatePostButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePostDialog createPostDialog = new();
            createPostDialog.ShowDialog();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F5:
                    if (CurrentPage is not null) CurrentPage.Refresh();
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Back:
                    Backward();
                    e.Handled = true;
                    break;
            }
        }
        public void Backward()
        {
            Go(LastPages, NextPages);
        }
        public void Forward()
        {
            Go(NextPages, LastPages);
        }
        public void Go(ExtendedStack stack1, ExtendedStack stack2)
        {
            if (stack1.Count > 0)
            {
                if (CurrentPage is not null) stack2.Push(CurrentPage);
                SetCurrentPage((Page)stack1.Pop(), false);
            }
        }
        public void UpdatePageStatus()
        {
            pageStatus.Content = $"Pages: {Pages.Count}";
        }
        private void UpdateMemoryStatus(object sender, EventArgs e)
        {
            Process process = Process.GetCurrentProcess();
            process.Refresh();
            PerformanceCounter performanceCounter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
            int memory = (int)((double)performanceCounter.RawValue / 1024 / 1024);
            Dispatcher.Invoke(() =>
            {
                memoryStatus.Content = $"Memory: {memory} MB";
            });
        }

        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.XButton1 == System.Windows.Input.MouseButtonState.Pressed)
            {
                Backward();
                e.Handled = true;
            }
            else if (e.XButton2 == System.Windows.Input.MouseButtonState.Pressed)
            {
                Forward();
                e.Handled = true;
            }
        }
    }
}
