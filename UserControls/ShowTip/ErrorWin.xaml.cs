using System.ComponentModel;
using System.Windows;
using UserControls.Common;

namespace UserControls.ShowTip
{
    /// <summary>
    /// ErrorWin.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorWin
    {
        public ErrorWin()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty TextTipProperty;
        [Bindable(true)]
        [Category("Appearance")]
        public string TextTip
        {
            get => (string)GetValue(TextTipProperty);
            set => SetValue(TextTipProperty, value);
        }
        static ErrorWin()
        {
            TextTipProperty = DependencyProperty.Register("TextTip", typeof(string), typeof(ErrorWin), new PropertyMetadata(string.Empty));
            SureEvent = EventManager.RegisterRoutedEvent("Sure", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ErrorWin));
        }
        public static RoutedEvent SureEvent;
        public event RoutedEventHandler SureClick
        {
            add => AddHandler(SureEvent, value);
            remove => RemoveHandler(SureEvent, value);
        }
        private void ButtonSure_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SureEvent, this));
            HideAndResetCountDown();
        }
        private void HideAndResetCountDown()
        {
            if (!(Parent is FrameworkElementAdorner frameworkElementAdorner)) return;
            if (frameworkElementAdorner.AdornedElement is AdornedControl adornedControl)
            {
                adornedControl.IsAdornerVisible = false;
            }
        }
       
    }
}
