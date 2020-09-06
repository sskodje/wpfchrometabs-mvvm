using System;
using System.Windows.Controls;

namespace Demo.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl3.xaml
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        public UserControl3()
        {
            InitializeComponent();
            this.ID.Text = Guid.NewGuid().ToString();
        }
    }
}
