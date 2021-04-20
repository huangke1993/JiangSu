using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserControls.Input.KeyBoard
{
    /// <summary>
    /// KeyButton.xaml 的交互逻辑
    /// </summary>
    public partial class KeyButton : UserControl
    {
        public KeyButton()
        {
            InitializeComponent();
        }

        static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(KeyButton), new PropertyMetadata("A"));
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }

        public new double FontSize { get; set; }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
        }
    }
}
