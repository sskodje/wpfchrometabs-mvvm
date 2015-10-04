using ChromeTabs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Demo.ViewModel
{
    public class ViewModelPinnedTabExampleWindow : ViewModelExampleBase, IViewModelPinnedTabExampleWindow
    {
        public RelayCommand<TabBase> PinTabCommand { get; set; }


        public ViewModelPinnedTabExampleWindow()
        {
            TabBase vm1 = CreateTab1();
            vm1.IsPinned = true;
            this.ItemCollection.Add(vm1);
            this.ItemCollection.Add(CreateTab2());
            this.ItemCollection.Add(CreateTab3());
            this.SelectedTab = this.ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias,
            //as show below by sorting both by tab number and Pinned status.
            view.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            this.PinTabCommand = new RelayCommand<TabBase>(PinTabCommandAction);
        }
        private void PinTabCommandAction(TabBase tab)
        {
            tab.IsPinned = !tab.IsPinned;
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            view.Refresh();
        }
    }
}
