using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Ink;

namespace UserControls.Input.HandInput
{
    /// <summary>
    /// HandWrite.xaml 的交互逻辑
    /// </summary>
    public partial class HandWrite
    {
        public HandWrite()
        {
            InitializeComponent();
        }

        private RecognizerContext _rct;

        private void InkInput_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            if (inkInput.Strokes.Count == 0)
                return;
            using (var ms = new MemoryStream())
            {
                inkInput.Strokes.Save(ms);
                var ink = new Ink();
                ink.Load(ms.ToArray());
                _rct.StopBackgroundRecognition();
                _rct.Strokes = ink.Strokes;
                _rct.BackgroundRecognizeWithAlternates(0);
            }
            
        }

        private void Ink_Here()
        {
            Recognizers recos = new Recognizers();
            Recognizer chineseReco = recos.GetDefaultRecognizer();

            _rct = chineseReco.CreateRecognizerContext();
            _rct.RecognitionFlags = RecognitionModes.WordMode;

           
        }
        private void Rct_RecognitionWithAlternates(object sender, RecognizerContextRecognitionWithAlternatesEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Send, new RecognizerContextRecognitionWithAlternatesEventHandler(Rct_RecognitionWithAlternates), sender, e);
                return;
            }
            if (RecognitionStatus.NoError == e.RecognitionStatus)
            {
                panelChoose.Children.Clear();
                var allAlternates = e.Result.GetAlternatesFromSelection();
                var i = 1;
                var width = 56;
                var height = 56;
                var isFirst = true;
                foreach (var oneAlternate in allAlternates)
                {
                    var lbl = new Label {Name = "lbl" + i, Tag = oneAlternate.ToString()};
                    lbl.Content = lbl.Tag.ToString();
                    lbl.Width = width;
                    lbl.Height = height;
                    lbl.FontSize = 24F;
                    if (isFirst)
                    {
                        lbl.Width = 172;
                        lbl.Height = 172;
                        lbl.FontSize = 35F;
                        isFirst = false;
                    }
                    lbl.FontFamily = new System.Windows.Media.FontFamily("微软雅黑");
                    lbl.BorderThickness = new Thickness(1);
                    lbl.Margin = new Thickness(1, 1, 1, 1);
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                    lbl.VerticalContentAlignment = VerticalAlignment.Center;
                    lbl.BorderBrush = border.BorderBrush;// System.Windows.Media.Brushes.Gray;
                    lbl.MouseLeftButtonDown += Lbl_MouseLeftButtonDown;
                    panelChoose.Children.Add(lbl);
                    ++i;
                }
            }
        }

        private void Lbl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Label label) InputCommon.Send(label.Content.ToString());
            Clear_PreviewMouseDown(null, null);
        }
        private void Clear_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            inkInput.Strokes.Clear();
            panelChoose.Children.Clear();
        }

        private void Cancel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (inkInput.Strokes.Count == 0)
                return;
            inkInput.Strokes.RemoveAt(inkInput.Strokes.Count - 1);
            InkInput_StrokeCollected(null, null);
            if (inkInput.Strokes.Count == 0)
                panelChoose.Children.Clear();
        }

        private void Delete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            inkInput.Strokes.Clear();
            panelChoose.Children.Clear();
            InputCommon.Send("{BACKSPACE}");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Ink_Here();
        }

        private void ClearHandWrite()
        {
            _rct.RecognitionWithAlternates -= Rct_RecognitionWithAlternates;
        }

        private void InitHandWrite()
        {
            _rct.RecognitionWithAlternates += Rct_RecognitionWithAlternates;
        }

        private void HandWrite_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitHandWrite();
                return;
            }
            ClearHandWrite();
        }
    }
}
