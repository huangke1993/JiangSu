using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace UserControls.TextBox
{
    public class TextBoxNoAutoKeyboard : System.Windows.Controls.TextBox
    {
        public TextBoxNoAutoKeyboard()
        {
            TextChanged += TextBoxNoAutoKeyboard_TextChanged;
        }

        private void TextBoxNoAutoKeyboard_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox) textBox.SelectionStart = textBox.Text.Length;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }

    }
}