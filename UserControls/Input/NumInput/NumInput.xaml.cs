using System.Windows;

namespace UserControls.Input.NumInput
{
    /// <summary>
    /// NumInput.xaml 的交互逻辑
    /// </summary>
    public partial class NumInput
    {
        public NumInput()
        {
            InitializeComponent();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            InputCommon.Send("{BACKSPACE}");
        }
        private void NumBtn_Click(object sender, RoutedEventArgs e)
        {
            var target = (System.Windows.Controls.Button)e.Source;
            if (target.Content == null) return;
            InputCommon.Send(target.Content.ToString());
        }
    }
}
