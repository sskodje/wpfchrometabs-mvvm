using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChromeTabs
{
    public class TabReorder
    {
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public bool ReorderSource { get; set; }
        public TabReorder(int from, int to, bool reorderSource = true)
        {
            this.FromIndex = from;
            this.ToIndex = to;
            this.ReorderSource = reorderSource;
        }
    }
}
