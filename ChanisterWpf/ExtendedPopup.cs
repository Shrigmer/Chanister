using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ChanisterWpf
{
    public class ExtendedPopup : Popup
    {
        public void MoveToCursor(double horizontalOffset, double verticalOffset)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            double horizontal = Mouse.GetPosition(mainWindow).X + horizontalOffset;
            double vertical = Mouse.GetPosition(mainWindow).Y + verticalOffset;
            if (mainWindow.WindowState == WindowState.Normal)
            {
                horizontal += mainWindow.Left;
                vertical += mainWindow.Top;
            }
            HorizontalOffset = horizontal;
            VerticalOffset = vertical;
        }
    }
}
