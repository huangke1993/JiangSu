using System.Windows;

namespace UserControls.Button
{
    /// <summary>
    /// LevButton.xaml 的交互逻辑
    /// </summary>
    public partial class LevButton
    {
        public LevButton()
        {
            InitializeComponent();
        }

        static LevButton()
        {
            ClickEvent = EventManager.RegisterRoutedEvent("LevButtonClick", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(LevButton));
        }

        public static RoutedEvent ClickEvent;
        public event RoutedEventHandler ClickClick
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }
    }
}
