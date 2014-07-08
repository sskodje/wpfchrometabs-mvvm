using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
   public class TabClass2 : ITab
    {
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        public string MyStringContent { get; set; }
        public int[] MyNumberCollection { get; set; }
        public int MySelectedNumber { get; set; }
    }
}
