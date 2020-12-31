using System.Threading;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
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
            LoadCommand = new RelayCommand<ChromiumWebBrowser>(ExcuteLoadCommand);
            CloseCommand=new RelayCommand(ExcuteCloseCommand);
        }
        public RelayCommand<ChromiumWebBrowser> LoadCommand { get; }

        private void ExcuteLoadCommand(ChromiumWebBrowser browser)
        {
            InitBrowser(browser);
            ShowHtmlPage();
        }
        private ChromiumWebBrowser _browser;
        private void InitBrowser(ChromiumWebBrowser browser)
        {
            _browser = browser;
            _browser.LoadError += _browser_LoadError;
        }
        private string _address;

        public string Address
        {
            get => _address;
            set { _address = value; RaisePropertyChanged(); }
        }
        private void ShowHtmlPage()
        {
            var config = _fileConfiguration.GetConfig<Configs>();
            Address = $"{config.TvUrl}?mac={config.DeviceMac}";
        }
        private int _errorCount;
        private const int MaxErrorCount = 10;
        private bool _isShowErrorMsg;
        public bool IsShowErrorMsg
        {
            get => _isShowErrorMsg;
            set { _isShowErrorMsg = value; RaisePropertyChanged(); }
        }

        private void _browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            _errorCount++;
            if (_errorCount >= MaxErrorCount)
            {
                IsShowErrorMsg = true;
                return;
            }
            Thread.Sleep(5000);
            _browser.Reload();
        }

        public RelayCommand CloseCommand { get; }

        private void ExcuteCloseCommand()
        {
            Application.Current.Shutdown();
        }
    }
}
