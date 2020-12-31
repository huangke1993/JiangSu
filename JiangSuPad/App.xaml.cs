using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Wpf;
using CommonServiceLocator;
using JiangSuPad.Window;
using LoggerDeclare;
using MessageBox = System.Windows.MessageBox;

namespace JiangSuPad
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            SetCefSharp();
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                var assemblyName = args.Name.Split(new[] {','}, 2)[0] + ".dll";
                var archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? "x64" : "x86",
                    assemblyName);

                return File.Exists(archSpecificPath)
                    ? Assembly.LoadFile(archSpecificPath)
                    : null;
            }

            return null;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SetCefSharp()
        {
            var settings = new CefSettings
            {
                Locale = "zh-CN",
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? "x64" : "x86",
                    "CefSharp.BrowserSubprocess.exe")
            };
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }

        private Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsOnlyOneProcess())
                OnlyOneProcessAction(e);
            else
                NotOnlyOneProcessAction();
        }

        private void OnlyOneProcessAction(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        private const int MainWinScreenIndex = 0;

        private void DisplayMainWin()
        {
            DisplayWin(new PadWin(), MainWinScreenIndex);
        }
        private const int TvWinScreenIndex = 1;
        private void DisPlayTvWin()
        {
            DisplayWin(new TvWin(), TvWinScreenIndex);
        }

        private void DisplayWin(System.Windows.Window win,int screenIndex)
        {
            if (Screen.AllScreens.ElementAtOrDefault(screenIndex) == null) return;
            var target = Screen.AllScreens[screenIndex];
            var targetWorkingArea = target.Bounds;
            win.Top = targetWorkingArea.Top;
            win.Left = targetWorkingArea.Left;
            win.Width = targetWorkingArea.Width;
            win.Height = targetWorkingArea.Height;
            win.Show();
        }

        private void NotOnlyOneProcessAction()
        {
            Shutdown();
        }

        private bool IsOnlyOneProcess()
        {
            _mutex = new Mutex(true, "JiangSuPad");
            return _mutex.WaitOne(0, false);
        }

        private ILogger _logger;

        private void InitGloableException()
        {
            _logger = ServiceLocator.Current.GetInstance<ILogger>();
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleGlobleException(e.Exception);
        }

        private void Current_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleGlobleException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleGlobleException((Exception)e.ExceptionObject);
        }

        private void HandleGlobleException(Exception exception)
        {
            _logger.WriteInfoLog(exception.Message);
            _logger.WriteInfoLog(exception.StackTrace);
            MessageBox.Show("程序遇到无法处理的错误，请联系管理员！");
            MainWindow?.Close();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            DisplayMainWin();
            DisPlayTvWin();
        }

        protected override void OnActivated(EventArgs e)
        {
            InitGloableException();
            base.OnActivated(e);
        }
    }
}
