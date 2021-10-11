using System;
using System.Windows.Forms;

namespace Programma
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        Settings IniFile = new Settings(Application.StartupPath + "\\settings.ini");
        private void button3_Click(object sender, EventArgs e)
        {
            IniFile.IniWriteValue("send", "login", textBox10.Text);
            IniFile.IniWriteValue("send", "password", textBox11.Text);
            IniFile.IniWriteValue("send", "server", textBox4.Text);
            IniFile.IniWriteValue("send", "port", textBox5.Text);
            IniFile.IniWriteValue("send", "address", textBox10.Text);
            IniFile.IniWriteValue("send", "ssl", checkBox2.Checked ? "1" : "0");

            IniFile.IniWriteValue("recive", "login", textBox10.Text);
            IniFile.IniWriteValue("recive", "password", textBox11.Text);
            IniFile.IniWriteValue("recive", "server", textBox8.Text);
            IniFile.IniWriteValue("recive", "port", textBox9.Text);
            IniFile.IniWriteValue("recive", "ssl", checkBox2.Checked ? "1" : "0");
            IniFile.IniWriteValue("recive", "delmessage", checkBox3.Checked ? "1" : "0");

            IniFile.IniWriteValue("recive", "imapserver", textBox14.Text);
            IniFile.IniWriteValue("recive", "imapport", textBox15.Text);
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            textBox4.Text = IniFile.IniReadValue("send", "server");
            textBox5.Text = IniFile.IniReadValue("send", "port");

            textBox8.Text = IniFile.IniReadValue("recive", "server");
            textBox9.Text = IniFile.IniReadValue("recive", "port");
            textBox10.Text = IniFile.IniReadValue("recive", "login");
            textBox11.Text = IniFile.IniReadValue("recive", "password");
            checkBox2.Checked = IniFile.IniReadValue("recive", "ssl").Contains("1");
            checkBox3.Checked = IniFile.IniReadValue("recive", "delmessage").Contains("1");

            textBox14.Text = IniFile.IniReadValue("recive", "imapserver");
            textBox15.Text = IniFile.IniReadValue("recive", "imapport");
        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            textBox6.Text = textBox10.Text;
            textBox7.Text = textBox10.Text;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            textBox6.Text = textBox7.Text;
            textBox10.Text = textBox7.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            textBox10.Text = textBox6.Text;
            textBox7.Text = textBox6.Text;
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            textBox16.Text = textBox11.Text;
            textBox17.Text = textBox11.Text;
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            textBox11.Text = textBox16.Text;
            textBox17.Text = textBox16.Text;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            textBox16.Text = textBox17.Text;
            textBox11.Text = textBox17.Text;
        }
    }
}
