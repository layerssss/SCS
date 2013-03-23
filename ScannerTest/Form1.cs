using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SCS;
namespace ScannerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SCS.Scanner scanner;
        private void Form1_Load(object sender, EventArgs e)
        {
            scanner = new Scanner(new string[] { "class" });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scanner.InitContext(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Token t = scanner.Scan();
            MessageBox.Show(t.Type.ToString()+"\""+t.Data+"\"");
        }
    }
}
