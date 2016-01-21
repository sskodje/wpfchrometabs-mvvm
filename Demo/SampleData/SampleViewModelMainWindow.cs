using Demo.ViewModel;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Demo.SampleData
{
    public class SampleViewModelMainWindow : ViewModelExampleBase, IViewModelMainWindow
    {
        public GalaSoft.MvvmLight.Command.RelayCommand AddTabCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public GalaSoft.MvvmLight.Command.RelayCommand<TabBase> PinTabCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public bool CanMoveTabs
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public GalaSoft.MvvmLight.Command.RelayCommand<TabBase> CloseTabCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<TabBase> _itemCollection;

        public System.Collections.ObjectModel.ObservableCollection<TabBase> ItemCollection
        {
            get
            {
                if (_itemCollection == null)
                {
                    _itemCollection = new System.Collections.ObjectModel.ObservableCollection<TabBase>();

                    _itemCollection.Add(CreateTab1());
                    _itemCollection.Add(CreateTab2());
                    _itemCollection.Add(CreateTab3());
                    _itemCollection.Add(CreateTab4());
                }
                return _itemCollection;
            }
            set { _itemCollection = value; }
        }


        public GalaSoft.MvvmLight.Command.RelayCommand<ChromeTabs.TabReorder> ReorderTabsCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public TabBase SelectedTab
        {
            get
            {
                return ItemCollection.FirstOrDefault();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ShowAddButton
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
