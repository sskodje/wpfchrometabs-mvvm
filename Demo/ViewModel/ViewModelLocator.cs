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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

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
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                SimpleIoc.Default.Register<IViewModelMainWindow, SampleData.SampleViewModelMainWindow>();
                SimpleIoc.Default.Register<IViewModelPinnedTabExampleWindow, SampleData.SampleViewModelPinnedTabExampleWindow>();
                SimpleIoc.Default.Register<IViewModelCustomStyleExampleWindow, SampleData.SampleViewModelCustomStyleExampleWindow>();
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IViewModelMainWindow, ViewModelMainWindow>();
                SimpleIoc.Default.Register<IViewModelPinnedTabExampleWindow, ViewModelPinnedTabExampleWindow>();
                SimpleIoc.Default.Register<IViewModelCustomStyleExampleWindow, ViewModelCustomStyleExampleWindow>();
            }

        }

        public IViewModelCustomStyleExampleWindow VieWModelCustomStyleExampleWindow
        {
             get
            {
                return ServiceLocator.Current.GetInstance<IViewModelCustomStyleExampleWindow>();
            }
        }

        public IViewModelMainWindow ViewModelMainWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IViewModelMainWindow>();
            }
        }

        public IViewModelPinnedTabExampleWindow ViewModelPinnedTabExampleWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IViewModelPinnedTabExampleWindow>();
            }

        }

        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}