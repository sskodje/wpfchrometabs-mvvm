using ChromeTabs;
using Demo.ViewModel;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for CustomStyleExampleWindow.xaml
    /// </summary>
    public partial class CustomStyleExampleWindow : WindowBase
    {
        public CustomStyleExampleWindow()
        {
            InitializeComponent();
        }

        private void TabControl_TabDraggedOutsideBonds(object sender, TabDragEventArgs e)
        {
            TabBase draggedTab = e.Tab as TabBase;
            if (TryDragTabToWindow(e.CursorPosition, draggedTab))
            {
                //Set Handled to true to tell the tab control that we have dragged the tab to a window, and the tab should be closed.
                e.Handled = true;
            }
        }

        protected override bool TryDockWindow(Point position, TabBase dockedWindowVM)
        {
            //Hit test against the tab control
            if (MyChromeTabControlWithCustomStyle.InputHitTest(position) is FrameworkElement element)
            {
                ////test if the mouse is over the tab panel or a tab item.
                if (CanInsertTabItem(element))
                {
                    //TabBase dockedWindowVM = (TabBase)win.DataContext;
                    ViewModelExampleBase vm = (ViewModelExampleBase)DataContext;
                    vm.ItemCollection.Add(dockedWindowVM);
                    vm.SelectedTab = dockedWindowVM;
                    //We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
                    MyChromeTabControlWithCustomStyle.GrabTab(dockedWindowVM);
                    return true;
                }
            }
            return false;
        }
    }
}
