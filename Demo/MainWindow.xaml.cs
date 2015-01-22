using ChromeTabs;
using Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //We use this collection to keep track of what windows we have open
        private List<DockingWindow> _openWindows;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModelMainWindow();
            this._openWindows = new List<DockingWindow>();
        }

        /// <summary>
        /// This event triggers when a tab is dragged outside the bonds of the tab control panel.
        /// We can use it to create a docking tab control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChromeTabControl_TabDraggedOutsideBonds(object sender, TabDragEventArgs e)
        {
            ITab draggedTab = e.Tab as ITab;
            if (draggedTab is TabClass3)
                return;//We don't want out TabClass3 to form new windows, so we stop it here.
            DockingWindow win = _openWindows.FirstOrDefault(x => x.DataContext == draggedTab);//check if it's already open

            if (win == null)//If not, create a new one
            {
                win = new DockingWindow();
                win.Title = draggedTab.TabName;
                win.DataContext = draggedTab;
                win.Closed += win_Closed;
                win.Loaded += win_Loaded;
                win.LocationChanged += win_LocationChanged;
                win.Tag = e.CursorPosition;
                win.Show();
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " got window");
                MoveWindow(win, e.CursorPosition);
            }
            this._openWindows.Add(win);
            Debug.WriteLine(e.CursorPosition);
        }

        private void win_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = (Window)sender;
            Point cursorPosition = (Point)win.Tag;
            MoveWindow(win, cursorPosition);
        }
        private void MoveWindow(Window win, Point pt)
        {
            //Use a BeginInvoke to delay the execution slightly, else we can have problems grabbing the newly opened window.
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

                win.Loaded -= win_Loaded;
                win.Topmost = true;
                //We position the window at the mouse position
                win.Left = pt.X - win.Width + 200;
                win.Top = pt.Y - 20;
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " dragging window");

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    win.DragMove();//capture the movement to the mouse, so it can be dragged around
                }

                win.Topmost = false;
            }));
        }
        //remove the window from the open windows collection when it is closed.
        private void win_Closed(object sender, EventArgs e)
        {
            this._openWindows.Remove(sender as DockingWindow);
            Debug.WriteLine(DateTime.Now.ToShortTimeString() + " closed window");
        }
        //We use this to keep track of where the window is on the screen, so we can dock it later
        private void win_LocationChanged(object sender, EventArgs e)
        {
            Window win = (Window)sender;
            if (!win.IsLoaded)
                return;
            W32Point pt = new W32Point();
            if (!Win32.GetCursorPos(ref pt))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            Point absoluteScreenPos = new Point(pt.X, pt.Y);

            var windowUnder = FindWindowUnderThisAt(win, absoluteScreenPos);

            if (windowUnder != null && windowUnder.Equals(this))
            {
                Point relativePoint = this.PointFromScreen(absoluteScreenPos);//The screen position relative to the main window
                FrameworkElement element = this.MyChromeTabControl.InputHitTest(relativePoint) as FrameworkElement;//Hit test against the tab control

                if (element != null)
                {
                    object child = LogicalTreeHelper.GetChildren(element).Cast<object>().FirstOrDefault(x => x is ChromeTabPanel);
                    ChromeTabItem tabItem = element.TemplatedParent as ChromeTabItem;
                    //test if the mouse is over the tab panel or a tab item.
                    if (child!=null ||tabItem!=null)
                    {

                        ITab dockedWindowVM = (ITab)win.DataContext;
                        ViewModelMainWindow mainWindowVm = (ViewModelMainWindow)this.DataContext;

                        mainWindowVm.ItemCollection.Add(dockedWindowVM);
                      
                        win.Close();

                        mainWindowVm.SelectedTab = dockedWindowVM;

                        //We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
                        this.MyChromeTabControl.GrabTab(dockedWindowVM);

                    }
                }
            }
        }

        /// <summary>
        /// Used P/Invoke to find and return the top window under the cursor position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        private Window FindWindowUnderThisAt(Window source, Point screenPoint)  // WPF units (96dpi), not device units
        {
            var allWindows = SortWindowsTopToBottom(Application.Current.Windows.OfType<Window>());
            var windowsUnderCurrent = from win in allWindows
                                      where (win.WindowState == System.Windows.WindowState.Maximized || new Rect(win.Left, win.Top, win.Width, win.Height).Contains(screenPoint))
                                      && !Equals(win, source)
                                      select win;
            return windowsUnderCurrent.FirstOrDefault();
        }

        /// <summary>
        /// We need to do some P/Invoke magic to get the windows on screen
        /// </summary>
        /// <param name="unsorted"></param>
        /// <returns></returns>
        private IEnumerable<Window> SortWindowsTopToBottom(IEnumerable<Window> unsorted)
        {
            var byHandle = unsorted.ToDictionary(win =>
                ((HwndSource)PresentationSource.FromVisual(win)).Handle);

            for (IntPtr hWnd = Win32.GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = Win32.GetWindow(hWnd, Win32.GW_HWNDNEXT))
            {
                if (byHandle.ContainsKey(hWnd))
                    yield return byHandle[hWnd];
            }
        }
    }
}
