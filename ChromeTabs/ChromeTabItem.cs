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
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace ChromeTabs
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromeTabs"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromeTabs;assembly=ChromeTabs"
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
    ///     <MyNamespace:ChromeTabs/>
    ///
    /// </summary>
    public class ChromeTabItem : HeaderedContentControl
    {
        private DispatcherTimer persistentTimer;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(ChromeTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnIsSelectedChanged)));


        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPinned.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.Register("IsPinned", typeof(bool), typeof(ChromeTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));



        public Brush SelectedTabBrush
        {
            get { return (Brush)GetValue(SelectedTabBrushProperty); }
            set { SetValue(SelectedTabBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabBrushProperty =
            DependencyProperty.Register("SelectedTabBrush", typeof(Brush), typeof(ChromeTabItem), new PropertyMetadata(Brushes.White));



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

        private static readonly RoutedUICommand pinTabCommand = new RoutedUICommand("Pin Tab", "PinTab", typeof(ChromeTabItem));

        public static RoutedUICommand PinTabCommand
        {
            get { return pinTabCommand; }
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
            CommandManager.RegisterClassCommandBinding(typeof(ChromeTabItem), new CommandBinding(pinTabCommand, HandlePinTabCommand));

        }
        public ChromeTabItem()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            this.Loaded += ChromeTabItem_Loaded;
        }

        private void ChromeTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ChromeTabItem_Loaded;
            this.Unloaded += ChromeTabItem_Unloaded;
        }

        private void ChromeTabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= ChromeTabItem_Unloaded;
            StoptPersistTimer();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChromeTabItem tabItem = (ChromeTabItem)d;

            if (tabItem.ParentTabControl != null && tabItem.ParentTabControl.TabPersistBehavior == TabPersistBehavior.Timed)
            {
                if ((bool)args.NewValue == true)
                {
                    tabItem.StoptPersistTimer();
                }
                else
                {
                    tabItem.StartPersistTimer();

                }
            }
        }

        private void StartPersistTimer()
        {
            StoptPersistTimer();

            persistentTimer = new DispatcherTimer();
            persistentTimer.Interval = ParentTabControl.TabPersistDuration;
            persistentTimer.Tick += PersistentTimer_Tick;
            persistentTimer.Start();
        }

        private void StoptPersistTimer()
        {
            if (persistentTimer != null)
            {
                persistentTimer.Stop();
                persistentTimer.Tick -= PersistentTimer_Tick;
                persistentTimer = null;
            }
        }


        private void PersistentTimer_Tick(object sender, EventArgs e)
        {
            StoptPersistTimer();
            if (ParentTabControl != null)
                ParentTabControl.RemoveFromItemHolder(this);
        }

        private static void HandlePinTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ChromeTabItem item = sender as ChromeTabItem;
            if (item == null) { return; }
            item.ParentTabControl.PinTab(item.DataContext);
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

        private static void HandleCloseTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ChromeTabItem item = sender as ChromeTabItem;
            if (item == null) { return; }
            item.ParentTabControl.RemoveFromItemHolder(item);
            item.ParentTabControl.RemoveTab(item);
        }
        public int Index
        {
            get
            {
                return ParentTabControl == null ? -1 : ParentTabControl.GetTabIndex(this);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Return)
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
