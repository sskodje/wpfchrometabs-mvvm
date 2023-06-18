using CommunityToolkit.Mvvm.Input;
using Demo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Demo
{
    public class WebViewTabObject
    {
        public string TabName { get; set; }
        public Uri TabUri { get; set; }

        public WebViewTabObject(string tabName = "Tab name", string defaultUri = "https://google.com")
        {
            TabName = tabName;
            TabUri = new Uri(defaultUri);
        }
    }

    /// <summary>
    /// Interaction logic for MinimalExampleWindow.xaml
    /// </summary>
    public partial class WebViewExampleWindow : Window, INotifyPropertyChanged
    {
        public RelayCommand AddTabCommand { get; set; }
        public RelayCommand<WebViewTabObject> CloseTabCommand { get; set; }
        public ObservableCollection<WebViewTabObject> TabsCollection { get; set; } = new ObservableCollection<WebViewTabObject>();

        private WebViewTabObject _selectedTab;
        public WebViewTabObject SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (!Equals(_selectedTab, value))
                {
                    _selectedTab = value;
                    RaisePropertyChanged(nameof(SelectedTab));
                }
            }
        }

        public WebViewExampleWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            AddTabCommand = new RelayCommand(AddTabCommandAction, () => true);
            CloseTabCommand = new RelayCommand<WebViewTabObject>(CloseTabCommandAction);
            AddTabCommand.Execute(null);
        }

        private void AddTabCommandAction()
        {
            TabsCollection.Add(new WebViewTabObject($"Tab#{Guid.NewGuid().ToString()}"));
        }

        private void CloseTabCommandAction(object tab)
        {
            TabsCollection.Remove(tab as WebViewTabObject);
        }
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
