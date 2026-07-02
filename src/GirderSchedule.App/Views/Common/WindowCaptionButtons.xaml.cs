using System.Windows;
using System.Windows.Controls;

namespace GirderSchedule.App.Views.Common
{
    public partial class WindowCaptionButtons : UserControl
    {
        public WindowCaptionButtons()
        {
            InitializeComponent();
        }

        private Window OwnerWindow
        {
            get { return Window.GetWindow(this); }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = OwnerWindow;

            if (window == null)
            {
                return;
            }

            window.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var window = OwnerWindow;

            if (window == null || window.ResizeMode == ResizeMode.NoResize)
            {
                return;
            }

            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = OwnerWindow;

            if (window == null)
            {
                return;
            }

            window.Close();
        }
    }
}