using System;
namespace Demo.ViewModel
{
    public interface IViewModelPinnedTabExampleWindow
    {
        GalaSoft.MvvmLight.Command.RelayCommand<TabBase> PinTabCommand { get; set; }
    }
}
