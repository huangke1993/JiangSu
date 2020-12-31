using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using ConmonMessage;
using FileConfigurationInterface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ICardReaderDeclare;
using ICardReaderDeclare.Enum;
using JiangSuPad.Config.ConfigModel;
using LoggerDeclare;
using ParameterDeclare;
using Timer = System.Threading.Timer;

namespace JiangSuPad.ViewModel
{
    public class PadWinViewModel : ViewModelBase
    {
        private readonly ICardReader _cardReader;
        private readonly ILogger _logger;
        private readonly IFileConfiguration _fileConfiguration;
        private readonly IParameterPass _parameterPass;

        public PadWinViewModel(ICardReader cardReader, ILogger logger, IFileConfiguration fileConfiguration,
            IParameterPass parameterPass)
        {
            _cardReader = cardReader;
            _logger = logger;
            _fileConfiguration = fileConfiguration;
            _parameterPass = parameterPass;
            LoadCommand = new RelayCommand<ChromiumWebBrowser>(ExcuteLoadCommand);
            UnLoadCommand = new RelayCommand(ExcuteUnLoadCommand);
            CloseCommand = new RelayCommand(ExcuteCloseCommand);
        }
        public RelayCommand UnLoadCommand { get; }

        private void ExcuteUnLoadCommand()
        {
            _timer.Dispose();
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
                var data = _parameterPass.GetDataByKey<IPersonInfo>(key);
                _browser.ExecuteScriptAsync($"readBack('{data.Name}|{data.IdNum}')");
            });
        }

        private void InitBrowser(ChromiumWebBrowser browser)
        {
            _browser = browser;
            _browser.LoadingStateChanged += _browser_LoadingStateChanged;
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

        private void _browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading) return;
            EveryFiveSecondToReadCard();
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

        private Timer _timer;
        private void EveryFiveSecondToReadCard()
        {
            if (_timer != null) return;
            _timer=new Timer(TimerCallback,null,5000,Timeout.Infinite);
        }

        private void TimerCallback(object state)
        {
            ReadIdCard();
            _timer.Change(5000, Timeout.Infinite);
        }

        private const int CardDataKey = 10;

        private void ReadIdCard()
        {
            var idData = _cardReader.ReadIdCardAsync().Result;
            if (DealResult(idData)) return;
            var socialData = _cardReader.ReadSocialCardAsync(CardType.Contact).Result;
            DealResult(socialData);
        }

        private bool DealResult(IMessage<IPersonInfo> result)
        {
            if (result.IsSuccess)
            {
                _logger.WriteInfoLog(result.Data.ToString());
                _parameterPass.AddOrUpdateStepData(CardDataKey, result.Data);
                return true;
            }
            _logger.WriteInfoLog(result.Message);
            return false;
        }
        public RelayCommand CloseCommand { get; }

        private void ExcuteCloseCommand()
        {
            Application.Current.Shutdown();
        }
    }
}