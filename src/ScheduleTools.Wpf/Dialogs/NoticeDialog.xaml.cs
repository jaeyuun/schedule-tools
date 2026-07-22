using System.Windows;

namespace ScheduleTools.Wpf.Dialogs
{
    public partial class NoticeDialog : Window
    {
        public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(nameof(DialogTitle), typeof(string), typeof(NoticeDialog), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty DialogContentProperty = DependencyProperty.Register(nameof(DialogContent), typeof(string), typeof(NoticeDialog), new PropertyMetadata(string.Empty));

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

        public NoticeDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public NoticeDialog(string title, string content) : this()
        {
            DialogTitle = title;
            DialogContent = content;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}