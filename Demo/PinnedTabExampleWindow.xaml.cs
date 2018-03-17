using System.Windows;
using ChromeTabs;
using Demo.ViewModel;

namespace Demo
{
    /// <summary>
    /// Interaction logic for PinnedTabExample.xaml
    /// </summary>
    public partial class PinnedTabExampleWindow
    {
        public PinnedTabExampleWindow()
        {
            InitializeComponent();
        }

        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            if (e.TabItem != null && e.Model is TabBase viewModel)
            {
                e.TabItem.IsPinned = viewModel.IsPinned;
            }
        }
    }
}
