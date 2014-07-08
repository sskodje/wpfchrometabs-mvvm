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
using System.Collections.Specialized;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using System.Reflection;


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
    public class ChromeTabControl : Selector
    {
        private object _lastSelectedItem;

        internal static readonly DependencyPropertyKey CanAddTabPropertyKey = DependencyProperty.RegisterReadOnly("CanAddTab", typeof(bool), typeof(ChromeTabControl), new PropertyMetadata(true));
        public static readonly DependencyProperty CanAddTabProperty = CanAddTabPropertyKey.DependencyProperty;
        public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(ChromeTabControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        public static readonly DependencyProperty CloseTabCommandProperty =
    DependencyProperty.Register(
        "CloseTabCommand",
        typeof(ICommand),
        typeof(ChromeTabControl));

        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }


        public static readonly DependencyProperty AddTabCommandProperty =
    DependencyProperty.Register(
        "AddTabCommand",
        typeof(ICommand),
        typeof(ChromeTabControl));

        public ICommand AddTabCommand
        {
            get { return (ICommand)GetValue(AddTabCommandProperty); }
            set { SetValue(AddTabCommandProperty, value); }
        }

        public static readonly DependencyProperty ReorderTabsCommandProperty =
            DependencyProperty.Register(
            "ReorderTabsCommand",
            typeof(ICommand),
            typeof(ChromeTabControl));

        public ICommand ReorderTabsCommand
        {
            get { return (ICommand)GetValue(ReorderTabsCommandProperty); }
            set { SetValue(ReorderTabsCommandProperty, value); }
        }

        public static DependencyProperty ReorderTabsCommandParameterProperty =
    DependencyProperty.Register("ReorderTabsCommandParameter", typeof(object), typeof(ChromeTabControl));
        public object ReorderTabsCommandParameter
        {
            get { return GetValue(ReorderTabsCommandParameterProperty); }
            set { SetValue(ReorderTabsCommandParameterProperty, value); }
        }

        // Provide CLR accessors for the event
        public event TabDragEventHandler TabDraggedOutsideBonds
        {
            add { AddHandler(TabDraggedOutsideBondsEvent, value); }
            remove { RemoveHandler(TabDraggedOutsideBondsEvent, value); }
        }

        // Using a RoutedEvent
        public static readonly RoutedEvent TabDraggedOutsideBondsEvent = EventManager.RegisterRoutedEvent(
            "TabDraggedOutsideBonds", RoutingStrategy.Bubble, typeof(TabDragEventHandler), typeof(ChromeTabControl));



        public bool CloseTabWhenDraggedOutsideBonds
        {
            get { return (bool)GetValue(CloseTabWhenDraggedOutsideBondsProperty); }
            set { SetValue(CloseTabWhenDraggedOutsideBondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseTabWhenDraggedOutsideBonds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseTabWhenDraggedOutsideBondsProperty =
            DependencyProperty.Register("CloseTabWhenDraggedOutsideBonds", typeof(bool), typeof(ChromeTabControl), new PropertyMetadata(false));





        public bool IsAddButtonVisible
        {
            get { return (bool)GetValue(IsAddButtonVisibleProperty); }
            set { SetValue(IsAddButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAddButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAddButtonVisibleProperty =
            DependencyProperty.Register("IsAddButtonVisible", typeof(bool), typeof(ChromeTabControl), new PropertyMetadata(true, new PropertyChangedCallback(IsAddButtonVisiblePropertyCallback)));

        private static void IsAddButtonVisiblePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromeTabControl ctc = d as ChromeTabControl;

            ChromeTabPanel panel = (ChromeTabPanel)ctc.ItemsHost;
            panel.InvalidateVisual();
        }



        public bool CanMoveTabs
        {
            get { return (bool)GetValue(CanMoveTabsProperty); }
            set { SetValue(CanMoveTabsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanMoveTabs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanMoveTabsProperty =
            DependencyProperty.Register("CanMoveTabs", typeof(bool), typeof(ChromeTabControl), new PropertyMetadata(true));




        public bool DragWindowWithOneTab
        {
            get { return (bool)GetValue(DragWindowWithOneTabProperty); }
            set { SetValue(DragWindowWithOneTabProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragWindowWithOneTab.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragWindowWithOneTabProperty =
            DependencyProperty.Register("DragWindowWithOneTab", typeof(bool), typeof(ChromeTabControl), new PropertyMetadata(true));





        public Brush SelectedTabBrush
        {
            get { return (Brush)GetValue(SelectedTabBrushProperty); }
            set { SetValue(SelectedTabBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabBrushProperty =
            DependencyProperty.Register("SelectedTabBrush", typeof(Brush), typeof(ChromeTabControl), new PropertyMetadata(Brushes.AliceBlue));

        


        static ChromeTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeTabControl), new FrameworkPropertyMetadata(typeof(ChromeTabControl)));
        }

        /// <summary>
        /// Grabs hold of the tab based on the input viewmodel and positions it at the mouse cursor.
        /// </summary>
        /// <param name="viewModel"></param>
        public void GrabTab(object viewModel)
        {
            ChromeTabPanel p = (ChromeTabPanel)ItemsHost;
            ChromeTabItem item = AsTabItem(viewModel);
            p.StartTabDrag(item);
        }
        protected Panel ItemsHost
        {
            get
            {
                return (Panel)typeof(MultiSelector).InvokeMember("ItemsHost",
                    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                    null, this, null);
            }
        }

        public void AddTab()
        {
            if (!CanAddTab)
            {
                return;
            }
            if (this.AddTabCommand != null)
                this.AddTabCommand.Execute(null);
        }

        public bool CanAddTab
        {
            get { return (bool)GetValue(CanAddTabProperty); }
        }

        public void RemoveTab(object tab)
        {
            ChromeTabItem removeItem = this.AsTabItem(tab);
            if(CloseTabCommand!=null)
            CloseTabCommand.Execute(removeItem.DataContext);
        }

        public void RemoveAllTabs(object exceptThis = null)
        {
            var objects = this.ItemsSource.Cast<object>().Where(x => x != exceptThis).ToList();
            foreach (object obj in objects)
            {
                CloseTabCommand.Execute(obj);
            }
        }


        public object SelectedContent
        {
            get { return (object)GetValue(SelectedContentProperty); }
            set { SetValue(SelectedContentProperty, value); }
        }

        internal int GetTabIndex(ChromeTabItem item)
        {
            for (int i = 0; i < this.Items.Count; i += 1)
            {
                ChromeTabItem tabItem = this.AsTabItem(this.Items[i]);
                if (tabItem == item)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void ChangeSelectedItem(ChromeTabItem item)
        {
            int index = this.GetTabIndex(item);
            if (index != this.SelectedIndex)
            {
                if (index > -1)
                {
                    if (this.SelectedItem != null)
                    {
                        Canvas.SetZIndex(this.AsTabItem(this.SelectedItem), 0);
                    }
                    this.SelectedIndex = index;
                    Canvas.SetZIndex(item, 1001);
                }
            }
        }

        internal void MoveTab(int fromIndex, int toIndex)
        {
            if (this.Items.Count == 0 || fromIndex == toIndex)
            {
                return;
            }
            object fromTab = this.Items[fromIndex];
            if (this.ReorderTabsCommand != null)
            {
                ReorderTabsCommandParameter = new TabReorder(fromIndex, toIndex);
                this.ReorderTabsCommand.Execute(ReorderTabsCommandParameter);
            }

            for (int i = 0; i < this.Items.Count; i += 1)
            {
                var v = this.AsTabItem(this.Items[i]);
                v.Margin = new Thickness(0);
            }
            this.SelectedItem = fromTab;
        }

        internal void SetCanAddTab(bool value)
        {
            SetValue(CanAddTabPropertyKey, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
           var tab = new ChromeTabItem();
           tab.SelectedTabBrush = this.SelectedTabBrush;
           return tab;

        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ChromeTabItem);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            bool somethingSelected = false;
            foreach (UIElement element in this.Items)
            {
                somethingSelected |= ChromeTabItem.GetIsSelected(element);
            }
            if (!somethingSelected)
            {
                this.SelectedIndex = 0;
            }
            KeyboardNavigation.SetIsTabStop(this, false);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            this.SetChildrenZ();
        }



        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.SetChildrenZ();


            if (e.AddedItems.Count == 0)
            {
                if (this.SelectedItem == null)
                {
                    if (this.Items.Count > 0)
                    {
                        if (_lastSelectedItem != null)
                            this.SelectedItem = _lastSelectedItem;
                        else
                            this.SelectedItem = this.Items[0];
                    }
                }
                return;
            }

            if (this.SelectedIndex > 0)
            {
                this._lastSelectedItem = this.Items[this.SelectedIndex - 1];
            }
            else if (this.SelectedIndex == 0 && this.Items.Count > 1)
            {
                this._lastSelectedItem = this.Items[this.SelectedIndex + 1];
            }
            else
            {
                this._lastSelectedItem = null;
            }
            ChromeTabItem item = this.AsTabItem(this.SelectedItem);
            this.SelectedContent = item != null ? item.Content : null;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element != item)
            {
                this.ObjectToContainer[item] = element;
                this.SetChildrenZ();
            }
        }

        private ChromeTabItem AsTabItem(object item)
        {
            ChromeTabItem tabItem = item as ChromeTabItem;
            if (tabItem == null && item != null && this.ObjectToContainer.ContainsKey(item))
            {
                tabItem = this.ObjectToContainer[item] as ChromeTabItem;
            }
            return tabItem;
        }

        private Dictionary<object, DependencyObject> ObjectToContainer
        {
            get
            {
                if (objectToContainerMap == null)
                {
                    objectToContainerMap = new Dictionary<object, DependencyObject>();
                }
                return objectToContainerMap;
            }
        }

        private void SetChildrenZ()
        {
            int zindex = this.Items.Count - 1;
            foreach (object element in this.Items)
            {
                ChromeTabItem tabItem = this.AsTabItem(element);
                if (tabItem == null) { continue; }
                if (ChromeTabItem.GetIsSelected(tabItem))
                {
                    Panel.SetZIndex(tabItem, this.Items.Count);
                }
                else
                {
                    Panel.SetZIndex(tabItem, zindex);
                }
                zindex -= 1;
            }
        }

        private Dictionary<object, DependencyObject> objectToContainerMap;
    }
}
