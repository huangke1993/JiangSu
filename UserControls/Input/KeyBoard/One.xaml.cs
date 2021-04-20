using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserControls.Input.KeyBoard
{
    /// <summary>
    /// One.xaml 的交互逻辑
    /// </summary>
    public partial class One : UserControl
    {
        public One()
        {
            InitializeComponent();
            
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public static DependencyProperty ZProperty = DependencyProperty.Register("Z", typeof(string), typeof(One), new PropertyMetadata("啊"));
        public string Z
        {
            get { return (string)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }

        public static DependencyProperty NumProperty = DependencyProperty.Register("Num", typeof(string), typeof(One), new PropertyMetadata("8"));
        public string Num
        {
            get { return (string)GetValue(NumProperty); }
            set { SetValue(NumProperty, value); }
        }
    }
}
