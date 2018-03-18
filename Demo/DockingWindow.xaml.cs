using System.Windows;
using System.Windows.Media.Animation;

namespace Demo
{
    /// <summary>
    /// Interaction logic for DockingWindow.xaml
    /// </summary>
    public partial class DockingWindow : Window
    {
        public DockingWindow()
        {
            InitializeComponent();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["FadeInContentAnim"] as Storyboard;
            sb?.Begin();
        }
    }
}
