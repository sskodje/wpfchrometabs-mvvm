using GalaSoft.MvvmLight.Command;

namespace Demo.ViewModel
{
    public interface IViewModelPinnedTabExampleWindow
    {
        RelayCommand<TabBase> PinTabCommand { get; set; }
    }
}
