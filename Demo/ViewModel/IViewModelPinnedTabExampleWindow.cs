using CommunityToolkit.Mvvm.Input;

namespace Demo.ViewModel
{
    public interface IViewModelPinnedTabExampleWindow
    {
        RelayCommand<TabBase> PinTabCommand { get; set; }
    }
}
