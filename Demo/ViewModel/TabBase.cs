using GalaSoft.MvvmLight;
using System;
using System.Windows.Media;
namespace Demo.ViewModel
{
    public abstract class TabBase : ViewModelBase
    {
        private int _tabNumber;
        public int TabNumber
        {
            get { return _tabNumber; }
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
            get { return _tabName; }
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
            get { return _isPinned; }
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
            get { return _tabIcon; }
            set
            {
                if (_tabIcon != value)
                {
                    Set(() => TabIcon, ref _tabIcon, value);
                }
            }
        }
    }
}
