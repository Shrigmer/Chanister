using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChanisterWpf
{
    public partial class CustomTabItem : Button
    {
        public static readonly RoutedEvent ClickTabEvent = EventManager.RegisterRoutedEvent("clickTabEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomTabItem));
        public Page RelatedPage { get; set; }
        public Grid RelatedGrid { get; set; }
        public bool isBoardTab = true;
        public CustomTabItem(Page page, Grid parentGrid)
        {
            InitializeComponent();
            RelatedPage = page;
            RelatedGrid = parentGrid;
            if (page.OPData != null)
            {
                if (page.OPData.sub == null)
                {
                    title.Text = page.OPData.no.ToString();
                }
                else
                {
                    title.Text = TagParser.DecodeText(page.OPData.sub);
                }
                ((MainWindow)Application.Current.MainWindow).Tabs.Stack.Children.Add(this);
            }
        }
        private void CustomTabItem_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickTabEvent, sender));
        }
        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RelatedPage.Close();
        }
    }
}
