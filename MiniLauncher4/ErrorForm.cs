using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiniLauncher4
{
    public partial class ErrorForm : Form
    {
        TextBox textBox1 = null;
        TextBox textBox2 = null;
        public ErrorForm()
        {
            InitializeComponent();

            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            textBox1 = new TextBox();
            textBox1.Height = 80;
            textBox1.Multiline = true;
            textBox1.Dock = DockStyle.Top;

            textBox2 = new TextBox();
            textBox2.Height = 300;
            textBox2.Multiline = true;
            textBox2.Dock = DockStyle.Bottom;

            this.Controls.Add(textBox1);
            this.Controls.Add(textBox2);
        }


        public void ShowMessage(LaunchInfo info, Exception ex)
        {
            this.Text = info.Name + " -- error";

            this.textBox1.Text = info.ExecutePath;

            this.textBox2.Text = ex.ToString();

            this.ShowDialog();
        }
    }
}
