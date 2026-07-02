using GirderSchedule.Domain.Models.Settings;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class DxfStyleItemViewModel : ViewModelBase
    {
        private string _styleName = string.Empty;
        private bool _isInvalid;

        public DxfStyleRole Role { get; private set; }
        public string DisplayName { get; private set; }

        public string StyleName
        {
            get { return _styleName; }
            set
            {
                if (_styleName == value)
                {
                    return;
                }

                _styleName = value;
                OnPropertyChanged(nameof(StyleName));
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

        public DxfStyleItemViewModel(DxfStyleRole role, string displayName, string styleName)
        {
            Role = role;
            DisplayName = displayName;
            StyleName = styleName;
        }
    }
}