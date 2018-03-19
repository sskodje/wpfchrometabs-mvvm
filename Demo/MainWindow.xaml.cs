using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ChromeTabs;
using Demo.Utilities;
using Demo.ViewModel;
using static System.Windows.PresentationSource;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This event triggers when a tab is dragged outside the bonds of the tab control panel.
        /// We can use it to create a docking tab control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            if (MyChromeTabControl.InputHitTest(position) is FrameworkElement element)
            {
                ////test if the mouse is over the tab panel or a tab item.
                if (CanInsertTabItem(element))
                {
                    //TabBase dockedWindowVM = (TabBase)win.DataContext;
                    ViewModelExampleBase vm = (ViewModelExampleBase)DataContext;
                    vm.ItemCollection.Add(dockedWindowVM);
                    vm.SelectedTab = dockedWindowVM;
                    //We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
                    MyChromeTabControl.GrabTab(dockedWindowVM);
                    return true;
                }
            }
            return false;
        }

        private void BnOpenPinnedTabExample_Click(object sender, RoutedEventArgs e)
        {
            new PinnedTabExampleWindow().Show();
        }

        private void BnOpenCustomStyleExample_Click(object sender, RoutedEventArgs e)
        {
            new CustomStyleExampleWindow().Show();
        }
    }
}
