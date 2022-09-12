using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChanisterWpf
{
    public static class ScrollSack
    {
        public static DependencyObject LastDependencyObject { get; set; } = null;
        public static ScrollViewer LastScrollViewer { get; set; } = null;
        public static DependencyObject GetScrollViewer(this DependencyObject dependencyObject)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (dependencyObject is ScrollViewer) return dependencyObject;
            if (dependencyObject == LastDependencyObject && LastScrollViewer is not null) return LastScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }
        public static void HandleScrollSpeed(object sender, MouseWheelEventArgs e)
        {
            if (sender is not DependencyObject) return;

            FlowDocumentReader flowDocumentReader = (FlowDocumentReader)sender;
            ScrollViewer scrollViewer = ((DependencyObject)sender).GetScrollViewer() as ScrollViewer;
            if (flowDocumentReader.ViewingMode == FlowDocumentReaderViewingMode.Scroll)
            {
                double offset = scrollViewer.VerticalOffset - (e.Delta * Properties.Settings.Default.Scroll_speed / 6);
                if (offset < 0)
                    scrollViewer.ScrollToVerticalOffset(0);
                else if (offset > scrollViewer.ExtentHeight)
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
                else
                    scrollViewer.ScrollToVerticalOffset(offset);

                LastDependencyObject = sender as DependencyObject;
                LastScrollViewer = scrollViewer;
                e.Handled = true;
            }
        }
    }
}
