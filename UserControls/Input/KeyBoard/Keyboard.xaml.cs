using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using tools;

namespace UserControls.Input.KeyBoard
{
    /// <summary>
    /// Keyboard.xaml 的交互逻辑
    /// </summary>
    public partial class Keyboard
    {
        List<CharacterButton> cbs = new List<CharacterButton>();
        List<One> selectors = new List<One>();
        List<KeyButton> kbs = new List<KeyButton>();
        PageHandle<char> page = null;
        bool isChinese = true;
        bool isUp = true;
        bool isNum = true;
        public Keyboard()
        {
            InitializeComponent();
            int index = 0;
            foreach (UIElement ui in sp1.Children)
            {
                if (sp1.Children[index].GetType().ToString().IndexOf("One") == -1)
                    continue;
                One o = ui as One;
                selectors.Add(o);
                o.Visibility = Visibility.Hidden;
            }

            foreach (UIElement ui in wp.Children)
            {
                System.Windows.Controls.UserControl uc = ui as System.Windows.Controls.UserControl;
                string tag = uc.Tag.ToString();
                if (tag == "Key")
                {
                    KeyButton cb = uc as KeyButton;
                    if (cb.Text.Length == 1)
                        kbs.Add(uc as KeyButton);
                }
                else if (tag == "Character")
                {
                    cbs.Add(uc as CharacterButton);
                }
            }
        }

        private void WrapPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.UserControl source = e.Source as System.Windows.Controls.UserControl;
            string tag = source.Tag.ToString();
            if (tag == "StyleButton")
            {
                StyleButton style = source as StyleButton;
                string txt = style.Text1;
                bool isLeft = style.IsLeft();
                if (txt == "中文")
                {
                    ClearPinYin();
                    isChinese = !isChinese;
                }
                if (txt == "大写")
                {
                    foreach (KeyButton kb in kbs)
                    {
                        if (isUp)
                            kb.Text = kb.Text.ToLower();
                        else
                            kb.Text = kb.Text.ToUpper();
                    }
                    isUp = !isUp;
                }
                if (txt == "数字")
                {
                    foreach (CharacterButton cb in cbs)
                    {
                        cb.IsNum = !cb.IsNum;
                    }
                    isNum = !isNum;
                }
            }

            if (source.Tag.ToString() == "Key")
            {
                KeyButton kb = source as KeyButton;
                string text = kb.Text;
                //输入英文
                if (text.Length == 1 && isChinese == false)
                {
                    InputCommon.Send(text);
                    return;
                }
                text = text.ToLower();
                //打拼音
                if (text.Length == 1 && isChinese && pinyin.Text.Length < 6)
                {
                    List<char> cs = null;
                    pinyin.Text += text;
                    cs = ZPoint.GetValues(pinyin.Text);
                    page = new PageHandle<char>(cs.ToArray(), 10);
                    char[] ca = page.getFirstPage(out _);
                    SetSelectors(ca);
                }
                //删除拼音
                else if (text == "删除" && pinyin.Text.Length > 0)
                {
                    List<char> cs = null;
                    pinyin.Text = pinyin.Text.Substring(0, pinyin.Text.Length - 1);
                    cs = ZPoint.GetValues(pinyin.Text);
                    page = new PageHandle<char>(cs.ToArray(), 10);
                    char[] ca = page.getFirstPage(out _);
                    SetSelectors(ca);
                }
                //删除字
                else if (text == "删除")
                {
                    InputCommon.Send("{BACKSPACE}");
                }
                else if (text == "enter")
                {
                    if (pinyin.Text.Length != 0)
                    {
                        InputCommon.Send(pinyin.Text);
                        ClearPinYin();
                    }
                    else
                        InputCommon.Send("{ENTER}");
                }
                else if (text == "空格")
                {
                    if (pinyin.Text.Length != 0)
                    {
                        if (selectors[0].Visibility == Visibility.Visible)
                        {
                            InputCommon.Send(selectors[0].Z);
                        }
                        ClearPinYin();
                    }
                    else
                        InputCommon.Send(" ");
                }
            }
            else if (source.Tag.ToString() == "Character")
            {
                CharacterButton cb = source as CharacterButton;
                string t1 = cb.Text1;
                string t2 = cb.Text2;
                //选择拼音结果
                if (pinyin.Text != "" && selectors[0].Visibility == Visibility.Visible)
                {
                    if (t1[0] >= '0' && t1[0] <= '9')
                    {
                        int index = int.Parse(t1);
                        foreach (One o in selectors)
                            if (o.Num == t1 && o.Visibility == Visibility.Visible)
                            {
                                InputCommon.Send(o.Z);
                                ClearPinYin();
                            }
                    }
                    else if (t1 == "<" && page != null)
                    {
                        char[] ca = page.getLastPage(out _);
                        SetSelectors(ca);
                    }
                    else if (t1 == ">" && page != null)
                    {
                        char[] ca = page.getNextPage(out _);
                        SetSelectors(ca);
                    }
                }
                else
                {
                    if (pinyin.Text != "")
                        pinyin.Text = "";
                    if (cb.IsNum)
                        InputCommon.Send(t1);
                    else
                        InputCommon.Send(t2);
                }
            }

        }
        private void ClearPinYin()
        {
            pinyin.Text = "";
            for (int i = 0; i < selectors.Count; i++)
                selectors[i].Visibility = Visibility.Hidden;
            page = null;
        }
        private void SetSelectors(char[] ca)
        {
            for (int i = 0; i < ca.Length; i++)
            {
                selectors[i].Z = ca[i].ToString();
                selectors[i].Visibility = Visibility.Visible;
            }
            for (int i = ca.Length; i < selectors.Count; i++)
                selectors[i].Visibility = Visibility.Hidden;
        }
        private void Send2(string txt)
        {
            try
            {
                if (txt[0] == '{' && txt[txt.Length - 1] == '}')
                    System.Windows.Forms.SendKeys.SendWait(txt);
                else
                {
                    System.Windows.Clipboard.SetText(txt);
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }
            }
            catch { }
        }


        ConcurrentQueue<string> cmds = new ConcurrentQueue<string>();
        public void ThreadHandle()
        {
            int maxTry = 1000;
            for (int i = 0; i < maxTry; i++)
            {
                if (cmds.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                bool isSuccess = cmds.TryDequeue(out var cmd);
                if (isSuccess)
                {
                    this.Dispatcher.Invoke(new Action(() => {
                        Send2(cmd);
                    }));
                }
                i--;
            }
        }


        private void Sp1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string type = e.Source.GetType().ToString();
            if (type.IndexOf("KeyBoard.One") == -1)
                return;
            One o = e.Source as One;
            InputCommon.Send(o.Z);
            ClearPinYin();
        }

        private void LastPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (page == null)
            {
                return;
            }
            char[] ca = page.getLastPage(out _);
            SetSelectors(ca);
        }

        private void NextPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (page == null)
            {
                return;
            }
            char[] ca = page.getNextPage(out _);
            SetSelectors(ca);
        }

    }
}
