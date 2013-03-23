using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GfcTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = System.IO.File.ReadAllText("gfc.txt");
                textBox2.Text = System.IO.File.ReadAllText("keywords.txt");
                textBox3.Text = System.IO.File.ReadAllText("accept.txt");
                textBox5.Text = System.IO.File.ReadAllText("sample.txt");
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText( "gfc.txt",textBox1.Text);
            System.IO.File.WriteAllText( "keywords.txt",textBox2.Text);
            System.IO.File.WriteAllText("accept.txt", textBox3.Text);
            System.IO.File.WriteAllText("sample.txt", textBox5.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] ss = System.IO.File.ReadAllLines("gfc.txt");
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].StartsWith("=>"))
                {
                    ss[i] = i.ToString().PadRight(3) + ss[i];
                }
            }
            textBox4.Text = "";
            SCS.Gfc.LL1Gfc gfc = new SCS.Gfc.LL1Gfc(
                System.IO.File.ReadAllLines("gfc.txt"),
                System.IO.File.ReadAllLines("keywords.txt"),
                System.IO.File.ReadAllText("accept.txt"));
            foreach (SCS.Gfc.GfcProduction production in gfc.Productions)
            {
                textBox4.Text += production.Name.PadRight(10);
                textBox4.Text += gfc.First[production].ContainEpsilon ? "Nullable:" : "        :";
                foreach (SCS.Token token in gfc.First[production])
                {
                    textBox4.Text += token.ToString() + ":";
                }
                textBox4.Text += "\r\n";
            }
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SCS.Parser parser=new SCS.Parser(new SCS.Gfc.LL1Gfc(
                System.IO.File.ReadAllLines("gfc.txt"),
                System.IO.File.ReadAllLines("keywords.txt"),
                System.IO.File.ReadAllText("accept.txt")
                ));
            parser.InitContext(textBox5.Text);
            parser.Parse();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText("gfc.txt", textBox1.Text);
            string[] ss = System.IO.File.ReadAllLines("gfc.txt");
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].StartsWith("=>"))
                {
                    ss[i] = i.ToString().PadRight(3) + ss[i];
                }
            }
            System.IO.File.WriteAllLines("gfc.txt", ss);
            textBox1.Text = System.IO.File.ReadAllText("gfc.txt");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            SCS.Gfc.LL1Gfc gfc = (new SCS.Gfc.Gramma.Gramma()).GetData();
            foreach (SCS.Gfc.GfcProduction production in gfc.Productions)
            {
                textBox4.Text += production.Name.PadRight(10);
                textBox4.Text += gfc.First[production].ContainEpsilon ? "Nullable:" : "        :";
                foreach (SCS.Token token in gfc.First[production])
                {
                    textBox4.Text += token.ToString() + ":";
                }
                textBox4.Text += "\r\n";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SCS.Parser p = new SCS.Parser((new SCS.Gfc.Gramma.Gramma()).GetData());
            p.InitContext(textBox5.Text);
            p.Parse();
        }
    }
}
