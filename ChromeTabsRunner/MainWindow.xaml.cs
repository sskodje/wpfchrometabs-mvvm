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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace ChromiumTabsRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.newTabNumber = 1;
        }

        private void HandleAddTab(object sender, RoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), false);
        }

        private void HandleAddTabAndSelect(object sender, RoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), true);
        }

        private object GenerateNewItem()
        {
            object itemToAdd = new Button { Content = "Moo " + this.newTabNumber };
            Interlocked.Increment(ref this.newTabNumber);
            if(this.title.Text.Length > 0)
            {
                itemToAdd = new ChromeTabs.ChromeTabItem
                {
                    Header = this.title.Text,
                    Content = itemToAdd
                };
            }
            return itemToAdd;
        }

        private void HandleRemoveTab(object sender, RoutedEventArgs e)
        {
            this.chrometabs.RemoveTab(this.chrometabs.SelectedItem);
        }

        private int newTabNumber;
    }
}
