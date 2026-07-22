using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScheduleTools.Wpf.Services
{
    public static class UiEditCommitService
    {
        public static void CommitAll()
        {
            Keyboard.ClearFocus();

            for (var i = 0; i < Application.Current.Windows.Count; i++)
            {
                var window = Application.Current.Windows[i];

                if (window == null)
                {
                    continue;
                }

                CommitDataGrids(window);
                CommitBindingGroup(window);
            }
        }

        private static void CommitDataGrids(DependencyObject source)
        {
            if (source == null)
            {
                return;
            }

            if (source is DataGrid dataGrid)
            {
                dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            }

            var count = VisualTreeHelper.GetChildrenCount(source);

            for (var i = 0; i < count; i++)
            {
                CommitDataGrids(VisualTreeHelper.GetChild(source, i));
            }
        }

        private static void CommitBindingGroup(FrameworkElement element)
        {
            if (element == null || element.BindingGroup == null)
            {
                return;
            }

            element.BindingGroup.CommitEdit();
        }
    }
}