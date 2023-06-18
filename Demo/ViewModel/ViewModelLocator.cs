/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Demo"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows.Navigation;


namespace Demo.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                // Create design time view services and models
                Ioc.Default.ConfigureServices(
                        new ServiceCollection()
                          .AddSingleton<IViewModelMainWindow, SampleData.SampleViewModelMainWindow>()
                          .AddSingleton<IViewModelPinnedTabExampleWindow, SampleData.SampleViewModelPinnedTabExampleWindow>()
                          .AddSingleton<IViewModelCustomStyleExampleWindow, SampleData.SampleViewModelCustomStyleExampleWindow>()
                          .BuildServiceProvider());
            }
            else
            {
                // Create run time view services and models
                Ioc.Default.ConfigureServices(
                        new ServiceCollection()
                          .AddSingleton<IViewModelMainWindow, ViewModelMainWindow>()
                          .AddSingleton<IViewModelPinnedTabExampleWindow, ViewModelPinnedTabExampleWindow>()
                          .AddSingleton<IViewModelCustomStyleExampleWindow, ViewModelCustomStyleExampleWindow>()
                          .BuildServiceProvider());
            }
        }

        public IViewModelCustomStyleExampleWindow VieWModelCustomStyleExampleWindow
        {
            get
            {
                return Ioc.Default.GetService<IViewModelCustomStyleExampleWindow>();
            }
        }

        public IViewModelMainWindow ViewModelMainWindow
        {
            get
            {
                return Ioc.Default.GetService<IViewModelMainWindow>();
            }
        }

        public IViewModelPinnedTabExampleWindow ViewModelPinnedTabExampleWindow
        {
            get
            {
                return Ioc.Default.GetService<IViewModelPinnedTabExampleWindow>();
            }

        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}