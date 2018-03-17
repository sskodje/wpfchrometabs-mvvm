using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Demo.ViewModel
{
    public abstract class TabBase : ViewModelBase
    {
        private int _tabNumber;
        public int TabNumber
        {
            get => _tabNumber;
            set
            {
                if (_tabNumber != value)
                {
                    Set(() => TabNumber, ref _tabNumber, value);
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
                    Set(() => TabName, ref _tabName, value);
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
                    Set(() => IsPinned, ref _isPinned, value);
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
                    Set(() => TabIcon, ref _tabIcon, value);
                }
            }
        }
    }
}
