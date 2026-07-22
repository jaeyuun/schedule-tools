using GirderSchedule.Domain.Models.Settings;
using ScheduleTools.Wpf.Mvvm;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class DxfLayerItemViewModel : ViewModelBase
    {
        private string _layerName = string.Empty;
        private bool _isInvalid;

        public DxfLayerRole Role { get; private set; }
        public string DisplayName { get; private set; }

        public string LayerName
        {
            get { return _layerName; }
            set
            {
                if (_layerName == value)
                {
                    return;
                }

                _layerName = value;
                IsInvalid = false;

                OnPropertyChanged(nameof(LayerName));
            }
        }

        public bool IsInvalid
        {
            get { return _isInvalid; }
            set
            {
                if (_isInvalid == value)
                {
                    return;
                }

                _isInvalid = value;
                OnPropertyChanged(nameof(IsInvalid));
            }
        }

        public DxfLayerItemViewModel(DxfLayerRole role, string displayName, string layerName)
        {
            Role = role;
            DisplayName = displayName;
            LayerName = layerName;
        }
    }
}