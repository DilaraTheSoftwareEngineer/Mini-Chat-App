using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private readonly List<ClientInfo> clients = new List<ClientInfo>();
        private readonly object clientsLock = new object();
        private Thread serverThread;

        // Ýstemci tarafý
        private TcpClient client;
        private StreamReader clientReader;
        private StreamWriter clientWriter;
        private Thread receiveThread;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            textBoxMessage.KeyDown += textBoxMessage_KeyDown;
            textBoxMessage.TextChanged += textBoxMessage_TextChanged;
        }

        private void SetupUI()
        {
            textBoxMessage.Clear();
            listBoxMessages.Items.Clear();
            buttonConnect.Enabled = true;
            buttonStartServer.Enabled = true;
            buttonSend.Enabled = false;
            textBoxIp.Text = "127.0.0.1";
            textBoxPort.Text = "9000";
        }

        // --- Sunucu tarafý ---

        private void ButtonStartServer_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxPort.Text, out int port))
            {
                MessageBox.Show("Geçerli bir port girin.");
                return;
            }

            server = new TcpListener(IPAddress.Any, port);
             
            server.Start();

            serverThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        TcpClient incoming = server.AcceptTcpClient();
                        Thread clientThread = new Thread(() => HandleClient(incoming));
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                }
                catch (SocketException) { /* Sunucu durdurulduysa çýkýþ */ }
            });

            serverThread.IsBackground = true;
            serverThread.Start();

            Invoke(new Action(() =>
            {
                listBoxMessages.Items.Add("Sunucu baþlatýldý.");
                buttonSend.Enabled = true;
            }));
        }

        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream ns = tcpClient.GetStream();
            using var reader = new StreamReader(ns, Encoding.UTF8, leaveOpen: true);
            using var writer = new StreamWriter(ns, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };

            // Ýlk satýr kullanýcý adý bekleniyor: "/nick:KullaniciAdi"
            string firstLine;
            try
            {
                firstLine = reader.ReadLine();
            }
            catch
            {
                tcpClient.Close();
                return;
            }

            string name = "Bilinmeyen";
            if (!string.IsNullOrEmpty(firstLine) && firstLine.StartsWith("/nick:"))
            {
                name = firstLine.Substring(6).Trim();
                if (string.IsNullOrWhiteSpace(name)) name = "Bilinmeyen";
            }

            var info = new ClientInfo
            {
                Client = tcpClient,
                Reader = reader,
                Writer = writer,
                Name = name
            };

            lock (clientsLock)
            {
                clients.Add(info);
            }

            BroadcastServerMessage($"-- {name} katýldý --", info);

            Invoke(new Action(() => listBoxMessages.Items.Add($"[{name}] baðlandý.")));

            try
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string framed = $"{name}: {line}";
                    BroadcastServerMessage(framed, info);
                    Invoke(new Action(() => listBoxMessages.Items.Add("Gelen: " + framed)));
                }
            }
            catch
            {
                // Okuma sýrasýnda hata/baðlantý kesildi
            }
            finally
            {
                lock (clientsLock)
                {
                    clients.Remove(info);
                }
                try { tcpClient.Close(); } catch { }
                BroadcastServerMessage($"-- {name} ayrýldý --", null);
                Invoke(new Action(() => listBoxMessages.Items.Add($"[{name}] ayrýldý.")));
            }
        }

        private void BroadcastServerMessage(string message, ClientInfo exclude)
        {
            lock (clientsLock)
            {
                foreach (var c in clients.ToArray())
                {
                    if (exclude != null && c == exclude) continue;
                    try
                    {
                        c.Writer.WriteLine(message);
                    }
                    catch
                    {
                        // Yazma hatasý varsa baðlantý kopmuþ olabilir; sonradan temizlenir
                    }
                }
            }
        }

        // --- Ýstemci tarafý ---

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            string ip = textBoxIp.Text;
            if (!int.TryParse(textBoxPort.Text, out int port))
            {
                MessageBox.Show("Geçerli bir port girin.");
                return;
            }

            client = new TcpClient();
            try
            {
                client.Connect(ip, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Baðlanýlamadý: " + ex.Message);
                return;
            }

            var ns = client.GetStream();
            clientReader = new StreamReader(ns, Encoding.UTF8, leaveOpen: true);
            clientWriter = new StreamWriter(ns, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };

            // Kullanýcýdan isim al
            string username = PromptUsername();
            if (string.IsNullOrWhiteSpace(username)) username = "Anonim";

            // Ýlk satýr olarak isim gönder
            clientWriter.WriteLine("/nick:" + username);

            receiveThread = new Thread(ReceiveMessages) { IsBackground = true };
            receiveThread.Start();

            listBoxMessages.Items.Add("Sunucuya baðlanýldý.");
            buttonSend.Enabled = true;
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            if (clientWriter != null && !string.IsNullOrWhiteSpace(textBoxMessage.Text))
            {
                string message = textBoxMessage.Text.Trim();
                try
                {
                    clientWriter.WriteLine(message);
                    listBoxMessages.Items.Add("Siz: " + message);
                    textBoxMessage.Clear();
                }
                catch
                {
                    MessageBox.Show("Mesaj gönderilemedi. Baðlantý kesilmiþ olabilir.");
                }
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                string line;
                while ((line = clientReader.ReadLine()) != null)
                {
                    Invoke(new Action(() => listBoxMessages.Items.Add("Gelen: " + line)));
                }
            }
            catch
            {
                // Baðlantý hatasý veya kapatma
            }
            finally
            {
                Invoke(new Action(() => listBoxMessages.Items.Add("Sunucu baðlantýsý kapandý.")));
            }
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ButtonSend_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {
            buttonSend.Enabled = !string.IsNullOrWhiteSpace(textBoxMessage.Text);
        }

        // Küçük modal prompt (Designer deðiþikliði gerektirmez)
        private string PromptUsername()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 140,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Kullanýcý adý",
                StartPosition = FormStartPosition.CenterParent
            };
            Label textLabel = new Label() { Left = 10, Top = 10, Text = "Kullanýcý adýnýzý girin:" };
            TextBox inputBox = new TextBox() { Left = 10, Top = 35, Width = 360 };
            Button okButton = new Button() { Text = "Tamam", Left = 210, Width = 75, Top = 65, DialogResult = DialogResult.OK };
            Button cancelButton = new Button() { Text = "Ýptal", Left = 295, Width = 75, Top = 65, DialogResult = DialogResult.Cancel };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(okButton);
            prompt.Controls.Add(cancelButton);
            prompt.AcceptButton = okButton;
            prompt.CancelButton = cancelButton;

            return prompt.ShowDialog(this) == DialogResult.OK ? inputBox.Text.Trim() : string.Empty;
        }

        // Yardýmcý sýnýf
        private class ClientInfo
        {
            public TcpClient Client { get; set; }
            public StreamReader Reader { get; set; }
            public StreamWriter Writer { get; set; }
            public string Name { get; set; }
        }
    }
}
