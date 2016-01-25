# WPF Chrome Tabs - MVVM

A tab control based on [WPF chrome tabs](https://github.com/realistschuckle/wpfchrometabs), modified to work with the MVVM pattern.
WPF chrome tabs is a tab control modeled after the tabs in Google's Chrome browser.

New features include:

 - Support for dragging tabs to new windows, and snapping them back into tabs.
 - Support for pinned tabs.
 - Bindable property to lock tabs
 - Bindable property to show/hide "add tab" button
 - Bindable property for selected tab brush
 - Bindable property to drag window instead when dragging the last remaining tab.
 - Bindable propertes for minimum tab width, maximum tab width, and pinned tab width.

Available on [NuGet](https://www.nuget.org/packages/WPFChromeTabsMVVM/).

The basic functionality can be done as simple as :

####xaml:
```
        xmlns:ct="clr-namespace:ChromeTabs;assembly=ChromeTabs"
    
        <ct:ChromeTabControl ItemsSource="{Binding ItemCollection}"
                             SelectedItem="{Binding SelectedTab}"
                             AddTabCommand="{Binding AddTabCommand}"
                             CloseTabCommand="{Binding CloseTabCommand}">
```
####viewmodel: 
```
        public ObservableCollection<MyTabClass> ItemCollection { get; set; }
        public MyTabClass SelectedTab { get; set; }
        (...)
        void CloseTabCommandAction(MyTabClass tab)
        {
            ItemCollection.Remove(tab);
        }
        void AddTabCommandAction()
        {
            ItemCollection.Add(New MyTabClass());
        }
```

When using an ObservableCollection and without implementing the ReorderTabsCommand, tab reordering works out of the box by modifying the source collection order.
The demo project shows how to implement your own sorting logic using CollectionViewSource, for when you don't wish to modify the source collection or for more advanced sorting.

##New in 1.2:

 - Background tab visual tree can now be cached in memory via the TabPersistBehavior, either for the lifetime of the application, or for a chosen duration.
 - Added support for styling the "new tab" button.
 - Added a property to change "new tab" button behavior between opening tabs in the background or foreground.
 - Added support for changing the tab overlap.
 - Fixed an UI issue where "new tab" button was not visually disabled when command CanExecute was set to false.
 
The demo project has also been updated to show how you can fully style the control by using your own templates.

##New in 1.1:

 - Pinned tabs: Chrome style pinned tabs is now supported.
 - Bindable propertes for minimum tab width, maximum tab width, and pinned tab width is added
 - Fixed several glitches and bugs that some times occured when moving or switching tabs.

####Pinned tab implementation:

###### I strongly advice to check out the new demo project if you upgrade to 1.1 and plan to use pinned tabs. The old demo project contained sorting logic that can break pinned tab functionality if not changed.

Pinning tabs requires the "IsPinned" property to be set on the ChromeTabItem.

Subscribe to the ContainerItemPreparedForOverride event on ChromeTabControl, and set the IsPinned property:
```
        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            MyViewModel viewModel = e.Model as MyViewModel;
            if (e.TabItem != null && viewModel != null)
            {
                e.TabItem.IsPinned = viewModel.IsPinned;
            }
        }
```

Then we need to add a SortDescription to the CollectionViewSource to keep the pinned tabs in place. This description needs to be added before any other SortDescriptions:
```
     ICollectionView view = CollectionViewSource.GetDefaultView(MyTabCollection) as ICollectionView;
     view.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
```


See the demo project for full implementations of functionality like tab tearing, snapping windows to tabs, tab reordering, context menus, pinning tabs, and more.

![Example](http://i.imgur.com/q5WXWh1.gif)

