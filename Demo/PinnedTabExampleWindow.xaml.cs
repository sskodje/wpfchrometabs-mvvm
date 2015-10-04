using ChromeTabs;
using Demo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// Interaction logic for PinnedTabExample.xaml
    /// </summary>
    public partial class PinnedTabExampleWindow : Window
    {
        public PinnedTabExampleWindow()
        {
            InitializeComponent();
        }

        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            TabBase viewModel = e.Model as TabBase;
            if (e.TabItem != null && viewModel != null)
            {
                e.TabItem.IsPinned = viewModel.IsPinned;
            }
        }
    }
}
