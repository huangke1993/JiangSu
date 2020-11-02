using FileConfigurationInterface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JiangSuPad.Config.ConfigModel;

namespace JiangSuPad.ViewModel
{
    public class TvWinViewModel:ViewModelBase
    {
        private readonly IFileConfiguration _fileConfiguration;
        public TvWinViewModel(IFileConfiguration fileConfiguration)
        {
            _fileConfiguration = fileConfiguration;
            LoadCommand=new RelayCommand(ExcuteLoadCommand);
        }
        public RelayCommand LoadCommand { get; }

        private void ExcuteLoadCommand()
        {
            ShowHtmlPage();
        }
        
        private string _address;

        public string Address
        {
            get => _address;
            set { _address = value; RaisePropertyChanged(); }
        }
        private void ShowHtmlPage()
        {
            Address = _fileConfiguration.GetConfig<Configs>().TvUrl;
        }
    }
}
