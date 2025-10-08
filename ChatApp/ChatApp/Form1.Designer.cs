namespace ChatApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBoxMessages;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxIp;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonStartServer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            listBoxMessages = new ListBox();
            textBoxMessage = new TextBox();
            buttonSend = new Button();
            textBoxIp = new TextBox();
            textBoxPort = new TextBox();
            buttonConnect = new Button();
            buttonStartServer = new Button();
            SuspendLayout();
            // 
            // listBoxMessages
            // 
            listBoxMessages.BackColor = Color.White;
            listBoxMessages.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            listBoxMessages.FormattingEnabled = true;
            listBoxMessages.Location = new Point(14, 16);
            listBoxMessages.Margin = new Padding(3, 4, 3, 4);
            listBoxMessages.Name = "listBoxMessages";
            listBoxMessages.Size = new Size(411, 264);
            listBoxMessages.TabIndex = 0;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 162);
            textBoxMessage.Location = new Point(14, 289);
            textBoxMessage.Margin = new Padding(3, 4, 3, 4);
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.Size = new Size(297, 27);
            textBoxMessage.TabIndex = 1;
            textBoxMessage.Text = "Mesajınızı yazınız...";
            // 
            // buttonSend
            // 
            buttonSend.BackColor = SystemColors.HotTrack;
            buttonSend.Location = new Point(318, 289);
            buttonSend.Margin = new Padding(3, 4, 3, 4);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(107, 31);
            buttonSend.TabIndex = 2;
            buttonSend.Text = "Gönder";
            buttonSend.UseVisualStyleBackColor = false;
            buttonSend.Click += ButtonSend_Click;
            // 
            // textBoxIp
            // 
            textBoxIp.BackColor = Color.FromArgb(192, 192, 255);
            textBoxIp.Location = new Point(14, 341);
            textBoxIp.Margin = new Padding(3, 4, 3, 4);
            textBoxIp.Name = "textBoxIp";
            textBoxIp.Size = new Size(137, 27);
            textBoxIp.TabIndex = 3;
            textBoxIp.Text = "127.0.0.1";
            // 
            // textBoxPort
            // 
            textBoxPort.BackColor = Color.FromArgb(255, 192, 255);
            textBoxPort.Location = new Point(158, 341);
            textBoxPort.Margin = new Padding(3, 4, 3, 4);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(68, 27);
            textBoxPort.TabIndex = 4;
            textBoxPort.Text = "9000";
            // 
            // buttonConnect
            // 
            buttonConnect.BackColor = Color.Gray;
            buttonConnect.Location = new Point(233, 341);
            buttonConnect.Margin = new Padding(3, 4, 3, 4);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(91, 31);
            buttonConnect.TabIndex = 5;
            buttonConnect.Text = "Bağlan";
            buttonConnect.UseVisualStyleBackColor = false;
            buttonConnect.Click += ButtonConnect_Click;
            // 
            // buttonStartServer
            // 
            buttonStartServer.BackColor = Color.DarkSeaGreen;
            buttonStartServer.Location = new Point(331, 341);
            buttonStartServer.Margin = new Padding(3, 4, 3, 4);
            buttonStartServer.Name = "buttonStartServer";
            buttonStartServer.Size = new Size(94, 31);
            buttonStartServer.TabIndex = 6;
            buttonStartServer.Text = "Sunucu";
            buttonStartServer.UseVisualStyleBackColor = false;
            buttonStartServer.Click += ButtonStartServer_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ScrollBar;
            ClientSize = new Size(439, 388);
            Controls.Add(buttonStartServer);
            Controls.Add(buttonConnect);
            Controls.Add(textBoxPort);
            Controls.Add(textBoxIp);
            Controls.Add(buttonSend);
            Controls.Add(textBoxMessage);
            Controls.Add(listBoxMessages);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Mini Chat";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}