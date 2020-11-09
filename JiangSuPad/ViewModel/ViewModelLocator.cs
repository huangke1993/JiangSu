using CommonServiceLocator;
using DefaultLogger;
using DefaultParameterPass;
using DkCardReaderImpl;
using FileConfigurationInterface;
using HsCardReaderImpl;
using ICardReaderDeclare;
using JiangSuPad.Config.ConfigModel;
using JsonFileConfiguration;
using LoggerDeclare;
using ParameterDeclare;
using TsCardReaderImpl;
using Unity;
using Unity.ServiceLocation;

namespace JiangSuPad.ViewModel
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
            var container = new UnityContainer();
            var fileConfig= InitConfig(container);
            RegisterAssembly(container, fileConfig);
            RegisterViewModel(container);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }
        private static IFileConfiguration InitConfig(IUnityContainer container)
        {
            var builder = new JsonFileConfigurationBuilder();
            var fileConfig=builder.AddConfigurationFile<Configs>("Config\\ConfigFile\\Configs.json").Build();
            container.RegisterInstance(fileConfig);
            return fileConfig;
        }
        private static void RegisterViewModel(IUnityContainer container)
        {
            container.RegisterType<PadWinViewModel>();
            container.RegisterType<TvWinViewModel>();
        }

        private static void RegisterAssembly(IUnityContainer container, IFileConfiguration fileConfiguration)
        {
            container.RegisterType<ILogger, DefaultLoggerImp>();
            container.RegisterSingleton(typeof(IParameterPass), typeof(ParameterPassImpl));
            RegisterDevice(container, fileConfiguration);
        }

        private static void RegisterDevice(IUnityContainer container, IFileConfiguration fileConfiguration)
        {
            var deviceType = fileConfiguration.GetConfig<Configs>().DeviceType;
            if (deviceType == DeviceType.Dk)
            {
                container.RegisterType<ICardReader, DkCardReader>();
                return;
            }
            if (deviceType == DeviceType.Ts)
            {
                container.RegisterType<ICardReader, TsCardReader>();
                return;
            }
            if (deviceType == DeviceType.Hs)
            {
                container.RegisterType<ICardReader, HsCardReader>();
                return;
            }
        }

        public PadWinViewModel PadWin=> ServiceLocator.Current.GetInstance<PadWinViewModel>();
        public TvWinViewModel TvWin => ServiceLocator.Current.GetInstance<TvWinViewModel>();
        
        public static void Cleanup()
        {
        }
    }
}