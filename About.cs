using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetDasm
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            pictureBox1.Image = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ideatic.net/");
        }
    }
}
