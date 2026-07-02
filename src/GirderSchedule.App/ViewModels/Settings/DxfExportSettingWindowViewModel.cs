using GirderSchedule.App.Commands;
using GirderSchedule.App.Services;
using System;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class DxfExportSettingWindowViewModel : ViewModelBase
    {
        private bool _continueAcrossFloors = true;
        private string _formColumnCountText = "3";

        public event EventHandler Accepted;
        public event EventHandler Cancelled;

        public bool ContinueAcrossFloors
        {
            get { return _continueAcrossFloors; }
            set
            {
                if (_continueAcrossFloors == value)
                {
                    return;
                }

                _continueAcrossFloors = value;
                OnPropertyChanged(nameof(ContinueAcrossFloors));
            }
        }

        public string FormColumnCountText
        {
            get { return _formColumnCountText; }
            set
            {
                if (_formColumnCountText == value)
                {
                    return;
                }

                _formColumnCountText = value;
                OnPropertyChanged(nameof(FormColumnCountText));
            }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public DxfExportSettingWindowViewModel()
        {
            OkCommand = new RelayCommand(p => Ok());
            CancelCommand = new RelayCommand(p => Cancel());
        }

        public DxfExportSetting CreateSetting()
        {
            var setting = new DxfExportSetting();
            setting.ContinueAcrossFloors = ContinueAcrossFloors;
            setting.FormColumnCount = ParseFormColumnCount();
            return setting;
        }

        private void Ok()
        {
            var count = ParseFormColumnCount();

            if (count <= 0)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_TotalColumnCountMustBeAtLeastOne);
                return;
            }

            if (Accepted != null)
            {
                Accepted(this, EventArgs.Empty);
            }
        }

        private void Cancel()
        {
            if (Cancelled != null)
            {
                Cancelled(this, EventArgs.Empty);
            }
        }

        private int ParseFormColumnCount()
        {
            int value;

            if (!int.TryParse(FormColumnCountText, out value))
            {
                return 0;
            }

            if (value < 1)
            {
                return 0;
            }

            return value;
        }
    }
}