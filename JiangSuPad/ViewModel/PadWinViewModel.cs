using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using FileConfigurationInterface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JiangSuPad.Config.ConfigModel;
using JiangSuPad.Window;
using LoggerDeclare;
using ParameterDeclare;

namespace JiangSuPad.ViewModel
{
    public class PadWinViewModel : ViewModelBase
    {
        private readonly IFileConfiguration _fileConfiguration;
        private readonly IParameterPass _parameterPass;
        private readonly ILogger _logger;

        public PadWinViewModel(ILogger logger,IFileConfiguration fileConfiguration,
            IParameterPass parameterPass)
        {
            _logger = logger;
            _fileConfiguration = fileConfiguration;
            _parameterPass = parameterPass;
            LoadCommand = new RelayCommand<ChromiumWebBrowser>(ExcuteLoadCommand);
            CloseCommand = new RelayCommand(ExcuteCloseCommand);
            OpenWinCommand = new RelayCommand(ExcuteOpenWinCommand);
        }
        public RelayCommand OpenWinCommand { get; }

        private void ExcuteOpenWinCommand()
        {
            var inputWin = new InputWin();
            inputWin.ShowDialog();
        }
        private ChromiumWebBrowser _browser;
        public RelayCommand<ChromiumWebBrowser> LoadCommand { get; }

        private async void ExcuteLoadCommand(ChromiumWebBrowser browser)
        {
            await InitMac();
            InitBrowser(browser);
            ShowHtmlPage();
            AddObserver();
        }

        private async Task InitMac()
        {
            var config = _fileConfiguration.GetConfig<Configs>();
            if (!string.IsNullOrEmpty(config.DeviceMac)) return;
            config.DeviceMac = GetMac();
            await _fileConfiguration.SetConfigurationInFileAsync(config);
        }

        private string GetMac()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            return interfaces == null
                ? string.Empty
                : BitConverter.ToString(interfaces.GetPhysicalAddress().GetAddressBytes());
        }
        private void AddObserver()
        {
            _parameterPass.AddObserveByKey(CardDataKey, key =>
            {
                var data = _parameterPass.GetDataByKey<string>(key);
                _browser.ExecuteScriptAsync($"readBack('{data}')");
                _logger.WriteInfoLog(data);
            });
        }

        private void InitBrowser(ChromiumWebBrowser browser)
        {
            _browser = browser;
            _browser.LoadError += _browser_LoadError;
        }

        private int _errorCount;
        private const int MaxErrorCount = 10;
        private bool _isShowErrorMsg;
        public bool IsShowErrorMsg
        {
            get => _isShowErrorMsg;
            set { _isShowErrorMsg = value;RaisePropertyChanged(); }
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

        private string _address;

        public string Address
        {
            get => _address;
            set { _address = value;RaisePropertyChanged(); }
        }
        private void ShowHtmlPage()
        {
            var config = _fileConfiguration.GetConfig<Configs>();
            Address =$"{config.PadUrl}?mac={config.DeviceMac}";
        }

        private const int CardDataKey = 10;
        public RelayCommand CloseCommand { get; }

        private void ExcuteCloseCommand()
        {
            Application.Current.Shutdown();
        }
    }
}