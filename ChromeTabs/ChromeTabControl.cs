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
    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
    public class ChromeTabControl : Selector
    {
        private bool _addTabButtonClicked;
        private object _lastSelectedItem;
        private Panel itemsHolder;
        private Dictionary<object, DependencyObject> objectToContainerMap;

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

        public static readonly DependencyProperty PinTabCommandProperty =
    DependencyProperty.Register(
       "PinTabCommand",
       typeof(ICommand),
       typeof(ChromeTabControl));

        public ICommand PinTabCommand
        {
            get { return (ICommand)GetValue(PinTabCommandProperty); }
            set { SetValue(PinTabCommandProperty, value); }
        }

        public static readonly DependencyProperty AddTabCommandProperty =
    DependencyProperty.Register(
        "AddTabCommand",
        typeof(ICommand),
        typeof(ChromeTabControl), new PropertyMetadata(new PropertyChangedCallback(AddTabCommandPropertyChanged)));

        private static void AddTabCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
                return;
            ChromeTabControl ct = (ChromeTabControl)d;
            if (e.NewValue != null)
            {
                ICommand command = (ICommand)e.NewValue;
                command.CanExecuteChanged += ct.Command_CanExecuteChanged;

            }
            if (e.OldValue != null)
            {
                ICommand command = (ICommand)e.NewValue;
                command.CanExecuteChanged -= ct.Command_CanExecuteChanged;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            ((ChromeTabPanel)this.ItemsHost).IsAddButtonEnabled = AddTabCommand.CanExecute(AddTabCommandParameter);
        }
        public static DependencyProperty AddTabCommandParameterProperty =
DependencyProperty.Register("AddTabCommandParameter", typeof(object), typeof(ChromeTabControl));
        public object AddTabCommandParameter
        {
            get { return GetValue(AddTabCommandParameterProperty); }
            set { SetValue(AddTabCommandParameterProperty, value); }
        }
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

        // Provide CLR accessors for the event
        public event TabDragEventHandler TabDraggedOutsideBonds
        {
            add { AddHandler(TabDraggedOutsideBondsEvent, value); }
            remove { RemoveHandler(TabDraggedOutsideBondsEvent, value); }
        }

        // Using a RoutedEvent
        public static readonly RoutedEvent TabDraggedOutsideBondsEvent = EventManager.RegisterRoutedEvent(
            "TabDraggedOutsideBonds", RoutingStrategy.Bubble, typeof(TabDragEventHandler), typeof(ChromeTabControl));



        // Provide CLR accessors for the event
        public event ContainerOverrideEventHandler ContainerItemPreparedForOverride
        {
            add { AddHandler(ContainerItemPreparedForOverrideEvent, value); }
            remove { RemoveHandler(ContainerItemPreparedForOverrideEvent, value); }
        }

        // Using a RoutedEvent
        public static readonly RoutedEvent ContainerItemPreparedForOverrideEvent = EventManager.RegisterRoutedEvent(
            "ContainerItemPreparedForOverride", RoutingStrategy.Bubble, typeof(ContainerOverrideEventHandler), typeof(ChromeTabControl));



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
            if (DesignerProperties.GetIsInDesignMode(d))
                return;
            ChromeTabControl ctc = d as ChromeTabControl;

            ChromeTabPanel panel = (ChromeTabPanel)ctc.ItemsHost;
            if (panel != null)
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
            DependencyProperty.Register("SelectedTabBrush", typeof(Brush), typeof(ChromeTabControl), new PropertyMetadata(null, new PropertyChangedCallback(SelectedTabBrushPropertyCallback)));


        public Brush AddTabButtonBrush
        {
            get { return (Brush)GetValue(AddButtonBrushProperty); }
            set { SetValue(AddButtonBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddButtonBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonBrushProperty =
            DependencyProperty.Register("AddTabButtonBrush", typeof(Brush), typeof(ChromeTabControl), new PropertyMetadata(Brushes.Transparent));

        public Brush AddTabButtonMouseDownBrush
        {
            get { return (Brush)GetValue(AddButtonMouseDownBrushProperty); }
            set { SetValue(AddButtonMouseDownBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddButtonBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonMouseDownBrushProperty =
            DependencyProperty.Register("AddTabButtonMouseDownBrush", typeof(Brush), typeof(ChromeTabControl), new PropertyMetadata(Brushes.DarkGray));

        public Brush AddTabButtonMouseOverBrush
        {
            get { return (Brush)GetValue(AddButtonMouseOverBrushProperty); }
            set { SetValue(AddButtonMouseOverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddButtonMouseOverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonMouseOverBrushProperty =
            DependencyProperty.Register("AddTabButtonMouseOverBrush", typeof(Brush), typeof(ChromeTabControl), new PropertyMetadata(Brushes.White));


        public Brush AddTabButtonDisabledBrush
        {
            get { return (Brush)GetValue(AddButtonDisabledBrushProperty); }
            set { SetValue(AddButtonDisabledBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddButtonDisabledBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonDisabledBrushProperty =
            DependencyProperty.Register("AddTabButtonDisabledBrush", typeof(Brush), typeof(ChromeTabControl), new PropertyMetadata(Brushes.DarkGray));


        public double MinimumTabWidth
        {
            get { return (double)GetValue(MinimumTabWidthProperty); }
            set { SetValue(MinimumTabWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumTabWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumTabWidthProperty =
            DependencyProperty.Register("MinimumTabWidth", typeof(double), typeof(ChromeTabControl), new PropertyMetadata(40.0, OnMinimumTabWidthPropertyChanged));

        private static void OnMinimumTabWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromeTabControl ctc = (ChromeTabControl)d;
            ctc.CoerceValue(PinnedTabWidthProperty);
            ctc.CoerceValue(MaximumTabWidthProperty);
        }

        public double MaximumTabWidth
        {
            get { return (double)GetValue(MaximumTabWidthProperty); }
            set { SetValue(MaximumTabWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumTabWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumTabWidthProperty =
            DependencyProperty.Register("MaximumTabWidth", typeof(double), typeof(ChromeTabControl), new PropertyMetadata(125.0, null, OnCoerceMaximumTabWidth));

        private static object OnCoerceMaximumTabWidth(DependencyObject d, object baseValue)
        {
            ChromeTabControl ctc = (ChromeTabControl)d;

            if ((double)baseValue <= ctc.MinimumTabWidth)
                return ctc.MinimumTabWidth + 1;
            else
                return baseValue;
        }

        public double PinnedTabWidth
        {
            get { return (double)GetValue(PinnedTabWidthProperty); }
            set { SetValue(PinnedTabWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PinnedTabWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PinnedTabWidthProperty =
            DependencyProperty.Register("PinnedTabWidth", typeof(double), typeof(ChromeTabControl), new PropertyMetadata(40.0, null, OnCoercePinnedTabWidth));

        private static object OnCoercePinnedTabWidth(DependencyObject d, object baseValue)
        {
            ChromeTabControl ctc = (ChromeTabControl)d;

            if (ctc.MinimumTabWidth > (double)baseValue)
                return ctc.MinimumTabWidth;
            else
                return baseValue;
        }

        /// <summary>
        /// The extra pixel distance you need to drag up or down the tab before the tab tears out.
        /// </summary>
        public double TabTearTriggerDistance
        {
            get { return (double)GetValue(TabTearTriggerDistanceProperty); }
            set { SetValue(TabTearTriggerDistanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabTearTriggerDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabTearTriggerDistanceProperty =
            DependencyProperty.Register("TabTearTriggerDistance", typeof(double), typeof(ChromeTabControl), new PropertyMetadata(0.0));

        public double TabOverlap
        {
            get { return (double)GetValue(TabOverlapProperty); }
            set { SetValue(TabOverlapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabOverlap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabOverlapProperty =
            DependencyProperty.Register("TabOverlap", typeof(double), typeof(ChromeTabControl), new PropertyMetadata(10.0));


        /// <summary>
        /// Controls the persist behavior of tabs. All = all tabs live in memory, None = no tabs live in memory, Timed= tabs gets cleared from memory after a period of being unselected.
        /// </summary>
        public TabPersistBehavior TabPersistBehavior
        {
            get { return (TabPersistBehavior)GetValue(TabPersistBehaviorProperty); }
            set { SetValue(TabPersistBehaviorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabTearTriggerDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabPersistBehaviorProperty =
            DependencyProperty.Register("TabPersistBehavior", typeof(TabPersistBehavior), typeof(ChromeTabControl), new PropertyMetadata(TabPersistBehavior.None, OnTabPersistBehaviorPropertyChanged));

        private static void OnTabPersistBehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromeTabControl ct = (ChromeTabControl)d;
            if (((TabPersistBehavior)e.NewValue) == TabPersistBehavior.None)
            {
                ct.itemsHolder.Children.Clear();
            }
            else
            {
                ct.SetSelectedContent(false);
            }
        }

        /// <summary>
        /// The time an inactive tab stays cached in memory before being cleared. Default duration is 30 minutes.
        /// </summary>
        public TimeSpan TabPersistDuration
        {
            get { return (TimeSpan)GetValue(TabPersistDurationProperty); }
            set { SetValue(TabPersistDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabTearTriggerDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabPersistDurationProperty =
            DependencyProperty.Register("TabPersistDuration", typeof(TimeSpan), typeof(ChromeTabControl), new PropertyMetadata(TimeSpan.FromMinutes(30)));



        public AddTabButtonBehavior AddTabButtonBehavior
        {
            get { return (AddTabButtonBehavior)GetValue(AddTabButtonBehaviorProperty); }
            set { SetValue(AddTabButtonBehaviorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddTabButtonBehavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddTabButtonBehaviorProperty =
            DependencyProperty.Register("AddTabButtonBehavior", typeof(AddTabButtonBehavior), typeof(ChromeTabControl), new PropertyMetadata(AddTabButtonBehavior.OpenNewTab));



        public ControlTemplate AddButtonTemplate
        {
            get { return (ControlTemplate)GetValue(AddButtonTemplateProperty); }
            set { SetValue(AddButtonTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddButtonControlTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonTemplateProperty =
            DependencyProperty.Register("AddButtonTemplate", typeof(ControlTemplate), typeof(ChromeTabControl), new PropertyMetadata(null, OnAddButtonTemplateChanged));

        private static void OnAddButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromeTabControl ctc = (ChromeTabControl)d;
            ChromeTabPanel panel = ctc.ItemsHost as ChromeTabPanel;
            if (panel != null)
            {
                panel.SetAddButtonControlTemplate(e.NewValue as ControlTemplate);
            }
        }

        static ChromeTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeTabControl), new FrameworkPropertyMetadata(typeof(ChromeTabControl)));

        }
        public ChromeTabControl()
        {

            this.Loaded += ChromeTabControl_Loaded;
        }

        private void ChromeTabControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((ChromeTabPanel)ItemsHost).IsAddButtonEnabled = AddTabCommand.CanExecute(AddTabCommandParameter);
        }

        private static void SelectedTabBrushPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromeTabControl ctc = (ChromeTabControl)d;
            if (e.NewValue != null && ctc.SelectedItem != null)
                ctc.AsTabItem(ctc.SelectedItem).SelectedTabBrush = (Brush)e.NewValue;
        }
        /// <summary>
        /// Grabs hold of the tab based on the input viewmodel and positions it at the mouse cursor.
        /// </summary>
        /// <param name="viewModel"></param>
        public void GrabTab(object viewModel)
        {
            ChromeTabPanel p = (ChromeTabPanel)ItemsHost;
            ChromeTabItem item = AsTabItem(viewModel);
            p.StartTabDrag(item, true);
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

        internal void AddTab()
        {
            if (!CanAddTab)
            {
                return;
            }
            _addTabButtonClicked = true;
            if (this.AddTabCommand != null)
                this.AddTabCommand.Execute(null);

        }

        internal bool CanAddTab
        {
            get { return (bool)GetValue(CanAddTabProperty); }
        }

        internal void RemoveTab(object tab)
        {
            ChromeTabItem removeItem = this.AsTabItem(tab);
            if (CloseTabCommand != null)
                CloseTabCommand.Execute(removeItem.DataContext);
        }

        internal void PinTab(object tab)
        {
            ChromeTabItem removeItem = this.AsTabItem(tab);
            if (PinTabCommand != null)
                PinTabCommand.Execute(removeItem.DataContext);
        }

        internal void RemoveAllTabs(object exceptThis = null)
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
        internal void ChangeSelectedIndex(int index)
        {

            //  int index = this.GetTabIndex(item);
            if (Items.Count <= index)
                return;
            ChromeTabItem item = AsTabItem(Items[index]);
            ChangeSelectedItem(item);
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
            if (this.SelectedContent == null && item != null)
                SetSelectedContent(false);
        }

        internal void MoveTab(int fromIndex, int toIndex)
        {
            if (this.Items.Count == 0 || fromIndex == toIndex || fromIndex >= this.Items.Count)
            {
                return;
            }
            object fromTab = this.Items[fromIndex];
            object toTab = this.Items[toIndex];
            ChromeTabItem fromItem = AsTabItem(fromTab);
            ChromeTabItem toItem = AsTabItem(toTab);
            if (fromItem.IsPinned && !toItem.IsPinned)
                return;
            if (!fromItem.IsPinned && toItem.IsPinned)
                return;
            if (this.ReorderTabsCommand != null)
            {
                this.ReorderTabsCommand.Execute(new TabReorder(fromIndex, toIndex));
            }
            else
            {
                var sourceType = ItemsSource.GetType();
                if (sourceType.IsGenericType)
                {
                    var sourceDefinition = sourceType.GetGenericTypeDefinition();
                    if (sourceDefinition == typeof(ObservableCollection<>))
                    {
                        var method = sourceType.GetMethod("Move");
                        method.Invoke(ItemsSource, new object[] { fromIndex, toIndex });
                    }
                }
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
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            itemsHolder = GetTemplateChild("PART_ItemsHolder") as Panel;
            SetSelectedContent(false);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var tab = new ChromeTabItem();
            if (this.SelectedTabBrush != null)
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
            SetInitialSelection();
            KeyboardNavigation.SetIsTabStop(this, false);
        }

        protected void SetInitialSelection()
        {
            bool? somethingSelected = null;
            foreach (object element in this.Items)
            {
                if (element is DependencyObject)
                    somethingSelected |= ChromeTabItem.GetIsSelected((DependencyObject)element);
            }
            if (somethingSelected.HasValue && somethingSelected.Value == false)
            {
                this.SelectedIndex = 0;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (itemsHolder != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        {
                            var itemsToRemove = itemsHolder.Children.Cast<ContentPresenter>().Where(x => !Items.Contains(x.Content)).ToList();
                            foreach (var item in itemsToRemove)
                                itemsHolder.Children.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        {
                            // don't do anything with new items not created by the add button, because we don't want to
                            // create visuals that aren't being shown.
                            if (_addTabButtonClicked && AddTabButtonBehavior == AddTabButtonBehavior.OpenNewTab)
                            {
                                _addTabButtonClicked = false;
                                if (e.NewItems != null)
                                    ChangeSelectedItem(AsTabItem(e.NewItems.Cast<object>().Last()));
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                ContentPresenter cp = FindChildContentPresenter(item);
                                if (cp != null)
                                {
                                    itemsHolder.Children.Remove(cp);
                                }
                            }
                        }


                        break;
                }
            }
            SetSelectedContent(Items.Count == 0);
            this.SetChildrenZ();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {

            base.OnSelectionChanged(e);
            this.SetChildrenZ();


            SetSelectedContent(e.AddedItems.Count == 0);
        }
        protected void SetSelectedContent(bool removeContent)
        {
            if (removeContent)
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
                    else
                    {
                        this.SelectedItem = null;
                        this.SelectedContent = null;
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
            if (TabPersistBehavior != TabPersistBehavior.None)
            {
                if (item != null && itemsHolder != null)
                {
                    CreateChildContentPresenter(this.SelectedItem);
                    // show the right child
                    foreach (ContentPresenter child in itemsHolder.Children)
                    {
                        ChromeTabItem childTabItem = AsTabItem(child.Content);
                        child.Visibility = childTabItem.IsSelected ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }

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
            RaiseEvent(new ContainerOverrideEventArgs(ChromeTabControl.ContainerItemPreparedForOverrideEvent, this, item, AsTabItem(element)));
        }

        protected ChromeTabItem AsTabItem(object item)
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

        protected void SetChildrenZ()
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
        /// <summary>
        /// create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            ContentPresenter cp = FindChildContentPresenter(item);

            if (cp != null)
            {
                return cp;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();
            cp.Content = (item is ChromeTabItem) ? (item as ChromeTabItem).Content : item;
            //cp.ContentTemplate = this.SelectedContentTemplate;
            //cp.ContentTemplateSelector = this.SelectedContentTemplateSelector;
            //cp.ContentStringFormat = this.SelectedContentStringFormat;
            cp.Visibility = Visibility.Collapsed;
            //  cp.Tag = AsTabItem(item);// (item is ChromeTabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
            itemsHolder.Children.Add(cp);
            return cp;
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is ChromeTabItem)
            {
                data = (data as ChromeTabItem).Content;
            }

            if (data == null)
            {
                return null;
            }

            if (itemsHolder == null)
            {
                return null;
            }

            foreach (ContentPresenter cp in itemsHolder.Children)
            {
                if (cp.Content == data)
                {
                    return cp;
                }
            }

            return null;
        }

        internal void RemoveFromItemHolder(ChromeTabItem item)
        {
            if (itemsHolder == null)
                return;
            ContentPresenter presenter = FindChildContentPresenter(item);
            if (presenter != null)
            {
                itemsHolder.Children.Remove(presenter);
                Debug.WriteLine("Removing cached ContentPresenter");
            }

        }
    }
}
