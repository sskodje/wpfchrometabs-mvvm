using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
    public class TabClass3 : ITab
    {
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        public string MyStringContent { get; set; }
        public Uri MyImageUrl { get; set; }
    }
}
