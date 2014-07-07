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
        public TabDragEventArgs(RoutedEvent routedEvent, object tab) : base(routedEvent) { this.Tab = tab; }
        public TabDragEventArgs(RoutedEvent routedEvent, object source, object tab) : base(routedEvent, source) { this.Tab = tab; }
    }
}
