# WPF Chrome Tabs - MVVM

A tab control based on [WPF chrome tabs](https://github.com/realistschuckle/wpfchrometabs), modified to work with the MVVM pattern.
WPF chrome tabs is a tab control modeled after the tabs in Google's Chrome browser.

New features include:

 - Support for dragging tabs to new windows, and snapping them back into tabs.
 - Bindable property to lock tabs
 - Bindable property to show/hide "add tab" button
 - Bindable property for selected tab brush
 - Bindable property to drag window instead when dragging the last remaining tab.

Available on [NuGet](https://www.nuget.org/packages/WPFChromeTabsMVVM/).

The basic functionality can be done as simple as :

        xaml:
        xmlns:ct="clr-namespace:ChromeTabs;assembly=ChromeTabs"
    
        <ct:ChromeTabControl ItemsSource="{Binding ItemCollection}"
                             SelectedItem="{Binding SelectedTab}"
                             AddTabCommand="{Binding AddTabCommand}"
                             CloseTabCommand="{Binding CloseTabCommand}">
                             
        viewmodel: 
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

See the demo project for advanced functionality like tab tearing, snapping windows to tabs, tab reordering, context menus and more.

![ScreenShot](http://imgur.com/nneM0Kw)
