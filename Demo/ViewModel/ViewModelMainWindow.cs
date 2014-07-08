using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using ChromeTabs;
using GalaSoft.MvvmLight;

namespace Demo
{
    public class ViewModelMainWindow:ViewModelBase
    {
        //I found no good way to sort the tabs inside the tab control with a bound collection, 
        //since we don't know what kind of objects are bound, so the sorting happens outside with the ReorderTabsCommand.
        public RelayCommand<TabReorder> ReorderTabsCommand { get; set; }
        public RelayCommand<ITab> CloseTabCommand { get; set; }
        public RelayCommand AddTabCommand { get; set; }

        public ObservableCollection<ITab> ItemCollection { get; set; }


        //This is the current selected tab, if you change it, the tab is selected in the tab control.
        private ITab _selectedTab;
        public ITab SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab != value)
                {
                    Set(() => SelectedTab, ref _selectedTab, value);
                }
            }
        }


        //this property is to show you can lock the tabs with a binding
        private bool _canMoveTabs;
        public bool CanMoveTabs
        {
            get { return _canMoveTabs; }
            set
            {
                if (_canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref _canMoveTabs, value);
                }
            }
        }
        //this property is to show you can bind the visibility of the add button
        private bool _showAddButton;
        public bool ShowAddButton
        {
            get { return _showAddButton; }
            set
            {
                if (_showAddButton != value)
                {
                    Set(() => ShowAddButton, ref _showAddButton, value);
                }
            }
        }


        public ViewModelMainWindow()
        {
            this.ItemCollection = new ObservableCollection<ITab>();
            this.ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
            AddTab1();
            AddTab2();
            AddTab3();
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;

            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending)); 

            this.CanMoveTabs = true;
            this.ShowAddButton = true;
            this.ReorderTabsCommand = new RelayCommand<TabReorder>(ReorderTabsCommandAction);
            this.CloseTabCommand = new RelayCommand<ITab>(CloseTabCommandAction);
            this.AddTabCommand = new RelayCommand(AddTabCommandAction);
        }

        private void AddTab1()
        {
            var tab = new TabClass1() { TabName = "Tab class 1", MyStringContent = "Try drag the tab from left to right" };
            this.ItemCollection.Add(tab);
            this.SelectedTab = tab;
        }
        private void AddTab2()
        {
            var tab = new TabClass2() { TabName = "Tab class 2, with a long name", MyStringContent = "Try drag the tab outside the bonds of the tab control", MyNumberCollection = new int[] { 1, 2, 3, 4, }, MySelectedNumber = 1 };
            this.ItemCollection.Add(tab);
            this.SelectedTab = tab;
        }
        private void AddTab3()
        {
            var tab = new TabClass3() { TabName = "Tab class 3", MyStringContent = "Try right clicking on the tab header. This tab can not be dragged out to a new window, to demonstrate that you can dynamically choose what tabs can, based on the viewmodel.", MyImageUrl = new Uri("/Resources/Kitten.jpg", UriKind.Relative) };
            this.ItemCollection.Add(tab);
            this.SelectedTab = tab;

        }

        //Adds a random tab
        private void AddTabCommandAction()
        {
            Random r = new Random();
            int num = r.Next(1, 100);
            if (num < 33)
                AddTab1();
            else if (num < 66)
                AddTab2();
            else
                AddTab3();
        }

        //To close a tab, we simply remove the viewmodel from the source collection.
        private void CloseTabCommandAction(ITab vm)
        {
            this.ItemCollection.Remove(vm);
        }


        /// <summary>
        /// Reorder the tabs and refresh collection sorting.
        /// </summary>
        /// <param name="reorder"></param>
        private void ReorderTabsCommandAction(TabReorder reorder)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            int from = reorder.FromIndex;
            int to = reorder.ToIndex;
            var sourceCol = view.SourceCollection.Cast<ITab>().OrderBy(x => x.TabNumber).ToList();//Get the ordered source collection of our tab control
            sourceCol[from].TabNumber = sourceCol[to].TabNumber; //Set the new index of our dragged tab

            if (to > from)
            {
                for (int i = from + 1; i <= to; i++)
                {
                    sourceCol[i].TabNumber--; //When we increment the tab index, we need to decrement all other tabs.
                }
            }
            else if (from > to)//when we decrement the tab index
            {
                for (int i = to; i < from; i++)
                {
                    sourceCol[i].TabNumber++;//When we decrement the tab index, we need to increment all other tabs.
                }
            }

            view.Refresh();//Refresh the view to force the sort description to do its work.
        }

        //We need to set the TabNumber property on the viewmodels when the item source changes.
        void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in this.ItemCollection)
                item.TabNumber = this.ItemCollection.IndexOf(item);
        }
    }
}
