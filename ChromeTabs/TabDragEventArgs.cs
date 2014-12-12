using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChromeTabs
{
    public delegate void TabDragEventHandler(object sender, TabDragEventArgs e);
    public class TabDragEventArgs : RoutedEventArgs
    {
        public object Tab { get; set; }
        public Point CursorPosition { get; set; }
        public TabDragEventArgs(RoutedEvent routedEvent, object tab, Point cursorPos) : base(routedEvent) { this.Tab = tab; this.CursorPosition = cursorPos; }
        public TabDragEventArgs(RoutedEvent routedEvent, object source, object tab, Point cursorPos) : base(routedEvent, source) { this.Tab = tab; this.CursorPosition = cursorPos; }
    }
}
