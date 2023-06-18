using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace Demo.ViewModel
{
    public abstract class TabBase : ObservableRecipient
    {
        private int _tabNumber;
        public int TabNumber
        {
            get => _tabNumber;
            set
            {
                if (_tabNumber != value)
                {
                    SetProperty(ref _tabNumber, value);
                }
            }
        }

        private string _tabName;
        public string TabName
        {
            get => _tabName;
            set
            {
                if (_tabName != value)
                {
                    SetProperty(ref _tabName, value);
                }
            }
        }


        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (_isPinned != value)
                {
                    SetProperty(ref _isPinned, value);
                }
            }
        }


        private ImageSource _tabIcon;
        public ImageSource TabIcon
        {
            get => _tabIcon;
            set
            {
                if (!Equals(_tabIcon, value))
                {
                    SetProperty(ref _tabIcon, value);
                }
            }
        }
    }
}
