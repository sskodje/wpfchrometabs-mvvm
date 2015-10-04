using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.ViewModel
{
   public class TabClass2 : TabBase
    {
        public string MyStringContent { get; set; }
        public int[] MyNumberCollection { get; set; }
        public int MySelectedNumber { get; set; }
    }
}
