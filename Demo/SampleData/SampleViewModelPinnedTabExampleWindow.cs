using Demo.ViewModel;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.SampleData
{
    class SampleViewModelPinnedTabExampleWindow:ViewModelExampleBase,IViewModelPinnedTabExampleWindow
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

        private System.Collections.ObjectModel.ObservableCollection<TabBase> _itemCollection;

        public System.Collections.ObjectModel.ObservableCollection<TabBase> ItemCollection
        {
            get
            {
                if (_itemCollection == null)
                {
                    _itemCollection = new System.Collections.ObjectModel.ObservableCollection<TabBase>();
                    TabBase tab1 = CreateTab1();
                    tab1.IsPinned = true;
                    _itemCollection.Add(tab1);
                    _itemCollection.Add(CreateTab2());
                    _itemCollection.Add(CreateTab3());
                }
                return _itemCollection;
            }
            set { _itemCollection = value; }
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
    }
}
