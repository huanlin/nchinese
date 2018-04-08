using System;
using System.Windows.Forms;
using NChinese.Imm;

namespace ReverseConversionDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            OutputTextBox.Clear();
            using (var pinyinProvider = new ImmPinyinConversionProvider())
            {
                string[] result = pinyinProvider.Convert(InputTextBox.Text);

                foreach (var s in result)
                {
                    OutputTextBox.Text += Environment.NewLine + s;
                }                
            }
        }
    }
}
