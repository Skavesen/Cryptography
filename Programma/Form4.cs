using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;

namespace Programma
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }
        Settings IniFile = new Settings(Application.StartupPath + "\\settings.ini");
        /// <summary>
        /// Приатачивание файлов
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы для отправки",
                InitialDirectory = Application.StartupPath
            };
            dlg.ShowDialog();
            // пользователь вышел из диалога ничего не выбрав
            if (dlg.FileName == String.Empty)
                return;
            foreach (string file in dlg.FileNames)
            {
                File.Copy(file, Application.StartupPath + "\\send\\" + @"\" + Path.GetFileName(file));
            }
        }
        /// <summary>
        /// Метод отправки сообщения
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            SendEmail(textBox3.Text, textBox12.Text, textBox13.Text, richTextBox2.Text);
        }
        private void SendEmail(
      string from,
      string to,
      string subject,
      string body)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(from);
            message.To.Add(to);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            message.Body = body;
            Directory.GetFiles("send", "*.*").ToList().ForEach(name => message.Attachments.Add(new Attachment(name, MediaTypeNames.Text.Plain)));
            int port;
            try
            {
                port = (int)Convert.ToInt16(IniFile.IniReadValue("send", "port"));
            }
            catch (Exception)
            {
                port = 25;
            }
            SmtpClient smtpClient = new SmtpClient(IniFile.IniReadValue("send", "server"), port);
            smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential(IniFile.IniReadValue("send", "login"), IniFile.IniReadValue("send", "password"));
            smtpClient.EnableSsl = IniFile.IniReadValue("send", "ssl").Contains("1");
            button6.Text = "Ожидание...";
            button6.Enabled = false;
            try
            {
                smtpClient.Send(message);
                message.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button6.Text = "Отправить";
                button6.Enabled = true;
                DirectoryInfo dirInfo = new DirectoryInfo("send");
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }
        }
        /// <summary>
        /// Жирный текст
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("<b>" + richTextBox2.SelectedText + "</b>");
            if (richTextBox2.SelectionLength > 0)
            {
                richTextBox2.Paste();
            }
            Clipboard.Clear();
        }
        /// <summary>
        /// Курсив
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("<i>" + richTextBox2.SelectedText + "</i>");
            if (richTextBox2.SelectionLength > 0)
            {
                richTextBox2.Paste();
            }
            Clipboard.Clear();
        }
        /// <summary>
        /// Подчёркнутый
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("<u>" + richTextBox2.SelectedText + "</u>");
            if (richTextBox2.SelectionLength > 0)
            {
                richTextBox2.Paste();
            }
            Clipboard.Clear();
        }
    }
}
