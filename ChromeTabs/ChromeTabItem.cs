using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace ChromeTabs
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs;assembly=ChromiumTabs"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ChromiumTabItem/>
    ///
    /// </summary>
    public class ChromeTabItem : HeaderedContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(ChromeTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

        private static readonly RoutedUICommand closeTabCommand = new RoutedUICommand("Close tab", "CloseTab", typeof(ChromeTabItem));

        public static RoutedUICommand CloseTabCommand
        {
            get { return closeTabCommand; }
        }

        private static readonly RoutedUICommand closeAllTabsCommand = new RoutedUICommand("Close all tabs", "CloseAllTabs", typeof(ChromeTabItem));

        public static RoutedUICommand CloseAllTabsCommand
        {
            get { return closeAllTabsCommand; }
        }

        private static readonly RoutedUICommand closeOtherTabsCommand = new RoutedUICommand("Close other tabs", "CloseOtherTabs", typeof(ChromeTabItem));

        public static RoutedUICommand CloseOtherTabsCommand
        {
            get { return closeOtherTabsCommand; }
        }

        public static void SetIsSelected(DependencyObject item, bool value)
        {
            item.SetValue(ChromeTabItem.IsSelectedProperty, value);
        }

        public static bool GetIsSelected(DependencyObject item)
        {
            return (bool)item.GetValue(ChromeTabItem.IsSelectedProperty);
        }


        static ChromeTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeTabItem), new FrameworkPropertyMetadata(typeof(ChromeTabItem)));
            CommandManager.RegisterClassCommandBinding(typeof(ChromeTabItem), new CommandBinding(closeTabCommand, HandleCloseTabCommand));
            CommandManager.RegisterClassCommandBinding(typeof(ChromeTabItem), new CommandBinding(closeAllTabsCommand, HandleCloseAllTabsCommand));
            CommandManager.RegisterClassCommandBinding(typeof(ChromeTabItem), new CommandBinding(closeOtherTabsCommand, HandleCloseOtherTabsCommand));
        }

        private static void HandleCloseOtherTabsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ChromeTabItem item = sender as ChromeTabItem;
            if (item == null) { return; }
            item.ParentTabControl.RemoveAllTabs(item.DataContext);
        }

        private static void HandleCloseAllTabsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ChromeTabItem item = sender as ChromeTabItem;
            if (item == null) { return; }
            item.ParentTabControl.RemoveAllTabs();
        }

        private static void HandleCloseTabCommand(object sender, ExecutedRoutedEventArgs args)
        {
            ChromeTabItem item = sender as ChromeTabItem;
            if (item == null) { return; }
            item.ParentTabControl.RemoveTab(item);
        }
        public int Index
        {
            get
            {
                return ParentTabControl == null ? -1 : ParentTabControl.GetTabIndex(this);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Return)
            {
                ParentTabControl.ChangeSelectedItem(this);
            }
        }




        private ChromeTabControl ParentTabControl
        {
            get { return ItemsControl.ItemsControlFromItemContainer(this) as ChromeTabControl; }
        }
    }
}
