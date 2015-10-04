using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChromeTabs
{
    public delegate void ContainerOverrideEventHandler(object sender, ContainerOverrideEventArgs e);
    public class ContainerOverrideEventArgs : RoutedEventArgs
    {
        public object Model { get; set; }
        public ChromeTabItem TabItem { get; set; }
        public ContainerOverrideEventArgs(RoutedEvent routedEvent, object model, ChromeTabItem tabItem) : base(routedEvent) { this.Model = model; this.TabItem = tabItem; }
        public ContainerOverrideEventArgs(RoutedEvent routedEvent, object source, object model, ChromeTabItem tabItem) : base(routedEvent, source) { this.Model = model; this.TabItem = tabItem; }
    }
}
