using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.ViewModel
{
    public class TabClass4 : TabBase
    {
        public string MyStringContent { get; set; }
        private bool _isBlinking;
        public bool IsBlinking
        {
            get { return _isBlinking; }
            set
            {
                if (_isBlinking != value)
                {
                    Set(() => IsBlinking, ref _isBlinking, value);
                }
            }
        }

    }
}
