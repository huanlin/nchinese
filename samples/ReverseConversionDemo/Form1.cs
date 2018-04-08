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
            using (MsImeFacade ime = new MsImeFacade(ImeClass.China))
            {
                // Show conversion mode capabilities.
                OutputTextBox.Text = $"ConversionModeCaps: {ime.ConversionModeCaps}";

                string[] result = ime.GetPinyin(InputTextBox.Text);

                OutputTextBox.Text += Environment.NewLine + result;
            }
        }
    }
}
