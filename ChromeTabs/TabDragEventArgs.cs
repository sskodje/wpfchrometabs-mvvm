using System.Windows;

namespace ChromeTabs
{
    public delegate void TabDragEventHandler(object sender, TabDragEventArgs e);
    public class TabDragEventArgs : RoutedEventArgs
    {
        public object Tab { get; set; }
        public Point CursorPosition { get; set; }
        public TabDragEventArgs(RoutedEvent routedEvent, object tab, Point cursorPos) : base(routedEvent) { Tab = tab; CursorPosition = cursorPos; }
        public TabDragEventArgs(RoutedEvent routedEvent, object source, object tab, Point cursorPos) : base(routedEvent, source) { Tab = tab; CursorPosition = cursorPos; }
    }
}
