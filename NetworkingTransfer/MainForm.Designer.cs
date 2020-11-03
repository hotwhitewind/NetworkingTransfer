namespace NetworkingTransfer
{
    partial class MainForm
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
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.textBoxServerPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_IPAddressDst = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonResetServer = new System.Windows.Forms.Button();
            this.listBoxFilesToSend = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxIsCrypt = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPortDst = new System.Windows.Forms.TextBox();
            this.buttonSendFiles = new System.Windows.Forms.Button();
            this.buttonAddFile = new System.Windows.Forms.Button();
            this.buttonDelFile = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxFileRcvDirectory = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonChangeRcvDirectory = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.HorizontalScrollbar = true;
            this.listBoxStatus.Location = new System.Drawing.Point(12, 340);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.ScrollAlwaysVisible = true;
            this.listBoxStatus.Size = new System.Drawing.Size(359, 95);
            this.listBoxStatus.TabIndex = 0;
            // 
            // textBoxServerPort
            // 
            this.textBoxServerPort.Location = new System.Drawing.Point(9, 31);
            this.textBoxServerPort.MaxLength = 5;
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Size = new System.Drawing.Size(121, 20);
            this.textBoxServerPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Порт";
            // 
            // textBox_IPAddressDst
            // 
            this.textBox_IPAddressDst.Location = new System.Drawing.Point(6, 31);
            this.textBox_IPAddressDst.Name = "textBox_IPAddressDst";
            this.textBox_IPAddressDst.Size = new System.Drawing.Size(135, 20);
            this.textBox_IPAddressDst.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "IP адрес назначения";
            // 
            // buttonResetServer
            // 
            this.buttonResetServer.Location = new System.Drawing.Point(9, 57);
            this.buttonResetServer.Name = "buttonResetServer";
            this.buttonResetServer.Size = new System.Drawing.Size(121, 37);
            this.buttonResetServer.TabIndex = 5;
            this.buttonResetServer.Text = "Перезагрузить сервер";
            this.buttonResetServer.UseVisualStyleBackColor = true;
            this.buttonResetServer.Click += new System.EventHandler(this.buttonResetServer_Click);
            // 
            // listBoxFilesToSend
            // 
            this.listBoxFilesToSend.FormattingEnabled = true;
            this.listBoxFilesToSend.HorizontalScrollbar = true;
            this.listBoxFilesToSend.Location = new System.Drawing.Point(12, 239);
            this.listBoxFilesToSend.Name = "listBoxFilesToSend";
            this.listBoxFilesToSend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBoxFilesToSend.ScrollAlwaysVisible = true;
            this.listBoxFilesToSend.Size = new System.Drawing.Size(318, 95);
            this.listBoxFilesToSend.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonResetServer);
            this.groupBox1.Controls.Add(this.textBoxServerPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки сервера";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxIsCrypt);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxPortDst);
            this.groupBox2.Controls.Add(this.buttonSendFiles);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBox_IPAddressDst);
            this.groupBox2.Location = new System.Drawing.Point(182, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(147, 169);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Передача файлов";
            // 
            // checkBoxIsCrypt
            // 
            this.checkBoxIsCrypt.AutoSize = true;
            this.checkBoxIsCrypt.Location = new System.Drawing.Point(6, 103);
            this.checkBoxIsCrypt.Name = "checkBoxIsCrypt";
            this.checkBoxIsCrypt.Size = new System.Drawing.Size(84, 17);
            this.checkBoxIsCrypt.TabIndex = 8;
            this.checkBoxIsCrypt.Text = "Шифровать";
            this.checkBoxIsCrypt.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Порт назначения";
            // 
            // textBoxPortDst
            // 
            this.textBoxPortDst.Location = new System.Drawing.Point(6, 72);
            this.textBoxPortDst.MaxLength = 5;
            this.textBoxPortDst.Name = "textBoxPortDst";
            this.textBoxPortDst.Size = new System.Drawing.Size(135, 20);
            this.textBoxPortDst.TabIndex = 6;
            // 
            // buttonSendFiles
            // 
            this.buttonSendFiles.Location = new System.Drawing.Point(6, 126);
            this.buttonSendFiles.Name = "buttonSendFiles";
            this.buttonSendFiles.Size = new System.Drawing.Size(135, 37);
            this.buttonSendFiles.TabIndex = 5;
            this.buttonSendFiles.Text = "Передать файлы";
            this.buttonSendFiles.UseVisualStyleBackColor = true;
            this.buttonSendFiles.Click += new System.EventHandler(this.buttonSendFiles_Click);
            // 
            // buttonAddFile
            // 
            this.buttonAddFile.Location = new System.Drawing.Point(336, 239);
            this.buttonAddFile.Name = "buttonAddFile";
            this.buttonAddFile.Size = new System.Drawing.Size(35, 35);
            this.buttonAddFile.TabIndex = 9;
            this.buttonAddFile.Text = "+";
            this.buttonAddFile.UseVisualStyleBackColor = true;
            this.buttonAddFile.Click += new System.EventHandler(this.buttonAddFile_Click);
            // 
            // buttonDelFile
            // 
            this.buttonDelFile.Location = new System.Drawing.Point(336, 299);
            this.buttonDelFile.Name = "buttonDelFile";
            this.buttonDelFile.Size = new System.Drawing.Size(35, 35);
            this.buttonDelFile.TabIndex = 9;
            this.buttonDelFile.Text = "-";
            this.buttonDelFile.UseVisualStyleBackColor = true;
            this.buttonDelFile.Click += new System.EventHandler(this.buttonDelFile_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(335, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(35, 136);
            this.button1.TabIndex = 10;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxFileRcvDirectory
            // 
            this.textBoxFileRcvDirectory.Location = new System.Drawing.Point(12, 202);
            this.textBoxFileRcvDirectory.Name = "textBoxFileRcvDirectory";
            this.textBoxFileRcvDirectory.Size = new System.Drawing.Size(318, 20);
            this.textBoxFileRcvDirectory.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Папка для приема файлов";
            // 
            // buttonChangeRcvDirectory
            // 
            this.buttonChangeRcvDirectory.Location = new System.Drawing.Point(336, 200);
            this.buttonChangeRcvDirectory.Name = "buttonChangeRcvDirectory";
            this.buttonChangeRcvDirectory.Size = new System.Drawing.Size(35, 23);
            this.buttonChangeRcvDirectory.TabIndex = 13;
            this.buttonChangeRcvDirectory.Text = "...";
            this.buttonChangeRcvDirectory.UseVisualStyleBackColor = true;
            this.buttonChangeRcvDirectory.Click += new System.EventHandler(this.buttonChangeRcvDirectory_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 442);
            this.Controls.Add(this.buttonChangeRcvDirectory);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxFileRcvDirectory);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonDelFile);
            this.Controls.Add(this.buttonAddFile);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBoxFilesToSend);
            this.Controls.Add(this.listBoxStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NetworkingTransfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.TextBox textBoxServerPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_IPAddressDst;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonResetServer;
        private System.Windows.Forms.ListBox listBoxFilesToSend;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSendFiles;
        private System.Windows.Forms.Button buttonAddFile;
        private System.Windows.Forms.Button buttonDelFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPortDst;
        private System.Windows.Forms.CheckBox checkBoxIsCrypt;
        private System.Windows.Forms.TextBox textBoxFileRcvDirectory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonChangeRcvDirectory;
    }
}

