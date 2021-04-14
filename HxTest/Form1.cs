using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HxCardReaderImpl;

namespace HxTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly HxCardReader _hxCardReader=new HxCardReader();
        private async void button1_Click(object sender, EventArgs e)
        {
            var result=await _hxCardReader.ReadIdCardAsync();
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Message);
                return;
            }

            MessageBox.Show(result.Data.Name);
            MessageBox.Show(result.Data.IdNum);
        }
    }
}
