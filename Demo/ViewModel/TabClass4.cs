namespace Demo.ViewModel
{
    public class TabClass4 : TabBase
    {
        public string MyStringContent { get; set; }
        private bool _isBlinking;
        public bool IsBlinking
        {
            get => _isBlinking;
            set
            {
                if (_isBlinking != value)
                {
                    SetProperty(ref _isBlinking, value);
                }
            }
        }

    }
}
