using System;
using System.Text;
using System.Windows.Forms;
using NChinese;
using NChinese.Imm;
using NChinese.Phonetic;

namespace ReverseConversionDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private string ReverseConversion(string input, IReverseConversionProvider converter)
        {
            string[] result = converter.Convert(input);

            var sb = new StringBuilder();
            foreach (string s in result)
            {
                sb.Append(s + " ");
            }
            return sb.ToString();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            ZhuyinFromBuiltInDict_TextBox.Text = ReverseConversion(
                InputTextBox.Text,
                new ZhuyinReverseConversionProvider());

            PinyinFromImmApi_TextBox.Text = ReverseConversion(
                InputTextBox.Text,
                new ImmPinyinConversionProvider());

            ZhuyinFromPinyin_TextBox.Text = ReverseConversion(
                InputTextBox.Text,
                new ImmZhuyinConversionProvider());
        }
    }
}
