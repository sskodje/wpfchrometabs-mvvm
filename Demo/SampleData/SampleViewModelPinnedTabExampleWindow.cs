using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Demo.ViewModel;

namespace Demo.SampleData
{
    class SampleViewModelPinnedTabExampleWindow:ViewModelExampleBase,IViewModelPinnedTabExampleWindow
    {

        public RelayCommand<TabBase> PinTabCommand
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        private ObservableCollection<TabBase> _itemCollection;

        public new ObservableCollection<TabBase> ItemCollection
        {
            get
            {
                if (_itemCollection != null) return _itemCollection;
                _itemCollection = new ObservableCollection<TabBase>();
                TabBase tab1 = CreateTab1();
                tab1.IsPinned = true;
                _itemCollection.Add(tab1);
                _itemCollection.Add(CreateTab2());
                _itemCollection.Add(CreateTab3());
                _itemCollection.Add(CreateTabLoremIpsum());
                return _itemCollection;
            }
            set => _itemCollection = value;
        }
        public new TabBase SelectedTab
        {
            get => ItemCollection.FirstOrDefault();
            set => throw new NotImplementedException();
        }
    }
}
