namespace UserControls.Input
{
    internal class InputCommon
    {
        public static void Send(string txt)
        {
            try
            {
                if (txt[0] == '{' && txt[txt.Length - 1] == '}')
                    System.Windows.Forms.SendKeys.SendWait(txt);
                else
                {
                    System.Windows.Clipboard.SetDataObject(txt);
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
