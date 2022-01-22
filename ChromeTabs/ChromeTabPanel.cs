using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ChromeTabs.Utilities;

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
    [ToolboxItem(false)]
    public class ChromeTabPanel : Panel
    {
        private const double _stickyReanimateDuration = 0.10;
        private const double _tabWidthSlidePercent = 0.5;
        private bool _isReleasingTab;
        private bool _hideAddButton;
        private Size _finalSize;
        private double _leftMargin;
        private double _rightMargin;
        private double _defaultMeasureHeight;
        private double _currentTabWidth;
        private int _captureGuard;
        private int _originalIndex;
        private int _slideIndex;
        private List<double> _slideIntervals;
        private ChromeTabItem _draggedTab;
        private Point _downPoint;
        private Point _downTabBoundsPoint;
        private ChromeTabControl _parent;
        private Rect _addButtonRect;
        private Button _addButton;
        private DateTime _lastMouseDown;
        private object _lockObject = new object();

        protected double Overlap => ParentTabControl?.TabOverlap ?? 10;
        protected double MinTabWidth => _parent?.MinimumTabWidth ?? 40;
        protected double MaxTabWidth => _parent?.MaximumTabWidth ?? 125;
        protected double PinnedTabWidth => _parent?.PinnedTabWidth ?? MinTabWidth;



        private bool _isAddButtonEnabled;

        public bool IsAddButtonEnabled
        {
            get => _isAddButtonEnabled;
            set
            {
                if (_isAddButtonEnabled != value)
                {
                    _isAddButtonEnabled = value;
                    _addButton.IsEnabled = value;
                    if (ParentTabControl != null)
                    {
                        _addButton.Background = value == false ? ParentTabControl.AddTabButtonDisabledBrush : ParentTabControl.AddTabButtonBrush;
                        InvalidateVisual();
                    }
                }
            }
        }

        static ChromeTabPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeTabPanel), new FrameworkPropertyMetadata(typeof(ChromeTabPanel)));
        }

        public ChromeTabPanel()
        {
            _leftMargin = 0.0;
            _rightMargin = 25.0;
            _defaultMeasureHeight = 30.0;
            ComponentResourceKey key = new ComponentResourceKey(typeof(ChromeTabPanel), "addButtonStyle");
            Style addButtonStyle = (Style)FindResource(key);
            _addButton = new Button { Style = addButtonStyle };
            this.Loaded += ChromeTabPanel_Loaded;
            this.Unloaded += ChromeTabPanel_Unloaded;
        }

        private void ChromeTabPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
                Window.GetWindow(this).Activated -= ChromeTabPanel_Dectivated;
        }

        private void ChromeTabPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
                Window.GetWindow(this).Deactivated += ChromeTabPanel_Dectivated;
        }

        private void ChromeTabPanel_Dectivated(object sender, EventArgs e)
        {

            if (_draggedTab != null && !IsMouseCaptured && !_isReleasingTab)
            {
                Point p = MouseUtilities.CorrectGetPosition(this);
                OnTabRelease(p, true, false);
            }
        }

        internal void SetAddButtonControlTemplate(ControlTemplate template)
        {
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Control.TemplateProperty, template));
            _addButton.Style = style;
        }

        protected override int VisualChildrenCount => base.VisualChildrenCount + 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index == VisualChildrenCount - 1)
            {
                return _addButton;
            }

            if (index < VisualChildrenCount - 1)
            {
                return base.GetVisualChild(index);
            }
            throw new IndexOutOfRangeException("Not enough visual children in the ChromeTabPanel.");
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            _rightMargin = ParentTabControl.IsAddButtonVisible ? 25 : 0;
            _currentTabWidth = CalculateTabWidth(finalSize);
            ParentTabControl.CanAddTabInternal=_currentTabWidth > MinTabWidth;

            if (_hideAddButton)
                _addButton.Visibility = Visibility.Hidden;
            else if (ParentTabControl.IsAddButtonVisible)
                _addButton.Visibility = _currentTabWidth > MinTabWidth ? Visibility.Visible : Visibility.Collapsed;
            else
                _addButton.Visibility = Visibility.Collapsed;

            _finalSize = finalSize;
            double offset = _leftMargin;
            foreach (UIElement element in Children)
            {
                double thickness = 0.0;
                ChromeTabItem item = ItemsControl.ContainerFromElement(ParentTabControl, element) as ChromeTabItem;
                thickness = item.Margin.Bottom;
                double tabWidth = element.DesiredSize.Width;
                element.Arrange(new Rect(offset, 0, tabWidth, finalSize.Height - thickness));
                offset += tabWidth - Overlap;
            }
            if (ParentTabControl.IsAddButtonVisible) {
                var addButtonSize = new Size(ParentTabControl.AddTabButtonWidth, ParentTabControl.AddTabButtonHeight);
                _addButtonRect = new Rect(new Point(offset + Overlap, (finalSize.Height - addButtonSize.Height) / 2), addButtonSize);
                _addButton.Arrange(_addButtonRect);
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _currentTabWidth = CalculateTabWidth(availableSize);
            ParentTabControl.CanAddTabInternal = _currentTabWidth > MinTabWidth;

            if (_hideAddButton)
                _addButton.Visibility = Visibility.Hidden;
            else if (ParentTabControl.IsAddButtonVisible)
                _addButton.Visibility = _currentTabWidth > MinTabWidth ? Visibility.Visible : Visibility.Collapsed;
            else
                _addButton.Visibility = Visibility.Collapsed;

            double height = double.IsPositiveInfinity(availableSize.Height) ? _defaultMeasureHeight : availableSize.Height;
            Size resultSize = new Size(0, availableSize.Height);
            foreach (UIElement child in Children)
            {
                ChromeTabItem item = ItemsControl.ContainerFromElement(ParentTabControl, child) as ChromeTabItem;
                Size tabSize = new Size(GetWidthForTabItem(item), height - item.Margin.Bottom);
                child.Measure(tabSize);
                resultSize.Width += child.DesiredSize.Width - Overlap;
            }
            if (ParentTabControl.IsAddButtonVisible)
            {
                var addButtonSize = new Size(ParentTabControl.AddTabButtonWidth, ParentTabControl.AddTabButtonHeight);
                _addButton.Measure(addButtonSize);
                resultSize.Width += addButtonSize.Width;
            }
            return resultSize;
        }
        private double GetWidthForTabItem(ChromeTabItem tab)
        {
            if (tab.IsPinned)
            {
                return PinnedTabWidth;
            }
            return _currentTabWidth;
        }

        private double CalculateTabWidth(Size availableSize)
        {
            double activeWidth = double.IsPositiveInfinity(availableSize.Width) ? 500 : availableSize.Width - _leftMargin - _rightMargin;
            int numberOfPinnedTabs = Children.Cast<ChromeTabItem>().Count(x => x.IsPinned);

            double totalPinnedTabsWidth = numberOfPinnedTabs > 0 ? ((numberOfPinnedTabs * PinnedTabWidth)) : 0;
            double totalNonPinnedTabsWidth = ((activeWidth) + (Children.Count - 1) * Overlap) - totalPinnedTabsWidth;
            return Math.Min(Math.Max(totalNonPinnedTabsWidth / (Children.Count - numberOfPinnedTabs), MinTabWidth), MaxTabWidth);
        }



        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            SetTabItemsOnTabs();
            if (Children.Count > 0)
            {
                if (Children[0] is ChromeTabItem)
                    ParentTabControl.ChangeSelectedItem(Children[0] as ChromeTabItem);
            }
            if (ParentTabControl != null && ParentTabControl.AddButtonTemplate != null)
            {
                SetAddButtonControlTemplate(ParentTabControl.AddButtonTemplate);
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            SetTabItemsOnTabs();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            lock (_lockObject)
            {
                if (_slideIntervals != null)
                {
                    return;
                }

                if (_addButtonRect.Contains(e.GetPosition(this)) && IsAddButtonEnabled)
                {
                    if (ParentTabControl != null)
                    {
                        _addButton.Background = ParentTabControl.AddTabButtonMouseDownBrush;
                        InvalidateVisual();
                    }
                    return;
                }

                //Check if we clicked the close button, and return if we do.
                DependencyObject originalSource = e.OriginalSource as DependencyObject;
                bool isButton = false;
                while (true)
                {
                    if (originalSource != null && originalSource.GetType() != typeof(ChromeTabPanel))
                    {
                        var parent = VisualTreeHelper.GetParent(originalSource);
                        if (parent is Button)
                        {
                            isButton = true;
                            break;
                        }
                        originalSource = parent;
                    }
                    else
                        break;
                }
                if (isButton)
                    return;

                _downPoint = e.GetPosition(this);
                StartTabDrag(_downPoint);

            }
        }
        internal void StartTabDrag(ChromeTabItem tab = null, bool isTabGrab = false)
        {
            Point downPoint = MouseUtilities.CorrectGetPosition(this);
            if (tab != null)
            {
                UpdateLayout();
                double totalWidth = 0;
                for (int i = 0; i < tab.Index; i++)
                {
                    totalWidth += GetWidthForTabItem(Children[i] as ChromeTabItem) - Overlap;
                }
                double xPos = totalWidth + ((GetWidthForTabItem(tab) / 2));
                _downPoint = new Point(xPos, downPoint.Y);
            }
            else
                _downPoint = downPoint;

            StartTabDrag(downPoint, tab, isTabGrab);
        }

        private ChromeTabItem GetTabFromMousePosition(Point mousePoint)
        {
            DependencyObject source = GetVisualItemFromMousePosition(mousePoint);
            while (source != null && !Children.Contains(source as UIElement))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source as ChromeTabItem;
        }
        private DependencyObject GetVisualItemFromMousePosition(Point mousePoint)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, mousePoint);
            DependencyObject source = result?.VisualHit;

            return source;
        }

        internal void StartTabDrag(Point p, ChromeTabItem tab = null, bool isTabGrab = false)
        {
            _lastMouseDown = DateTime.UtcNow;
            if (tab == null)
            {
                tab = GetTabFromMousePosition(_downPoint);
            }

            if (tab != null)
                _draggedTab = tab;
            else
            {
                //The mouse is not over a tab item, so just return.
                return;
            }

            if (_draggedTab != null)
            {
                if (Children.Count == 1
                    && ParentTabControl.DragWindowWithOneTab
                    && Mouse.LeftButton == MouseButtonState.Pressed
                    && !isTabGrab)
                {
                    _draggedTab = null;
                    Window.GetWindow(this).DragMove();
                }
                else
                {
                    _downTabBoundsPoint = MouseUtilities.CorrectGetPosition(_draggedTab);
                    SetZIndex(_draggedTab, 1000);
                    ParentTabControl.ChangeSelectedItem(_draggedTab);
                    if (isTabGrab)
                    {
                        ProcessMouseMove(new Point(p.X + 0.1, p.Y));
                    }
                }
            }
        }

        private void ProcessMouseMove(Point p)
        {
            Point nowPoint = p;
            if (ParentTabControl != null && ParentTabControl.IsAddButtonVisible && IsAddButtonEnabled)
            {
                if (_addButtonRect.Contains(nowPoint))
                {
                    _addButton.Background = ParentTabControl.AddTabButtonMouseOverBrush;
                    InvalidateVisual();
                }
                else
                {
                    _addButton.Background = ParentTabControl.AddTabButtonBrush;
                    InvalidateVisual();
                }
            }
            if (_draggedTab == null || !ParentTabControl.CanMoveTabs)
            {
                return;
            }

            Point insideTabPoint = TranslatePoint(p, _draggedTab);
            Thickness margin = new Thickness(nowPoint.X - _downPoint.X, 0, _downPoint.X - nowPoint.X, 0);


            int guardValue = Interlocked.Increment(ref _captureGuard);
            if (guardValue == 1)
            {
                _draggedTab.Margin = margin;

                //we capture the mouse and start tab movement
                _originalIndex = _draggedTab.Index;
                _slideIndex = _originalIndex + 1;
                //Add slide intervals, the positions  where the tab slides over the next.
                _slideIntervals = new List<double> { double.NegativeInfinity };

                for (int i = 1; i <= Children.Count; i += 1)
                {
                    var tab = Children[i - 1] as ChromeTabItem;
                    var diff = i - _slideIndex;
                    var sign = diff == 0 ? 0 : diff / Math.Abs(diff);
                    var bound = Math.Min(1, Math.Abs(diff)) * ((sign * GetWidthForTabItem(tab) * _tabWidthSlidePercent) + ((Math.Abs(diff) < 2) ? 0 : (diff - sign) * (GetWidthForTabItem(tab) - Overlap)));
                    _slideIntervals.Add(bound);
                }
                _slideIntervals.Add(double.PositiveInfinity);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (CaptureMouse())
                        Debug.WriteLine("has mouse capture=true");
                    else
                        Debug.WriteLine("has mouse capture=false");
                }));
            }
            else if (_slideIntervals != null)
            {
                if (insideTabPoint.X > 0 && (nowPoint.X + (_draggedTab.ActualWidth - insideTabPoint.X)) >= ActualWidth)
                {
                    return;
                }

                if (insideTabPoint.X < _downTabBoundsPoint.X && (nowPoint.X - insideTabPoint.X) <= 0)
                {
                    return;
                }
                _draggedTab.Margin = margin;
                //We return on small marging changes to avoid the tabs jumping around when quickly clicking between tabs.
                if (Math.Abs(_draggedTab.Margin.Left) < 10)
                    return;
                _addButton.Visibility = Visibility.Hidden;
                _hideAddButton = true;

                int changed = 0;
                int localSlideIndex = _slideIndex;
                if (localSlideIndex - 1 >= 0
                    && localSlideIndex - 1 < _slideIntervals.Count
                    && margin.Left < _slideIntervals[localSlideIndex - 1])
                {
                    SwapSlideInterval(localSlideIndex - 1);
                    localSlideIndex -= 1;
                    changed = 1;
                }
                else if (localSlideIndex + 1 >= 0
                    && localSlideIndex + 1 < _slideIntervals.Count
                    && margin.Left > _slideIntervals[localSlideIndex + 1])
                {
                    SwapSlideInterval(localSlideIndex + 1);
                    localSlideIndex += 1;
                    changed = -1;
                }
                if (changed != 0)
                {
                    var rightedOriginalIndex = _originalIndex + 1;
                    var diff = 1;
                    if (changed > 0 && localSlideIndex >= rightedOriginalIndex)
                    {
                        changed = 0;
                        diff = 0;
                    }
                    else if (changed < 0 && localSlideIndex <= rightedOriginalIndex)
                    {
                        changed = 0;
                        diff = 2;
                    }

                    int index = localSlideIndex - diff;
                    if (index >= 0 && index < Children.Count)
                    {
                        ChromeTabItem shiftedTab = Children[index] as ChromeTabItem;

                        if (!shiftedTab.Equals(_draggedTab)
                            && ((shiftedTab.IsPinned && _draggedTab.IsPinned) || (!shiftedTab.IsPinned && !_draggedTab.IsPinned)))
                        {
                            var offset = changed * (GetWidthForTabItem(_draggedTab) - Overlap);
                            StickyReanimate(shiftedTab, offset, _stickyReanimateDuration);
                            _slideIndex = localSlideIndex;
                        }
                    }
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            
            if (ParentTabControl != null && ParentTabControl.IsAddButtonVisible && IsAddButtonEnabled)
            {
                _addButton.Background = ParentTabControl.AddTabButtonBrush;
                InvalidateVisual();
            }
            
            if (_draggedTab != null && Mouse.LeftButton != MouseButtonState.Pressed && !_isReleasingTab)
            {
                Point p = e.GetPosition(this);
                OnTabRelease(p, true, false);
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            ProcessMouseMove(e.GetPosition(this));

            if (_draggedTab == null || DateTime.UtcNow.Subtract(_lastMouseDown).TotalMilliseconds < 50)
            {
                return;
            }
            Point nowPoint = e.GetPosition(this);
            bool isOutsideTabPanel = nowPoint.X < 0 - ParentTabControl.TabTearTriggerDistance
                || nowPoint.X > ActualWidth + ParentTabControl.TabTearTriggerDistance
                || nowPoint.Y < -(ActualHeight)
                || nowPoint.Y > ActualHeight + 5 + ParentTabControl.TabTearTriggerDistance;
            if (isOutsideTabPanel && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                object viewmodel = _draggedTab.Content;
                var eventArgs = new TabDragEventArgs(ChromeTabControl.TabDraggedOutsideBondsEvent, this, viewmodel, PointToScreen(e.GetPosition(this)));
                RaiseEvent(eventArgs);
                bool closeTab = eventArgs.Handled || ParentTabControl.CloseTabWhenDraggedOutsideBonds;
                OnTabRelease(e.GetPosition(this), IsMouseCaptured, closeTab, 0.01);//If we set it to 0 the completed event never fires, so we set it to a small decimal.

            }
        }


        private void OnTabRelease(Point p, bool isDragging, bool closeTabOnRelease, double animationDuration = _stickyReanimateDuration)
        {
            lock (_lockObject)
            {
                if (ParentTabControl != null && ParentTabControl.IsAddButtonVisible)
                {
                    if (_addButtonRect.Contains(p) && IsAddButtonEnabled)
                    {
                        _addButton.Background = ParentTabControl.AddTabButtonBrush;
                        InvalidateVisual();
                        if (_addButton.Visibility == Visibility.Visible)
                        {
                            ParentTabControl.AddTab();
                        }
                        return;
                    }

                }
                if (isDragging)
                {
                    ReleaseMouseCapture();
                    double offset = GetTabOffset();
                    int localSlideIndex = _slideIndex;
                    void completed()
                    {
                        if (_draggedTab != null)
                        {
                            try
                            {
                                ParentTabControl.ChangeSelectedItem(_draggedTab);
                                object vm = _draggedTab.Content;
                                _draggedTab.Margin = new Thickness(offset, 0, -offset, 0);
                                _draggedTab = null;
                                _captureGuard = 0;
                                ParentTabControl.MoveTab(_originalIndex, localSlideIndex - 1);
                                _slideIntervals = null;
                                _addButton.Visibility = Visibility.Visible;
                                _hideAddButton = false;
                                InvalidateVisual();
                                if (closeTabOnRelease && ParentTabControl.CloseTabCommand != null)
                                {
                                    Debug.WriteLine("sendt close tab command");
                                    ParentTabControl.CloseTabCommand.Execute(vm);
                                }
                                if (Children.Count > 1)
                                {
                                    //this fixes a bug where sometimes tabs got stuck in the wrong position.
                                    RealignAllTabs();
                                }
                            }
                            finally
                            {
                                _isReleasingTab = false;
                            }
                        }
                    }

                    if (Reanimate(_draggedTab, offset, animationDuration, completed))
                    {
                        _isReleasingTab = true;
                    }
                }
                else
                {
                    if (_draggedTab != null)
                    {
                        double offset = GetTabOffset();
                        ParentTabControl.ChangeSelectedItem(_draggedTab);
                        _draggedTab.Margin = new Thickness(offset, 0, -offset, 0);
                    }
                    _draggedTab = null;
                    _captureGuard = 0;
                    _slideIntervals = null;
                }
            }
        }

        private double GetTabOffset()
        {
            double offset = 0;
            if (_slideIntervals != null)
            {
                if (_slideIndex < _originalIndex + 1)
                {
                    offset = _slideIntervals[_slideIndex + 1] - GetWidthForTabItem(_draggedTab) * (1 - _tabWidthSlidePercent) + Overlap;
                }
                else if (_slideIndex > _originalIndex + 1)
                {
                    offset = _slideIntervals[_slideIndex - 1] + GetWidthForTabItem(_draggedTab) * (1 - _tabWidthSlidePercent) - Overlap;
                }
            }
            return offset;
        }

        private void RealignAllTabs()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var shiftedTab = Children[i] as ChromeTabItem;
                var offset = 1 * (GetWidthForTabItem(shiftedTab) - Overlap);
                shiftedTab.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            OnTabRelease(e.GetPosition(this), IsMouseCaptured, false);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            _parent = null;

        }

        private ChromeTabControl ParentTabControl
        {
            get
            {
                if (_parent == null)
                {
                    DependencyObject parent = this;
                    while (parent != null && !(parent is ChromeTabControl))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                    _parent = parent as ChromeTabControl;
                }
                return _parent;
            }
        }
        /*
         Unused method => 
        private UIElement GetTopContainer() => Application.Current.MainWindow.Content as UIElement;
        */
        private void StickyReanimate(ChromeTabItem tab, double left, double duration)
        {
            void completed()
            {
                if (_draggedTab != null)
                {
                    tab.Margin = new Thickness(left, 0, -left, 0);
                }
            }

            Reanimate(tab, left, duration, completed);
        }

        private bool Reanimate(ChromeTabItem tab, double left, double duration, Action completed)
        {
            if (tab == null)
            {
                return false;
            }
            Thickness offset = new Thickness(left, 0, -left, 0);
            ThicknessAnimation moveBackAnimation = new ThicknessAnimation(tab.Margin, offset, new Duration(TimeSpan.FromSeconds(duration)));
            Storyboard.SetTarget(moveBackAnimation, tab);
            Storyboard.SetTargetProperty(moveBackAnimation, new PropertyPath(MarginProperty));
            Storyboard sb = new Storyboard();
            sb.Children.Add(moveBackAnimation);
            sb.FillBehavior = FillBehavior.Stop;
            sb.AutoReverse = false;
            sb.Completed += (o, ea) =>
            {
                sb.Remove();
                completed?.Invoke();
            };
            sb.Begin();
            return true;
        }



        private void SetTabItemsOnTabs()
        {
            for (int i = 0; i < Children.Count; i += 1)
            {
                if (!(Children[i] is DependencyObject depObj))
                {
                    continue;
                }
                ChromeTabItem item = ItemsControl.ContainerFromElement(ParentTabControl, depObj) as ChromeTabItem;
                if (item != null)
                {
                    KeyboardNavigation.SetTabIndex(item, i);
                }
            }
        }

        private void SwapSlideInterval(int index)
        {
            _slideIntervals[_slideIndex] = _slideIntervals[index];
            _slideIntervals[index] = 0;
        }
    }
}
