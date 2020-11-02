using System.Threading;
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
        }

        public RelayCommand UnLoadCommand { get; }

        private void ExcuteUnLoadCommand()
        {
            _timer.Dispose();
        }

        private ChromiumWebBrowser _browser;
        public RelayCommand<ChromiumWebBrowser> LoadCommand { get; }

        private void ExcuteLoadCommand(ChromiumWebBrowser browser)
        {
            InitBrowser(browser);
            ShowHtmlPage();
            AddObserver();
            EveryFiveSecondToReadCard();
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
        }

        private string _address;

        public string Address
        {
            get => _address;
            set { _address = value;RaisePropertyChanged(); }
        }

        private void ShowHtmlPage()
        {
            Address = _fileConfiguration.GetConfig<Configs>().PadUrl;
        }

        private Timer _timer;
        private void EveryFiveSecondToReadCard()
        {
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
    }
}