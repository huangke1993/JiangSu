using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UserControls.Input.KeyBoard
{
    /// <summary>
    /// StyleButton.xaml 的交互逻辑
    /// </summary>
    public partial class StyleButton : UserControl
    {
        Brush normalLeftBrush = Brushes.Red;
        public StyleButton()
        {
            InitializeComponent();
            normalLeftBrush = style1.Foreground;
        }
        static DependencyProperty Text1Property = DependencyProperty.Register("Text1", typeof(string), typeof(StyleButton), new PropertyMetadata("A"));
        public string Text1 { get { return (string)GetValue(Text1Property); } set { SetValue(Text1Property, value); } }

        static DependencyProperty Text2Property = DependencyProperty.Register("Text2", typeof(string), typeof(StyleButton), new PropertyMetadata("A"));
        public string Text2 { get { return (string)GetValue(Text2Property); } set { SetValue(Text2Property, value); } }



        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Brush b1 = style1.Foreground;
            Brush b2 = style2.Foreground;
            style2.Foreground = b1;
            style1.Foreground = b2;

            FontWeight fw1 = style1.FontWeight;
            FontWeight fw2 = style2.FontWeight;
            style2.FontWeight = fw1;
            style1.FontWeight = fw2;

            e.Handled = false;
        }

        public bool IsLeft()
        {
            return style1.Foreground == normalLeftBrush;
        }
    }
}
