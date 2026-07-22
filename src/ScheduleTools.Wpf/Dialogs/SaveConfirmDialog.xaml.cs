using ScheduleTools.Wpf.Utilities;
using System.Windows;

namespace ScheduleTools.Wpf.Dialogs
{
    public partial class SaveConfirmDialog : Window
    {
        public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(nameof(DialogTitle), typeof(string), typeof(SaveConfirmDialog), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty DialogContentProperty = DependencyProperty.Register(nameof(DialogContent), typeof(string), typeof(SaveConfirmDialog), new PropertyMetadata(string.Empty));

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;

        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }

        public string DialogContent
        {
            get { return (string)GetValue(DialogContentProperty); }
            set { SetValue(DialogContentProperty, value); }
        }

        public SaveConfirmDialog()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += Window_Loaded;
        }

        public SaveConfirmDialog(string title, string content) : this()
        {
            DialogTitle = title;
            DialogContent = content;
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            DialogResult = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            DialogResult = null;
            Close();
        }
    }
}