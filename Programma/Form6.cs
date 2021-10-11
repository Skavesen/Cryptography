using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Programma
{
    public partial class Form6 : Form
    {
        private static CspParameters cspp = new CspParameters();
        private static TripleDESCryptoServiceProvider tDESalg;
        const string keyFilee = "Key";
        private static RSACryptoServiceProvider rsa;
        public static string keyFile = null;
        public Form6()
        {
            cspp.KeyContainerName = keyFilee;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            tDESalg = new TripleDESCryptoServiceProvider();
            tDESalg.KeySize = 128;
            tDESalg.Mode = CipherMode.CBC;
            InitializeComponent();
        }
        Settings IniFile = new Settings(Application.StartupPath + "\\settings.ini");
        /// <summary>
        /// Кнопка шифрования файла
        /// </summary>
        private void EncryptFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                InitialDirectory = Application.StartupPath,
                RestoreDirectory = false
            };

            openFile.Title = "Выберите файл для шифрования.";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFile.FileName;
                if (fileName != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog()
                    {
                        InitialDirectory = Application.StartupPath + "\\send\\",
                        RestoreDirectory = false
                    };

                    saveFile.Filter = "Шифр (*.txt)|*.txt|All files (*.*)|*.*";

                    saveFile.Title = "Укажите имя для зашифрованного файла.";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        string encryptFileName = saveFile.FileName/* + ".txt"*/;
                        if (encryptFileName != null)
                        {
                            Encrypt(fileName, encryptFileName, keyFile);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Метод для шифрования файла
        /// </summary>
        private static void Encrypt(string fileName, string encryptFileName, string keyFile)
        {
            if (keyFile != null)
            {
                StreamReader sr = new StreamReader(keyFile);
                rsa.FromXmlString(sr.ReadToEnd());
                sr.Close();
            }
            ICryptoTransform transform = tDESalg.CreateEncryptor();
            byte[] keyEncrypted = rsa.Encrypt(tDESalg.Key, false);

            int lKey = keyEncrypted.Length;
            byte[] LenK = BitConverter.GetBytes(lKey);
            int lIV = tDESalg.IV.Length;
            byte[] LenIV = BitConverter.GetBytes(lIV);

            FileStream outFs = new FileStream(encryptFileName, FileMode.Create);
            outFs.Write(LenK, 0, 4);
            outFs.Write(LenIV, 0, 4);
            outFs.Write(keyEncrypted, 0, lKey);
            outFs.Write(tDESalg.IV, 0, lIV);

            CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write);
            int count = 0;
            int offset = 0;

            // blockSizeBytes can be any arbitrary size.
            int blockSize = tDESalg.BlockSize / 8;
            byte[] data = new byte[blockSize];
            int bytesRead = 0;

            FileStream inFs = new FileStream(fileName, FileMode.Open);
            do
            {
                count = inFs.Read(data, 0, blockSize);
                offset += count;
                outStreamEncrypted.Write(data, 0, count);
                bytesRead += blockSize;
            }
            while (count > 0);
            inFs.Close();
            outStreamEncrypted.FlushFinalBlock();
            outStreamEncrypted.Close();
        }
        /// <summary>
        /// Кнопка для расшифровки сообщения
        /// </summary>
        private void DecryptFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                InitialDirectory = Application.StartupPath + "\\Attachments\\",
                RestoreDirectory = false
            };

            openFile.Title = "Выберите файл для расшифрования.";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFile.FileName;
                if (fileName != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog()
                    {
                        InitialDirectory = Application.StartupPath + "\\send\\",
                        RestoreDirectory = false
                    };

                    saveFile.Filter = "Шифр (*.txt)|*.txt|All files (*.*)|*.*";

                    saveFile.Title = "Укажите имя для расшифрованного файла.";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        string encryptFileName = saveFile.FileName/* + ".txt"*/;
                        if (encryptFileName != null)
                        {
                            Decrypt(fileName, encryptFileName);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Метод для расшифровки сообщения
        /// </summary>
        private static void Decrypt(string fileName, string encryptFileName)
        {
            FileStream inFs = new FileStream(fileName, FileMode.Open);

            byte[] LenK = new byte[4];
            inFs.Read(LenK, 0, 4);
            byte[] LenIV = new byte[4];
            inFs.Read(LenIV, 0, 4);

            int lenK = BitConverter.ToInt32(LenK, 0);
            int lenIV = BitConverter.ToInt32(LenIV, 0);

            int startC = lenK + lenIV + 8;
            int lenC = (int)inFs.Length - startC;

            byte[] KeyEncrypted = new byte[lenK];
            byte[] IV = new byte[lenIV];

            inFs.Seek(8, SeekOrigin.Begin);
            inFs.Read(KeyEncrypted, 0, lenK);
            inFs.Seek(8 + lenK, SeekOrigin.Begin);
            inFs.Read(IV, 0, lenIV);

            byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

            ICryptoTransform transform = tDESalg.CreateDecryptor(KeyDecrypted, IV);

            FileStream outFs = new FileStream(encryptFileName, FileMode.Create);

            int count = 0;
            int offset = 0;

            int blockSizeBytes = tDESalg.BlockSize / 8;
            byte[] data = new byte[blockSizeBytes];


            inFs.Seek(startC, SeekOrigin.Begin);
            CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write);
            do
            {
                count = inFs.Read(data, 0, blockSizeBytes);
                offset += count;
                outStreamDecrypted.Write(data, 0, count);
            }
            while (count > 0);

            outStreamDecrypted.FlushFinalBlock();
            outStreamDecrypted.Close();
            outFs.Close();
            inFs.Close();

        }

        /// <summary>
        /// Кнопка для выполнения цифровой подписи
        /// </summary>
        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Выберите необходимый файл.";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFile.FileName;
                if (fileName != null)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog()
                    {
                        InitialDirectory = Application.StartupPath + "\\send\\",
                        RestoreDirectory = false
                    };
                    saveDialog.Title = "Укажите имя для подписанного файла.";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        string newFile = saveDialog.FileName + ".txt";

                        if (newFile != null)
                        {
                            Podpis(fileName, newFile);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Метод для выполенения ЭЦП
        /// </summary>
        private static void Podpis(string fileName, string newFile)
        {
            byte[] data = File.ReadAllBytes(fileName);

            byte[] signature = rsa.SignData(data, new SHA256CryptoServiceProvider());

            int lSign = signature.Length;

            byte[] LenSign = BitConverter.GetBytes(lSign);
            using (FileStream outFs = new FileStream(newFile, FileMode.Create))
            {
                outFs.Write(LenSign, 0, 4);
                outFs.Write(signature, 0, lSign);
                outFs.Write(data, 0, data.Length);
            }
        }
        /// <summary>
        /// Кнопка для проверки цифровой подписи
        /// </summary>
        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                InitialDirectory = Application.StartupPath + "\\Attachments\\",
                RestoreDirectory = false
            };
            openFile.Title = "Выберите файл для проверки цифровой подписи.";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFile.FileName;
                if (fileName != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Title = "Введите путь к файлу для сохранения данных";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        string newFile = saveFile.FileName + ".txt";

                        if (newFile != null)
                        {
                            bool success = ProverkaPodpisi(fileName, newFile, keyFilee);
                            if (success)
                            {
                                MessageBox.Show("Проверка подписи пройдена!");
                            }
                            else
                            {
                                MessageBox.Show("Проверка подписи не пройдена!");
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Метод для проверки ЭЦП
        /// </summary>
        private static bool ProverkaPodpisi(string fileName, string newFile, string keyFilee)
        {
            if (keyFile != null)
            {
                StreamReader sr = new StreamReader(keyFile);
                rsa.FromXmlString(sr.ReadToEnd());
                sr.Close();
            }

            using (FileStream inFs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    byte[] LenSign = new byte[4];
                    inFs.Read(LenSign, 0, 4);

                    int lenSign = BitConverter.ToInt32(LenSign, 0);

                    int startData = lenSign + 4;
                    int lenData = (int)inFs.Length - startData;

                    byte[] sign = new byte[lenSign];
                    byte[] data = new byte[lenData];

                    inFs.Read(sign, 0, lenSign);
                    inFs.Read(data, 0, lenData);

                    File.WriteAllBytes(newFile, data);
                    return rsa.VerifyData(data, new SHA256CryptoServiceProvider(), sign);
                }
                catch (Exception e) { return false; }

            }
        }
        /// <summary>
        /// Экспорт ключа для текста
        /// </summary>
        private void ExportPublicKey_Click(object sender, EventArgs e)
        {
            try
            {
                keyFile = IniFile.IniReadValue("send", "address");
                StreamWriter sw = new StreamWriter("send\\key." + keyFile + ".key");
                sw.Write(rsa.ToXmlString(false));
                sw.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так. Попробуйте ещё раз.");
            }
        }
        /// <summary>
        /// Импорт ключа для текста
        /// </summary>
        private void ImportPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                InitialDirectory = Application.StartupPath + "\\Keys\\",
                RestoreDirectory = false
            };
            openFile.Title = "Введите файл для импорта открытого ключа";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                keyFile = openFile.FileName;
            }
        }
    }
}
