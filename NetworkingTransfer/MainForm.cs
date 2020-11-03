using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NetworkingTransfer
{
    public partial class MainForm : Form
    {
        private readonly TcpModule _tcpmoduleServer = new TcpModule();
        private readonly TcpModule _tcpmoduleClient = new TcpModule();

        [Serializable]
        public struct Properties
        {
            public string Port;
            public string IpAddressDst;
            public string PortDst;
            public string RcvDirectory;
            public bool IsCrypt;
        }

        private Properties _localProp;

        public MainForm()
        {
            InitializeComponent();
            var timer = new System.Timers.Timer {AutoReset = true};
            timer.Elapsed += TimerOnElapsed;
            timer.Enabled = true;
            timer.Interval = 1000;
            var systemTimeDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Text = "NetworkTransfer (" + systemTimeDate + ")";
            _tcpmoduleServer.Receive += _tcpmoduleServer_Receive;
            _tcpmoduleServer.Disconnected += _tcpmoduleServer_Disconnected;
            _tcpmoduleServer.Accept += _tcpmoduleServer_Accept;
            _tcpmoduleServer.ShowStatusMessage += _tcpmoduleServer_ShowStatusMessage;

            _tcpmoduleClient.Receive += _tcpmoduleClient_Receive;
            _tcpmoduleClient.Connected += _tcpmoduleClient_Connected;
            _tcpmoduleClient.ShowStatusMessage += _tcpmoduleClient_ShowStatusMessage;

            if (File.Exists("properties.xml"))
            {
                var x = new XmlSerializer(typeof(Properties));
                using (TextReader reader = new StreamReader("properties.xml"))
                {
                    _localProp = (Properties) x.Deserialize(reader);
                    textBoxServerPort.Text = _localProp.Port;
                    textBox_IPAddressDst.Text = _localProp.IpAddressDst;
                    textBoxPortDst.Text = _localProp.PortDst;
                    checkBoxIsCrypt.Checked = _localProp.IsCrypt;
                    textBoxFileRcvDirectory.Text = _localProp.RcvDirectory;
                }
            }
            else
            {
                textBoxServerPort.Text = @"33333";
                textBoxPortDst.Text = @"1234";
                textBox_IPAddressDst.Text = @"127.0.0.1";
                checkBoxIsCrypt.Checked = false;
                textBoxFileRcvDirectory.Text = Application.StartupPath;
            }
            _tcpmoduleServer.DstDirectory = textBoxFileRcvDirectory.Text;

            try
            {
                if(!button1.Visible)
                    _tcpmoduleServer.StartServer(Convert.ToInt32(textBoxServerPort.Text));
            }
            catch (Exception e)
            {
                _tcpmoduleServer_ShowStatusMessage(e.Message);
            }
        }

        [Localizable(false)]
        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private delegate void UpdateDateTime(object sender, ElapsedEventArgs elapsedEventArgs);

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var systemTimeDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            if (InvokeRequired)
            {
                UpdateDateTime udp = TimerOnElapsed;
                Invoke(udp, sender, elapsedEventArgs);
            }
            Text = "NetworkTransfer (" + systemTimeDate + ")";
        }

        void _tcpmoduleServer_Accept(object sender)
        {
            _tcpmoduleServer_ShowStatusMessage("Клиент подключился!");
        }

        void _tcpmoduleServer_Disconnected(object sender, string result)
        {
            _tcpmoduleServer_ShowStatusMessage(result);
        }

        private void _tcpmoduleServer_Receive(object sender, ReceiveEventArgs e)
        {

            if (e.SendInfo.Message != null)
            {
                //сервер получил клиентский паблик в ответ формируем свой и отправляем клиенту
                _tcpmoduleServer.DiffieHellman = new DiffieHellmanWorker();
                //отсылаем второму участнику
                _tcpmoduleServer.IsCrypt = true;
                var t = new Thread(_tcpmoduleServer.SendData);
                var param = new SendDataParam
                {
                    Message = Convert.ToBase64String(_tcpmoduleServer.DiffieHellman.PublicKey)
                };
                t.Start(param);
                //и формируем свой секретный ключ
                _tcpmoduleServer.DiffieHellman.CreateSecretKey(e.SendInfo.Message);
                _tcpmoduleServer_ShowStatusMessage(string.Format("Получен публичный ключ клиента:{0}", e.SendInfo.Message));
            }

            if (e.SendInfo.Filesize > 0)
            {
                _tcpmoduleServer_ShowStatusMessage("Файл: " + e.SendInfo.Filename);
            }
        }

        private delegate void UpdateReceiveDisplayDelegate(string message);

        public void _tcpmoduleServer_ShowStatusMessage(string message)
        {
            if (listBoxStatus.InvokeRequired)
            {
                UpdateReceiveDisplayDelegate rdd = _tcpmoduleServer_ShowStatusMessage;

                // Данный метод вызывается в дочернем потоке,
                // ищет основной поток и выполняет делегат указанный в качестве параметра 
                // в главном потоке, безопасно обновляя интерфейс формы.
                Invoke(rdd, message);
            }
            else
            {
                // Если не требуется вызывать метод Invoke, обратимся напрямую к элементу формы.
                listBoxStatus.Items.Add((listBoxStatus.Items.Count + 1) + ". " + message);
            }
        }

        private void _tcpmoduleClient_Connected(object sender, string result)
        {
            _tcpmoduleClient_ShowStatusMessage(result);
            //только здесь отправляем файлы, если все гуд
            if (result != "Подключение успешно!") return;
            if (checkBoxIsCrypt.Checked)
            {
                SendDataParam param = new SendDataParam();
                //производим обмен ключами
                //создаем публичный ключ
                _tcpmoduleClient.DiffieHellman = new DiffieHellmanWorker();
                param.Message = Convert.ToBase64String(_tcpmoduleClient.DiffieHellman.PublicKey);
                //отсылаем второму участнику
                var t = new Thread(_tcpmoduleClient.SendData);
                t.Start(param);
            }
            else
            {
                //отправим файлы без шифрования
                var t = new Thread(_tcpmoduleClient.SendMultipleFiles);
                var fileslist =
                    (from object item in listBoxFilesToSend.Items select item.ToString()).ToList();
                t.Start(fileslist);
            }
        }

        private void _tcpmoduleClient_Receive(object sender, ReceiveEventArgs e)
        {
            if (e.SendInfo.Message != null)
            {
                _tcpmoduleClient_ShowStatusMessage(string.Format("Получен публичный ключ сервера:{0}", e.SendInfo.Message));
                //получаем паблик от сервера и формируем свой секретный ключ и можем передавать файлы
                _tcpmoduleClient.DiffieHellman.CreateSecretKey(e.SendInfo.Message);
                //теперь идем по списку файлов и передаем их
                var t = new Thread(_tcpmoduleClient.SendMultipleFiles);
                var fileslist = (from object item in listBoxFilesToSend.Items select item.ToString()).ToList();
                t.Start(fileslist);
            }

            if (e.SendInfo.Filesize > 0)
            {
                //ShowReceiveMessage("Файл: " + e.sendInfo.filename);
                _tcpmoduleClient_ShowStatusMessage("Файл: " + e.SendInfo.Filename);
            }
        }

        public void _tcpmoduleClient_ShowStatusMessage(string message)
        {
            if (listBoxStatus.InvokeRequired)
            {
                UpdateReceiveDisplayDelegate rdd = _tcpmoduleClient_ShowStatusMessage;

                // Данный метод вызывается в дочернем потоке,
                // ищет основной поток и выполняет делегат указанный в качестве параметра 
                // в главном потоке, безопасно обновляя интерфейс формы.
                Invoke(rdd, message);
            }
            else
            {
                // Если не требуется вызывать метод Invoke, обратимся напрямую к элементу формы.
                listBoxStatus.Items.Add((listBoxStatus.Items.Count + 1) + ". " + message);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _tcpmoduleServer.CloseSocket();
            _tcpmoduleClient.CloseSocket();
            _localProp.Port = textBoxServerPort.Text;
            _localProp.IpAddressDst = textBox_IPAddressDst.Text;
            _localProp.IsCrypt = checkBoxIsCrypt.Checked;
            _localProp.PortDst = textBoxPortDst.Text;
            _localProp.RcvDirectory = textBoxFileRcvDirectory.Text;
            var x = new XmlSerializer(typeof (Properties));
            using (TextWriter writer = new StreamWriter("properties.xml"))
            {
                x.Serialize(writer, _localProp);
            }
        }

        private void buttonResetServer_Click(object sender, EventArgs e)
        {
            _tcpmoduleServer.StopServer();
            _tcpmoduleServer.StartServer(Convert.ToInt32(textBoxServerPort.Text));
        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                listBoxFilesToSend.Items.Add(dlg.FileName);
            }
        }

        private void buttonDelFile_Click(object sender, EventArgs e)
        {
            if(listBoxFilesToSend.SelectedIndex >= 0)
                listBoxFilesToSend.Items.RemoveAt(listBoxFilesToSend.SelectedIndex);
        }
        
        private void buttonSendFiles_Click(object sender, EventArgs e)
        {
            //присоединимся к серверу
            _tcpmoduleClient.IsCrypt = checkBoxIsCrypt.Checked;
            if (string.IsNullOrEmpty(textBoxPortDst.Text))
            {
                _tcpmoduleClient_ShowStatusMessage("Не указан порт назначения");
                return;
            }
            if (string.IsNullOrEmpty(textBox_IPAddressDst.Text))
            {
                _tcpmoduleClient_ShowStatusMessage("Не указан IP адрес назначения");
                return;
            }
            try
            {
                _tcpmoduleClient.ConnectClient(textBox_IPAddressDst.Text, Convert.ToInt32(textBoxPortDst.Text));
            }
            catch (Exception ex)
            {
                _tcpmoduleClient_ShowStatusMessage(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _tcpmoduleServer.StartServer(Convert.ToInt32(textBoxServerPort.Text));
        }

        private void buttonChangeRcvDirectory_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            textBoxFileRcvDirectory.Text = dlg.SelectedPath;
            _tcpmoduleServer.DstDirectory = textBoxFileRcvDirectory.Text;
        }
    }
}
