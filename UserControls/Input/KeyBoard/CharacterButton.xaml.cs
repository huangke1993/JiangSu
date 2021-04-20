using System.Windows;
using System.Windows.Media;

namespace UserControls.Input.KeyBoard
{
    /// <summary>
    /// CharacterButton.xaml 的交互逻辑
    /// </summary>
    public partial class CharacterButton
    {
        public CharacterButton()
        {
            InitializeComponent();
        }

        static readonly DependencyProperty Text1Property = DependencyProperty.Register("Text1", typeof(string), typeof(CharacterButton), new PropertyMetadata("A"));
        public string Text1 { get => (string)GetValue(Text1Property);
            set => SetValue(Text1Property, value);
        }

        static readonly DependencyProperty Text2Property = DependencyProperty.Register("Text2", typeof(string), typeof(CharacterButton), new PropertyMetadata("A"));
        public string Text2 { get => (string)GetValue(Text2Property);
            set => SetValue(Text2Property, value);
        }

        static readonly DependencyProperty IsNumProperty = DependencyProperty.Register("IsNum", typeof(bool), typeof(CharacterButton), new PropertyMetadata(true));
        public bool IsNum { get => (bool)GetValue(IsNumProperty);
            set
            {
                if(value != IsNum)
                {
                    Brush b1 = num.Foreground;
                    Brush b2 = character.Foreground;
                    character.Foreground = b1;
                    num.Foreground = b2;
                    SetValue(IsNumProperty, value);
                }
            }
        }
    }
}
