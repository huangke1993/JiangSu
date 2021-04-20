using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ParameterDeclare;

namespace JiangSuPad.ViewModel
{
    public class InputWinViewModel : ViewModelBase
    {
        private readonly IParameterPass _pamPass;
        public InputWinViewModel(IParameterPass pamPass)
        {
            _pamPass = pamPass;
            SureCommand=new RelayCommand(ExcuteSureCommand);
            LoadCommand=new RelayCommand<System.Windows.Window>(ExcuteLoadCommand);
        }
        private const int CardDataKey = 10;
        public RelayCommand SureCommand { get; }

        private void ExcuteSureCommand()
        {
            _pamPass.AddOrUpdateStepData(CardDataKey, $"{Name}|{IdNum}");
            _win.Close();
        }

        private System.Windows.Window _win;
        public RelayCommand<System.Windows.Window> LoadCommand { get; }

        private void ExcuteLoadCommand(System.Windows.Window win)
        {
            _win = win;
        }
        public string Name { get; set; }
        public string IdNum { get; set; }
    }
}
