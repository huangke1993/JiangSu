using System.Collections.Generic;
using System.Windows;

namespace UserControls.Input
{
    /// <summary>
    /// InputTypeSelect.xaml 的交互逻辑
    /// </summary>
    public partial class InputTypeSelect
    {
        public InputTypeSelect()
        {
            InitializeComponent();
            Init();
            SelectType(InputType);
        }
        public static readonly DependencyProperty InputTypeProperty;
        public InputType InputType
        {
            get => (InputType)GetValue(InputTypeProperty);
            set => SetValue(InputTypeProperty, value);
        }

        static InputTypeSelect()
        {
            InputTypeProperty = DependencyProperty.Register("InputType", typeof(InputType), typeof(InputTypeSelect),
                new PropertyMetadata(InputType.KeyBoard,InputTypeChangedCallback));
        }

        public static void InputTypeChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var target = d as InputTypeSelect;
            target?.SelectType((InputType) e.NewValue);
        }

        private void KeyBoard_OnClick(object sender, RoutedEventArgs e)
        {
            SelectType(InputType.KeyBoard);
        }

        private void HandWrite_OnClick(object sender, RoutedEventArgs e)
        {
            SelectType(InputType.HandWrite);
        }

        private void NumInput_OnClick(object sender, RoutedEventArgs e)
        {
            SelectType(InputType.NumInput);
        }

        private Dictionary<InputType, FrameworkElement> _typeToElements;

        private void Init()
        {
            _typeToElements = new Dictionary<InputType, FrameworkElement>()
            {
                {InputType.KeyBoard,Keyboard },
                {InputType.HandWrite,HandWrite },
                {InputType.NumInput,NumInput }
            };
        }

        internal void SelectType(InputType type)
        {
            foreach (var keyValue in _typeToElements)
            {
                keyValue.Value.Visibility = keyValue.Key == type ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
