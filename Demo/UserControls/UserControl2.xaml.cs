using System;
using System.Windows.Controls;

namespace Demo.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();
            this.ID.Text = Guid.NewGuid().ToString();
        }
    }
}
