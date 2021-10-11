using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System.Drawing;

namespace Programma
{
    public partial class Form5 : Form
    {
        Settings IniFile = new Settings(Application.StartupPath + "\\settings.ini");
        private Pop3Client pop3Client;
        private ImapClient imapClient;
        private Dictionary<int, OpenPop.Mime.Message> messages;
        Settings textmessageEmail;
        public Form5()
        {
            pop3Client = new Pop3Client();
            imapClient = new ImapClient();
            messages = new Dictionary<int, OpenPop.Mime.Message>(); DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath + "\\Attachments\\");
            DirectoryInfo directorysend = new DirectoryInfo(Application.StartupPath + "\\send\\");
            if (directoryInfo.Exists)
            {
                DirectorySecurity accessControl = directoryInfo.GetAccessControl();
                directoryInfo.Delete(true);
                directoryInfo.Create(accessControl);
            }
            else
                directoryInfo.Create();
            if (directorysend.Exists)
            {
                DirectorySecurity accessControl = directorysend.GetAccessControl();
                directorysend.Delete(true);
                directorysend.Create(accessControl);
            }
            else
                directorysend.Create();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //button1.Enabled = false;
            try
            {
                button1.Image = Image.FromFile(Application.StartupPath + "\\2.png");
                treeView1.Nodes.Clear();
                ReceiveMails();
                SaveMails();
            }
            finally
            {
                //button1.Enabled = true;
                button1.Image = Image.FromFile(Application.StartupPath + "\\clock_date_internet_refresh_reload_security_time_icon_127112.png");
            }
        }
        /// <summary>
        /// Открытие загруженных атачментов
        /// </summary>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                richTextBox1.Text = Mesbody(e.Node);
                webBrowser1.Visible = false;
            }
            else
            {
                richTextBox1.Text = Mesbody(e.Node.Parent);
                switch ((e.Node.Tag as FileInfo).Extension.ToLowerInvariant())
                {

                    case ".rtf":
                        richTextBox1.Visible = true;
                        richTextBox1.Text = File.ReadAllText((e.Node.Tag as FileInfo).FullName);
                        webBrowser1.Visible = false;
                        break;
                    case ".txt":
                        richTextBox1.Visible = true;
                        richTextBox1.Text = File.ReadAllText((e.Node.Tag as FileInfo).FullName);
                        webBrowser1.Visible = false;
                        break;
                    default:
                        richTextBox1.Visible = false;
                        webBrowser1.Visible = true;
                        webBrowser1.Navigate((e.Node.Tag as FileInfo).FullName);
                        break;
                }
            }
        }  
        /// <summary>
            /// Загрузка содержимого сообщений
            /// </summary>
        int i = 0;
        private string Mesbody(TreeNode e)
        {
            string str = "";
            OpenPop.Mime.Message message;
            if (messages.TryGetValue((int)Convert.ToInt16(e.Tag), out message))
            {
                textBox1.Text = message.Headers.From.MailAddress.ToString();
                try
                {
                    label4.Text = message.Headers.To[0].MailAddress.ToString();
                }
                catch { }
                textBox2.Text = message.Headers.Subject;
                OpenPop.Mime.MessagePart plainTextVersion = message.FindFirstPlainTextVersion();
                if (plainTextVersion != null)
                {
                    str = plainTextVersion.GetBodyAsText();
                }
                else
                {
                    List<OpenPop.Mime.MessagePart> allTextVersions = message.FindAllTextVersions();
                    str = allTextVersions.Count < 1 ? "Не удается найти текстовую версию сообщения" : allTextVersions[0].GetBodyAsText();
                }
                textmessageEmail = new Settings(Application.StartupPath + "\\Attachments\\" + "\\" + i + ".txt");
                textmessageEmail.IniWriteValue("Письмо", "Кому", label1.Text);
                textmessageEmail.IniWriteValue("Письмо", "От кого", textBox1.Text);
                textmessageEmail.IniWriteValue("Письмо", "Тема", textBox2.Text);
                textmessageEmail.IniWriteValue("Письмо", "Текст сообщения", richTextBox1.Text);
            }
            i++;
            return str;
        }
        /// <summary>
        /// Метод получения писем с почты
        /// </summary>
        private void ReceiveMails()
        {
            try
            {
                if (pop3Client.Connected)
                    pop3Client.Disconnect();
                int port;
                try
                {
                    port = (int)Convert.ToInt16(IniFile.IniReadValue("recive", "port"));
                }
                catch (Exception)
                {
                    port = 110;
                }
                pop3Client.Connect(IniFile.IniReadValue("recive", "server"), port, IniFile.IniReadValue("recive", "ssl").Contains("1"));
                pop3Client.Authenticate(IniFile.IniReadValue("recive", "login"), IniFile.IniReadValue("recive", "password"));
                int messageCount = pop3Client.GetMessageCount();
                messages.Clear();
                for (int index = 0; index < messageCount; index++)
                //for (int index = messageCount; index >= 1; --index)
                {
                    try
                    {
                        Application.DoEvents();
                        OpenPop.Mime.Message message = pop3Client.GetMessage(index);
                        TreeNode node = new TreeNode(Convert.ToDateTime(message.Headers.Date).ToString("dd-MM-yyyy HH:mm:ss") + " | " + (object)message.Headers.From.MailAddress);
                        node.Tag = (object)index;
                        foreach (OpenPop.Mime.MessagePart allAttachment in message.FindAllAttachments())
                        {
                            if (allAttachment != null)
                            {
                                TreeNode treeNode = node.Nodes.Add(allAttachment.FileName);
                                string str = allAttachment.FileName.Split('.')[1];
                                string publicKey = allAttachment.FileName.Split('.')[0];
                                string folder = "Attachments";
                                if (publicKey == "key")
                                {
                                    folder = "Keys";
                                    FileInfo file = new FileInfo(Application.StartupPath + "\\" + folder + "\\" + allAttachment.FileName);
                                    allAttachment.Save(file);
                                    treeNode.Tag = (object)file;
                                }
                                else
                                {
                                    FileInfo file = new FileInfo(Application.StartupPath + "\\" + folder + "\\" + allAttachment.FileName + "." + DateTime.Now.Ticks.ToString() + "." + str);
                                    allAttachment.Save(file);
                                    treeNode.Tag = (object)file;
                                }
                            }
                        }
                        treeView1.Nodes.Add(node);
                        messages.Add(index, message);
                        if (IniFile.IniReadValue("recive", "delmessage").Contains("1"))
                            pop3Client.DeleteMessage(index);
                    }
                    catch
                    {
                    }
                }
            }
            catch (InvalidLoginException)
            {
                int num = (int)MessageBox.Show((IWin32Window)this, "Сервер не принимает учетные данные пользователя!", "Проверка подлинности сервера POP3");
            }
            catch (PopServerNotFoundException)
            {
                int num = (int)MessageBox.Show((IWin32Window)this, "Сервер не найден", "Получение POP3");
            }
            catch (PopServerLockedException)
            {
                int num = (int)MessageBox.Show((IWin32Window)this, "Доступ к почтовому ящику блокируется", "Учетная запись POP3 заблокирована");
            }
            catch (LoginDelayException)
            {
                int num = (int)MessageBox.Show((IWin32Window)this, "Вход не допускается. Сервер обеспечивает задержку между входами. Вы уже подключились?", "Задержка входа учетной записи POP3");
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show((IWin32Window)this, "Произошла ошибка при получении почты. " + ex.Message, "Получение POP3");
            }
            finally
            {
            }
        }
        /// <summary>
        /// Сохранение всех сообщений
        /// </summary>
        string Subject = "";
        string From = "";
        string To = "";
        string Cc = "";
        string Sender = "";
        string TextBody = "";
        int j = 0;
        string folder = "";
        string mesage = "";
        int count = 0;
        private void SaveMails()
        {
            for (int papka = 0; papka < 5; papka++)
            {
                try
                {
                    j = 0;
                    using (var client = new ImapClient())
                    {
                        int port;
                        try
                        {
                            port = (int)Convert.ToInt16(IniFile.IniReadValue("recive", "imapport"));
                        }
                        catch (Exception)
                        {
                            port = 110;
                        }
                        client.Connect(IniFile.IniReadValue("recive", "imapserver"), port, true);
                        //client.Connect(imap, 993, true); // Конект к серверу IMap первое значение это название сервера,второе это порт сервера и третье используется ли SSL подключение
                        client.AuthenticationMechanisms.Remove("XOAUTH");
                        string login = IniFile.IniReadValue("send", "login");
                        string password = IniFile.IniReadValue("send", "password");
                        client.Authenticate(login, password); // Авторизируемся данными из полей
                        if (papka == 0)
                        {
                            client.Inbox.Open(FolderAccess.ReadWrite);
                            count = client.Inbox.Count;
                        }
                        if (papka == 1)
                        {
                            client.GetFolder(SpecialFolder.Trash).Open(FolderAccess.ReadWrite);
                            count = client.GetFolder(SpecialFolder.Trash).Count;
                        }
                        if (papka == 2)
                        {
                            client.GetFolder(SpecialFolder.Sent).Open(FolderAccess.ReadWrite);
                            count = client.GetFolder(SpecialFolder.Sent).Count;
                        }
                        if (papka == 3)
                        {
                            client.GetFolder(SpecialFolder.Drafts).Open(FolderAccess.ReadWrite);
                            count = client.GetFolder(SpecialFolder.Drafts).Count;
                        }
                        if (papka == 4)
                        {
                            client.GetFolder(SpecialFolder.Junk).Open(FolderAccess.ReadWrite);
                            count = client.GetFolder(SpecialFolder.Junk).Count;
                        }
                        if (j < count)
                        {
                            if (papka == 0)
                            {
                                var items = client.Inbox.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                                foreach (var item in items)
                                {
                                    var message = client.Inbox.GetMessage(j);
                                    ////////////////////////////
                                    if (message.Subject != null)
                                    {
                                        Subject = "Subject: " + message.Subject.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Subject = "Subject: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.From != null)
                                    {
                                        From = "From: " + message.From.ToString() + "\n";
                                    }
                                    else
                                    {
                                        From = "From: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.To != null)
                                    {
                                        To = "To: " + message.To.ToString() + "\n";
                                    }
                                    else
                                    {
                                        To = "To: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Cc != null)
                                    {
                                        Cc = "Cc: " + message.Cc.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Cc = "Cc: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Sender != null)
                                    {
                                        Sender = "Sender: " + message.Sender.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Sender = "Sender: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.TextBody != null)
                                    {
                                        TextBody = "TextBody: " + message.TextBody.ToString() + "\n";
                                    }
                                    if (message.HtmlBody != null)
                                    {
                                        TextBody = "TextBody: " + message.HtmlBody.ToString() + "\n";
                                    }
                                    else
                                    {
                                        TextBody = "TextBody: " + "Не удается найти текстовую версию сообщения" + "\n";
                                    }
                                    var directory = Path.Combine(Application.StartupPath + "\\inbox\\", item.UniqueId.ToString());
                                    Directory.CreateDirectory(directory);
                                    using (FileStream fstream = new FileStream(directory + "\\Message.txt", FileMode.OpenOrCreate))
                                    {
                                        byte[] input = Encoding.Default.GetBytes(Sender + From + To + Cc + Subject + TextBody);
                                        fstream.Write(input, 0, input.Length);

                                    }
                                    foreach (var attachment in item.Attachments)
                                    {
                                        // download the attachment just like we did with the body
                                        var entity = client.Inbox.GetBodyPart(item.UniqueId, attachment);

                                        // attachments can be either message/rfc822 parts or regular MIME parts
                                        if (entity is MimeKit.MessagePart)
                                        {
                                            var rfc822 = (MimeKit.MessagePart)entity;

                                            var path = Path.Combine(directory, attachment.PartSpecifier + ".eml");

                                            rfc822.Message.WriteTo(path);
                                        }
                                        else
                                        {
                                            var part = (MimePart)entity;

                                            // note: it's possible for this to be null, but most will specify a filename
                                            var fileName = part.FileName;

                                            var path = Path.Combine(directory, fileName);

                                            // decode and save the content to a file
                                            using (var stream = File.Create(path))
                                                part.Content.DecodeTo(stream);
                                        }
                                    }
                                    j++;
                                }
                            }
                            if (papka == 1)
                            {
                                var items = client.GetFolder(SpecialFolder.Trash).Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                                foreach (var item in items)
                                {
                                    var message = client.GetFolder(SpecialFolder.Trash).GetMessage(j);
                                    ////////////////////////////
                                    if (message.Subject != null)
                                    {
                                        Subject = "Subject: " + message.Subject.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Subject = "Subject: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.From != null)
                                    {
                                        From = "From: " + message.From.ToString() + "\n";
                                    }
                                    else
                                    {
                                        From = "From: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.To != null)
                                    {
                                        To = "To: " + message.To.ToString() + "\n";
                                    }
                                    else
                                    {
                                        To = "To: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Cc != null)
                                    {
                                        Cc = "Cc: " + message.Cc.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Cc = "Cc: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Sender != null)
                                    {
                                        Sender = "Sender: " + message.Sender.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Sender = "Sender: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.TextBody != null)
                                    {
                                        TextBody = "TextBody: " + message.TextBody.ToString() + "\n";
                                    }
                                    if (message.HtmlBody != null)
                                    {
                                        TextBody = "TextBody: " + message.HtmlBody.ToString() + "\n";
                                    }
                                    else
                                    {
                                        TextBody = "TextBody: " + "Не удается найти текстовую версию сообщения" + "\n";
                                    }
                                    var directory = Path.Combine(Application.StartupPath + "\\trash\\", item.UniqueId.ToString());
                                    Directory.CreateDirectory(directory);
                                    using (FileStream fstream = new FileStream(directory + "\\Message.txt", FileMode.OpenOrCreate))
                                    {
                                        byte[] input = Encoding.Default.GetBytes(Sender + From + To + Cc + Subject + TextBody);
                                        fstream.Write(input, 0, input.Length);

                                    }
                                    foreach (var attachment in item.Attachments)
                                    {
                                        // download the attachment just like we did with the body
                                        var entity = client.GetFolder(SpecialFolder.Trash).GetBodyPart(item.UniqueId, attachment);

                                        // attachments can be either message/rfc822 parts or regular MIME parts
                                        if (entity is MimeKit.MessagePart)
                                        {
                                            var rfc822 = (MimeKit.MessagePart)entity;

                                            var path = Path.Combine(directory, attachment.PartSpecifier + ".eml");

                                            rfc822.Message.WriteTo(path);
                                        }
                                        else
                                        {
                                            var part = (MimePart)entity;

                                            // note: it's possible for this to be null, but most will specify a filename
                                            var fileName = part.FileName;

                                            var path = Path.Combine(directory, fileName);

                                            // decode and save the content to a file
                                            using (var stream = File.Create(path))
                                                part.Content.DecodeTo(stream);
                                        }
                                    }
                                    j++;
                                }
                            }
                            if (papka == 2)
                            {
                                var items = client.GetFolder(SpecialFolder.Sent).Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                                foreach (var item in items)
                                {
                                    var message = client.GetFolder(SpecialFolder.Sent).GetMessage(j);
                                    ////////////////////////////
                                    if (message.Subject != null)
                                    {
                                        Subject = "Subject: " + message.Subject.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Subject = "Subject: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.From != null)
                                    {
                                        From = "From: " + message.From.ToString() + "\n";
                                    }
                                    else
                                    {
                                        From = "From: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.To != null)
                                    {
                                        To = "To: " + message.To.ToString() + "\n";
                                    }
                                    else
                                    {
                                        To = "To: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Cc != null)
                                    {
                                        Cc = "Cc: " + message.Cc.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Cc = "Cc: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Sender != null)
                                    {
                                        Sender = "Sender: " + message.Sender.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Sender = "Sender: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.TextBody != null)
                                    {
                                        TextBody = "TextBody: " + message.TextBody.ToString() + "\n";
                                    }
                                    if (message.HtmlBody != null)
                                    {
                                        TextBody = "TextBody: " + message.HtmlBody.ToString() + "\n";
                                    }
                                    else
                                    {
                                        TextBody = "TextBody: " + "Не удается найти текстовую версию сообщения" + "\n";
                                    }
                                    var directory = Path.Combine(Application.StartupPath + "\\sent\\", item.UniqueId.ToString());
                                    Directory.CreateDirectory(directory);
                                    using (FileStream fstream = new FileStream(directory + "\\Message.txt", FileMode.OpenOrCreate))
                                    {
                                        byte[] input = Encoding.Default.GetBytes(Sender + From + To + Cc + Subject + TextBody);
                                        fstream.Write(input, 0, input.Length);

                                    }
                                    foreach (var attachment in item.Attachments)
                                    {
                                        // download the attachment just like we did with the body
                                        var entity = client.GetFolder(SpecialFolder.Sent).GetBodyPart(item.UniqueId, attachment);

                                        // attachments can be either message/rfc822 parts or regular MIME parts
                                        if (entity is MimeKit.MessagePart)
                                        {
                                            var rfc822 = (MimeKit.MessagePart)entity;

                                            var path = Path.Combine(directory, attachment.PartSpecifier + ".eml");

                                            rfc822.Message.WriteTo(path);
                                        }
                                        else
                                        {
                                            var part = (MimePart)entity;

                                            // note: it's possible for this to be null, but most will specify a filename
                                            var fileName = part.FileName;

                                            var path = Path.Combine(directory, fileName);

                                            // decode and save the content to a file
                                            using (var stream = File.Create(path))
                                                part.Content.DecodeTo(stream);
                                        }
                                    }
                                    j++;
                                }
                            }
                            if (papka == 3)
                            {
                                var items = client.GetFolder(SpecialFolder.Drafts).Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                                foreach (var item in items)
                                {
                                    var message = client.GetFolder(SpecialFolder.Drafts).GetMessage(j);
                                    ////////////////////////////
                                    if (message.Subject != null)
                                    {
                                        Subject = "Subject: " + message.Subject.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Subject = "Subject: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.From != null)
                                    {
                                        From = "From: " + message.From.ToString() + "\n";
                                    }
                                    else
                                    {
                                        From = "From: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.To != null)
                                    {
                                        To = "To: " + message.To.ToString() + "\n";
                                    }
                                    else
                                    {
                                        To = "To: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Cc != null)
                                    {
                                        Cc = "Cc: " + message.Cc.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Cc = "Cc: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Sender != null)
                                    {
                                        Sender = "Sender: " + message.Sender.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Sender = "Sender: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.TextBody != null)
                                    {
                                        TextBody = "TextBody: " + message.TextBody.ToString() + "\n";
                                    }
                                    if (message.HtmlBody != null)
                                    {
                                        TextBody = "TextBody: " + message.HtmlBody.ToString() + "\n";
                                    }
                                    else
                                    {
                                        TextBody = "TextBody: " + "Не удается найти текстовую версию сообщения" + "\n";
                                    }
                                    var directory = Path.Combine(Application.StartupPath + "\\drafts\\", item.UniqueId.ToString());
                                    Directory.CreateDirectory(directory);
                                    using (FileStream fstream = new FileStream(directory + "\\Message.txt", FileMode.OpenOrCreate))
                                    {
                                        byte[] input = Encoding.Default.GetBytes(Sender + From + To + Cc + Subject + TextBody);
                                        fstream.Write(input, 0, input.Length);

                                    }
                                    foreach (var attachment in item.Attachments)
                                    {
                                        // download the attachment just like we did with the body
                                        var entity = client.GetFolder(SpecialFolder.Drafts).GetBodyPart(item.UniqueId, attachment);

                                        // attachments can be either message/rfc822 parts or regular MIME parts
                                        if (entity is MimeKit.MessagePart)
                                        {
                                            var rfc822 = (MimeKit.MessagePart)entity;

                                            var path = Path.Combine(directory, attachment.PartSpecifier + ".eml");

                                            rfc822.Message.WriteTo(path);
                                        }
                                        else
                                        {
                                            var part = (MimePart)entity;

                                            // note: it's possible for this to be null, but most will specify a filename
                                            var fileName = part.FileName;

                                            var path = Path.Combine(directory, fileName);

                                            // decode and save the content to a file
                                            using (var stream = File.Create(path))
                                                part.Content.DecodeTo(stream);
                                        }
                                    }
                                    j++;
                                }
                            }
                            if (papka == 4)
                            {
                                var items = client.GetFolder(SpecialFolder.Junk).Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                                foreach (var item in items)
                                {
                                    var message = client.GetFolder(SpecialFolder.Junk).GetMessage(j);
                                    ////////////////////////////
                                    if (message.Subject != null)
                                    {
                                        Subject = "Subject: " + message.Subject.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Subject = "Subject: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.From != null)
                                    {
                                        From = "From: " + message.From.ToString() + "\n";
                                    }
                                    else
                                    {
                                        From = "From: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.To != null)
                                    {
                                        To = "To: " + message.To.ToString() + "\n";
                                    }
                                    else
                                    {
                                        To = "To: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Cc != null)
                                    {
                                        Cc = "Cc: " + message.Cc.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Cc = "Cc: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.Sender != null)
                                    {
                                        Sender = "Sender: " + message.Sender.ToString() + "\n";
                                    }
                                    else
                                    {
                                        Sender = "Sender: " + "Пусто" + "\n";
                                    }
                                    ////////////////////////////
                                    if (message.TextBody != null)
                                    {
                                        TextBody = "TextBody: " + message.TextBody.ToString() + "\n";
                                    }
                                    if (message.HtmlBody != null)
                                    {
                                        TextBody = "TextBody: " + message.HtmlBody.ToString() + "\n";
                                    }
                                    else
                                    {
                                        TextBody = "TextBody: " + "Не удается найти текстовую версию сообщения" + "\n";
                                    }
                                    var directory = Path.Combine(Application.StartupPath + "\\junk\\", item.UniqueId.ToString());
                                    Directory.CreateDirectory(directory);
                                    using (FileStream fstream = new FileStream(directory + "\\Message.txt", FileMode.OpenOrCreate))
                                    {
                                        byte[] input = Encoding.Default.GetBytes(Sender + From + To + Cc + Subject + TextBody);
                                        fstream.Write(input, 0, input.Length);

                                    }
                                    foreach (var attachment in item.Attachments)
                                    {
                                        // download the attachment just like we did with the body
                                        var entity = client.GetFolder(SpecialFolder.Junk).GetBodyPart(item.UniqueId, attachment);

                                        // attachments can be either message/rfc822 parts or regular MIME parts
                                        if (entity is MimeKit.MessagePart)
                                        {
                                            var rfc822 = (MimeKit.MessagePart)entity;

                                            var path = Path.Combine(directory, attachment.PartSpecifier + ".eml");

                                            rfc822.Message.WriteTo(path);
                                        }
                                        else
                                        {
                                            var part = (MimePart)entity;

                                            // note: it's possible for this to be null, but most will specify a filename
                                            var fileName = part.FileName;

                                            var path = Path.Combine(directory, fileName);

                                            // decode and save the content to a file
                                            using (var stream = File.Create(path))
                                                part.Content.DecodeTo(stream);
                                        }
                                    }
                                    j++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
