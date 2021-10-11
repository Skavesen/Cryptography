namespace Programma
{
    partial class Form6
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
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.DecryptFile = new System.Windows.Forms.Button();
            this.ExportPublicKey = new System.Windows.Forms.Button();
            this.ImportPublicKey = new System.Windows.Forms.Button();
            this.EncryptFile = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label10.Location = new System.Drawing.Point(31, 70);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(224, 23);
            this.label10.TabIndex = 67;
            this.label10.Text = "Электронная цифровая подпись";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label9.Location = new System.Drawing.Point(75, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(142, 23);
            this.label9.TabIndex = 66;
            this.label9.Text = "Шифрование файла";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label22.Location = new System.Drawing.Point(239, 271);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(0, 16);
            this.label22.TabIndex = 65;
            // 
            // DecryptFile
            // 
            this.DecryptFile.BackColor = System.Drawing.Color.Beige;
            this.DecryptFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DecryptFile.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DecryptFile.Location = new System.Drawing.Point(144, 35);
            this.DecryptFile.Name = "DecryptFile";
            this.DecryptFile.Size = new System.Drawing.Size(126, 32);
            this.DecryptFile.TabIndex = 60;
            this.DecryptFile.Text = "Расшифровать";
            this.DecryptFile.UseVisualStyleBackColor = false;
            this.DecryptFile.Click += new System.EventHandler(this.DecryptFile_Click);
            // 
            // ExportPublicKey
            // 
            this.ExportPublicKey.BackColor = System.Drawing.Color.Beige;
            this.ExportPublicKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ExportPublicKey.Location = new System.Drawing.Point(12, 134);
            this.ExportPublicKey.Name = "ExportPublicKey";
            this.ExportPublicKey.Size = new System.Drawing.Size(258, 34);
            this.ExportPublicKey.TabIndex = 61;
            this.ExportPublicKey.Text = "Экспортировать публичный ключ RSA";
            this.ExportPublicKey.UseVisualStyleBackColor = false;
            this.ExportPublicKey.Click += new System.EventHandler(this.ExportPublicKey_Click);
            // 
            // ImportPublicKey
            // 
            this.ImportPublicKey.BackColor = System.Drawing.Color.Beige;
            this.ImportPublicKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ImportPublicKey.Location = new System.Drawing.Point(12, 174);
            this.ImportPublicKey.Name = "ImportPublicKey";
            this.ImportPublicKey.Size = new System.Drawing.Size(258, 34);
            this.ImportPublicKey.TabIndex = 62;
            this.ImportPublicKey.Text = "Импортировать публичный ключ RSA";
            this.ImportPublicKey.UseVisualStyleBackColor = false;
            this.ImportPublicKey.Click += new System.EventHandler(this.ImportPublicKey_Click);
            // 
            // EncryptFile
            // 
            this.EncryptFile.BackColor = System.Drawing.Color.Beige;
            this.EncryptFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EncryptFile.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.EncryptFile.Location = new System.Drawing.Point(12, 35);
            this.EncryptFile.Name = "EncryptFile";
            this.EncryptFile.Size = new System.Drawing.Size(126, 32);
            this.EncryptFile.TabIndex = 59;
            this.EncryptFile.Text = "Зашифровать";
            this.EncryptFile.UseVisualStyleBackColor = false;
            this.EncryptFile.Click += new System.EventHandler(this.EncryptFile_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.Beige;
            this.button10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button10.Location = new System.Drawing.Point(144, 96);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(126, 32);
            this.button10.TabIndex = 64;
            this.button10.Text = "Проверить";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.Beige;
            this.button11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button11.Location = new System.Drawing.Point(12, 96);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(126, 32);
            this.button11.TabIndex = 63;
            this.button11.Text = "Выполнить";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 233);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.DecryptFile);
            this.Controls.Add(this.ExportPublicKey);
            this.Controls.Add(this.ImportPublicKey);
            this.Controls.Add(this.EncryptFile);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button11);
            this.Name = "Form6";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form6";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button DecryptFile;
        private System.Windows.Forms.Button ExportPublicKey;
        private System.Windows.Forms.Button ImportPublicKey;
        private System.Windows.Forms.Button EncryptFile;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
    }
}