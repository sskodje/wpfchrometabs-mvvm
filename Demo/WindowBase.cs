using ChromeTabs;
using Demo.Utilities;
using Demo.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static System.Windows.PresentationSource;

namespace Demo
{
    public abstract partial class WindowBase : Window
    {
        //We use this collection to keep track of what windows we have open
        protected List<DockingWindow> OpenWindows = new List<DockingWindow>();

        protected abstract bool TryDockWindow(Point position, TabBase dockedWindowVM);

        public WindowBase()
        {

        }

        protected bool TryDragTabToWindow(Point position, TabBase draggedTab)
        {
            if (draggedTab is TabClass3)
                return false;//As an example, we don't want TabClass3 to form new windows, so we stop it here.
            if (draggedTab.IsPinned)
                return false;//We don't want pinned tabs to be draggable either.

            DockingWindow win = OpenWindows.FirstOrDefault(x => x.DataContext == draggedTab);//check if it's already open

            if (win == null)//If not, create a new one
            {
                win = new DockingWindow
                {
                    Title = draggedTab?.TabName,
                    DataContext = draggedTab
                };

                win.Closed += win_Closed;
                win.Loaded += win_Loaded;
                win.LocationChanged += win_LocationChanged;
                win.Tag = position;
                win.Left = position.X - win.Width + 200;
                win.Top = position.Y - 20;
                win.Show();
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " got window");
                MoveWindow(win, position);
            }
            OpenWindows.Add(win);
            return true;
        }



        private void win_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = (Window)sender;
            win.Loaded -= win_Loaded;
            Point cursorPosition = (Point)win.Tag;
            MoveWindow(win, cursorPosition);
        }
        private void MoveWindow(Window win, Point pt)
        {
            //Use a BeginInvoke to delay the execution slightly, else we can have problems grabbing the newly opened window.
            Dispatcher.BeginInvoke(new Action(() =>
            {
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
            OpenWindows.Remove(sender as DockingWindow);
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
                Point relativePoint = PointFromScreen(absoluteScreenPos);//The screen position relative to the main window
                if (TryDockWindow(relativePoint, win.DataContext as TabBase))
                {
                    win.Close();
                }
            }
        }

        protected bool CanInsertTabItem(FrameworkElement element)
        {
            if (element is ChromeTabItem)
                return true;
            if (element is ChromeTabPanel)
                return true;
            object child = LogicalTreeHelper.GetChildren(element).Cast<object>().FirstOrDefault(x => x is ChromeTabPanel);
            if (child != null)
                return true;
            FrameworkElement localElement = element;
            while (true)
            {
                Object obj = localElement?.TemplatedParent;
                if (obj == null)
                    break;

                if (obj is ChromeTabItem)
                    return true;
                localElement = localElement.TemplatedParent as FrameworkElement;
            }
            return false;
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
                                      where (win.WindowState == WindowState.Maximized || new Rect(win.Left, win.Top, win.Width, win.Height).Contains(screenPoint))
                                      && !Equals(win, source)
                                      //This prevents "UI debugging tools for XAML" from interfering when debugging.
                                      && win.GetType().ToString() != "Microsoft.VisualStudio.DesignTools.WpfTap.WpfVisualTreeService.Adorners.AdornerLayerWindow"
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
                ((HwndSource)FromVisual(win)).Handle);

            for (IntPtr hWnd = Win32.GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = Win32.GetWindow(hWnd, Win32.GW_HWNDNEXT))
            {
                if (byHandle.ContainsKey(hWnd))
                    yield return byHandle[hWnd];
            }
        }
    }
}
