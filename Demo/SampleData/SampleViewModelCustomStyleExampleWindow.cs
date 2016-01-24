using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.ViewModel;

namespace Demo.SampleData
{
    class SampleViewModelCustomStyleExampleWindow : ViewModel.ViewModelExampleBase, IViewModelCustomStyleExampleWindow
    {



        private System.Collections.ObjectModel.ObservableCollection<TabBase> _itemCollection;

        public new System.Collections.ObjectModel.ObservableCollection<TabBase> ItemCollection
        {
            get
            {
                if (_itemCollection == null)
                {
                    _itemCollection = new System.Collections.ObjectModel.ObservableCollection<TabBase>();

                    _itemCollection.Add(CreateTab1());
                    _itemCollection.Add(CreateTab2());
                    _itemCollection.Add(CreateTab3());
                    _itemCollection.Add(CreateTabLoremIpsum());
                }
                return _itemCollection;
            }
            set { _itemCollection = value; }
        }
        public new TabBase SelectedTab
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
