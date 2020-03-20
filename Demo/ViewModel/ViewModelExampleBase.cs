using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ChromeTabs;
using Demo.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Demo.ViewModel
{
    public class ViewModelExampleBase : ViewModelBase
    {
        //since we don't know what kind of objects are bound, so the sorting happens outside with the ReorderTabsCommand.
        public RelayCommand<TabReorder> ReorderTabsCommand { get; set; }
        public RelayCommand AddTabCommand { get; set; }
        public RelayCommand<TabBase> CloseTabCommand { get; set; }
        public ObservableCollection<TabBase> ItemCollection { get; set; }

        //This is the current selected tab, if you change it, the tab is selected in the tab control.
        private TabBase _selectedTab;
        public TabBase SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    Set(() => SelectedTab, ref _selectedTab, value);
                }
            }
        }

        private bool _canAddTabs;
        public bool CanAddTabs
        {
            get => _canAddTabs;
            set
            {
                if (_canAddTabs == value) return;
                Set(() => CanAddTabs, ref _canAddTabs, value);
                AddTabCommand.RaiseCanExecuteChanged();
            }
        }


        public ViewModelExampleBase()
        {
            ItemCollection = new ObservableCollection<TabBase>();
            ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
            ReorderTabsCommand = new RelayCommand<TabReorder>(ReorderTabsCommandAction);
            AddTabCommand = new RelayCommand(AddTabCommandAction,()=>CanAddTabs);
            CloseTabCommand = new RelayCommand<TabBase>(CloseTabCommandAction);
            CanAddTabs = true;
        }

        protected TabClass1 CreateTab1()
        {
            var tab = new TabClass1 { TabName = "Tab class 1", MyStringContent = "Try drag the tab from left to right", TabIcon = new BitmapImage(new Uri("/Resources/1.png", UriKind.Relative)) };
            return tab;
        }
        protected TabClass2 CreateTab2()
        {
            var tab = new TabClass2 { TabName = "Tab class 2, with a long name", MyStringContent = "Try drag the tab outside the bonds of the tab control", MyNumberCollection = new[] { 1, 2, 3, 4 }, MySelectedNumber = 1, TabIcon = new BitmapImage(new Uri("/Resources/2.png", UriKind.Relative)) };
            return tab;

        }
        protected TabClass3 CreateTab3()
        {
            var tab = new TabClass3 { TabName = "Tab class 3", MyStringContent = "Try right clicking on the tab header. This tab can not be dragged out to a new window, to demonstrate that you can dynamically choose what tabs can, based on the viewmodel.", MyImageUrl = new Uri("/Resources/Kitten.jpg", UriKind.Relative), TabIcon = new BitmapImage(new Uri("/Resources/3.png", UriKind.Relative)) };
            return tab;
        }
        protected TabClass4 CreateTab4()
        {
            var tab = new TabClass4 { TabName = "Tab class 4", MyStringContent = "This tab demonstrates a custom tab header implementation", IsBlinking=true };
            return tab;
        }
        protected TabClass1 CreateTabLoremIpsum()
        {
            var tab = new TabClass1 { TabName = "Tab class 1", MyStringContent = Resources.LoremIpsum, TabIcon = new BitmapImage(new Uri("/Resources/1.png", UriKind.Relative)) };
            return tab;
        }
        /// <summary>
        /// Reorder the tabs and refresh collection sorting.
        /// </summary>
        /// <param name="reorder"></param>
        protected virtual void ReorderTabsCommandAction(TabReorder reorder)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            int from = reorder.FromIndex;
            int to = reorder.ToIndex;
            var tabCollection = view.Cast<TabBase>().ToList();//Get the ordered collection of our tab control

            tabCollection[from].TabNumber = tabCollection[to].TabNumber; //Set the new index of our dragged tab

            if (to > from)
            {
                for (int i = from + 1; i <= to; i++)
                {
                    tabCollection[i].TabNumber--; //When we increment the tab index, we need to decrement all other tabs.
                }
            }
            else if (from > to)//when we decrement the tab index
            {
                for (int i = to; i < from; i++)
                {
                    tabCollection[i].TabNumber++;//When we decrement the tab index, we need to increment all other tabs.
                }
            }

            view.Refresh();//Refresh the view to force the sort description to do its work.
        }

        //We need to set the TabNumber property on the viewmodels when the item source changes to keep it in sync.
        void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TabBase tab in e.NewItems)
                {
                    if (ItemCollection.Count > 1)
                    {
                        //If the new tab don't have an existing number, we increment one to add it to the end.
                        if (tab.TabNumber == 0)
                            tab.TabNumber = ItemCollection.OrderBy(x => x.TabNumber).LastOrDefault().TabNumber + 1;
                    }
                }
            }
            else
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
                view.Refresh();
                var tabCollection = view.Cast<TabBase>().ToList();
                foreach (var item in tabCollection)
                    item.TabNumber = tabCollection.IndexOf(item);
            }
        }

        //To close a tab, we simply remove the viewmodel from the source collection.
        private void CloseTabCommandAction(TabBase vm)
        {
            ItemCollection.Remove(vm);
        }

        //Adds a random tab
        private void AddTabCommandAction()
        {
            Random r = new Random();
            int num = r.Next(1, 100);
            if (num < 33)
                ItemCollection.Add(CreateTab1());
            else if (num < 66)
                ItemCollection.Add(CreateTab2());
            else
                ItemCollection.Add(CreateTab3());
        }
    }
}
