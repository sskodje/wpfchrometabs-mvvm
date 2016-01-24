using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Demo.ViewModel
{
    public class ViewModelCustomStyleExampleWindow : ViewModelExampleBase, IViewModelCustomStyleExampleWindow
    {

        public ViewModelCustomStyleExampleWindow()
        {
            this.ItemCollection.Add(CreateTab1());
            this.ItemCollection.Add(CreateTab2());
            this.ItemCollection.Add(CreateTab3());
            this.ItemCollection.Add(CreateTab4());
           
            

            this.SelectedTab = this.ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
        }
    }
}
