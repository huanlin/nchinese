namespace ReverseConversion
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            InputTextBox = new TextBox();
            GoButton = new Button();
            label1 = new Label();
            label2 = new Label();
            ZhuyinFromBuiltInDict_TextBox = new TextBox();
            SuspendLayout();
            // 
            // InputTextBox
            // 
            InputTextBox.Location = new Point(136, 26);
            InputTextBox.Margin = new Padding(3, 5, 3, 5);
            InputTextBox.Name = "InputTextBox";
            InputTextBox.Size = new Size(476, 27);
            InputTextBox.TabIndex = 0;
            InputTextBox.Text = "便宜又方便得不得了";
            // 
            // GoButton
            // 
            GoButton.Location = new Point(650, 20);
            GoButton.Margin = new Padding(3, 5, 3, 5);
            GoButton.Name = "GoButton";
            GoButton.Size = new Size(105, 37);
            GoButton.TabIndex = 1;
            GoButton.Text = "查字根";
            GoButton.UseVisualStyleBackColor = true;
            GoButton.Click += GoButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 29);
            label1.Name = "label1";
            label1.Size = new Size(114, 19);
            label1.TabIndex = 3;
            label1.Text = "輸入一些中文：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 95);
            label2.Name = "label2";
            label2.Size = new Size(84, 19);
            label2.TabIndex = 4;
            label2.Text = "注音字根：";
            // 
            // ZhuyinFromBuiltInDict_TextBox
            // 
            ZhuyinFromBuiltInDict_TextBox.Location = new Point(136, 92);
            ZhuyinFromBuiltInDict_TextBox.Name = "ZhuyinFromBuiltInDict_TextBox";
            ZhuyinFromBuiltInDict_TextBox.Size = new Size(826, 27);
            ZhuyinFromBuiltInDict_TextBox.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1041, 169);
            Controls.Add(ZhuyinFromBuiltInDict_TextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(GoButton);
            Controls.Add(InputTextBox);
            Font = new Font("Microsoft JhengHei", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(3, 5, 3, 5);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "反查注音／拼音字根";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputTextBox;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ZhuyinFromBuiltInDict_TextBox;
    }
}

