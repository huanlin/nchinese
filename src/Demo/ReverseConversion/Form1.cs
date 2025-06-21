using System;
using System.Text;
using System.Windows.Forms;
using NChinese;
using NChinese.Phonetic;

namespace ReverseConversion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private string ReverseConversion(string input)
        {
            var converter = new ZhuyinReverseConverter();
            string[] result = converter.GetZhuyin(input);

            var sb = new StringBuilder();
            foreach (string s in result)
            {
                sb.Append(s + " ");
            }
            return sb.ToString();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            ZhuyinFromBuiltInDict_TextBox.Text = ReverseConversion(InputTextBox.Text);
        }
    }
}
