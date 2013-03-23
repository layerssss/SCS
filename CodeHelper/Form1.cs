using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            char c1 = char.Parse(textBox5.Text);
            for (char c = Char.Parse(textBox2.Text); c <= Char.Parse(textBox3.Text); c++)
            {
                textBox4.Text += textBox1.Text.Replace("{0}", c.ToString()).Replace("{1}",c1.ToString());
                c1++;
            }
        }
    }
}
