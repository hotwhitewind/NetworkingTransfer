using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace NetworkingTransfer
{
    /// <summary>
    /// Класс для передачи десериализированного контейнера при 
    /// возникновении события получения сетевых данных.
    /// </summary>
    internal class ReceiveEventArgs : EventArgs
    {
        private readonly SendInfo _sendinfo;

        public ReceiveEventArgs(SendInfo sendinfo)
        {
            _sendinfo = sendinfo;
        }

        public SendInfo SendInfo
        {
            get { return _sendinfo; }
        }

    }


    /// <summary>
    /// Класс способный выступать в роли сервера или клиента в TCP соединении.
    /// Отправляет и получает по сети файлы и текстовые сообщения.
    /// </summary>
    internal class TcpModule
    {

        #region Определение событий сетевого модуля

        // Типы делегатов для обработки событий в паре с соответствующим объектом события.

        // Обработчики события акцептирования (принятия клиентов) прослушивающим сокетом
        public delegate void AcceptEventHandler(object sender);

        public event AcceptEventHandler Accept;

        // Обработчики события подключения клиента к серверу
        public delegate void ConnectedEventHandler(object sender, string result);

        public event ConnectedEventHandler Connected;

        // Обработчики события отключения конечных точек (клиентов или сервера)
        public delegate void DisconnectedEventHandler(object sender, string result);

        public event DisconnectedEventHandler Disconnected;

        // Обработчики события извлечения данных 
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);

        public event ReceiveEventHandler Receive;

        public delegate void ShowStatusMessageHandler(string message);

        public event ShowStatusMessageHandler ShowStatusMessage;

        #endregion


        #region Объявления членов класса

        // Прослушивающий сокет для работы модуля в режиме сервера TCP
        private TcpListener _tcpListener;

        // Удобный контейнер для подключенного клиента.
        private TcpClientData _tcpClient;

        /// <summary>
        /// Возможные режимы работы TCP модуля
        /// </summary>
        public enum Mode
        {
            Indeterminately,
            Server,
            Client
        };

        /// <summary>
        /// Режим работы TCP модуля
        /// </summary>
        public Mode ModeNetwork;

        public DiffieHellmanWorker DiffieHellman;

        public bool IsCrypt { set; get; }

        public string DstDirectory { set; get; }

        private readonly CipherEngine _engine;

        public TcpModule()
        {
            _engine = new CipherEngine(new Kuznyechik());
        }

        #endregion


        #region Интерфейсная часть TCP модуля

        /// <summary>
        /// Запускает сервер, прослушивающий все IP адреса, и одновременно
        /// метод асинхронного принятия (акцептирования) клиентов.
        /// </summary>
        public void StartServer(int port)
        {
            if (ModeNetwork == Mode.Indeterminately)
            {
                try
                {
                    _tcpListener = new TcpListener(IPAddress.Any, port);
                    _tcpListener.Start();
                    _tcpListener.BeginAcceptTcpClient(AcceptCallback, _tcpListener);
                    ModeNetwork = Mode.Server;
                    if (ShowStatusMessage != null) ShowStatusMessage(string.Format("Сервер запущен! Порт {0}", port));
                }
                catch (Exception e)
                {
                    _tcpListener = null;
                    if (ShowStatusMessage != null) ShowStatusMessage(e.Message);
                }
            }
            else
            {
                SoundError();
            }
        }


        /// <summary>
        /// Остановка сервера
        /// </summary>
        public void StopServer()
        {
            if (ModeNetwork != Mode.Server) return;
            ModeNetwork = Mode.Indeterminately;
            _tcpListener.Stop();
            _tcpListener = null;


            DeleteClient(_tcpClient);
            if (ShowStatusMessage != null) ShowStatusMessage("Сервер остановлен!");


            //Parent.ChangeBackColor(Color.FromKnownColor(KnownColor.Control));
        }


        /// <summary>
        /// Попытка асинхронного подключения клиента к серверу
        /// </summary>
        /// <param name="ipserver">IP адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        public void ConnectClient(string ipserver, int port)
        {
            try
            {

                if (ModeNetwork == Mode.Indeterminately)
                {
                    _tcpClient = new TcpClientData();
                    _tcpClient.TcpClient.BeginConnect(IPAddress.Parse(ipserver), port, ConnectCallback, _tcpClient);

                    ModeNetwork = Mode.Client;
                }
                else
                {
                    SoundError();
                }
            }
            catch (Exception e)
            {
                if (ShowStatusMessage != null) ShowStatusMessage(e.Message);
            }
        }


        /// <summary>
        /// Отключение клиента от сервера
        /// </summary>
        public void DisconnectClient()
        {
            if (ModeNetwork != Mode.Client) return;
            ModeNetwork = Mode.Indeterminately;
            DeleteClient(_tcpClient);
        }


        /// <summary>
        /// Завершение работы подключенного клиента
        /// </summary>
        private static void DeleteClient(TcpClientData mtc)
        {
            if (mtc == null || mtc.TcpClient.Connected != true) return;
            mtc.TcpClient.GetStream().Close(); // по настоянию MSDN закрываем поток отдельно у клиента
            mtc.TcpClient.Close(); // затем закрываем самого клиента
        }
 

        /// <summary>
        /// Метод упрощенного создания заголовка с информацией о размере данных отправляемых по сети.
        /// </summary>
        /// <param name="length">длина данных подготовленных для отправки по сети</param>
        /// <returns>возращает байтовый массив заголовка</returns>
        private static byte[] GetHeader(int length)
        {
            //string header = length.ToString();
            //if (header.Length < 9)
            //{
            //    string zeros = null;
            //    for (int i = 0; i < (9 - header.Length); i++)
            //    {
            //        zeros += "0";
            //    }
            //    header = zeros + header;
            //}

            var byteheader = new byte[4];//Encoding.Default.GetBytes(header);

            byteheader[0] = (byte)((length & 0xff000000) >> 24);
            byteheader[1] = (byte)((length & 0xff0000) >> 16);
            byteheader[2] = (byte)((length & 0xff00) >> 8);
            byteheader[3] = (byte)(length & 0xff);
            return byteheader;
        }

        /// <summary>
        /// Метод получения длины данных из заголовка.
        /// </summary>
        /// <param name="byteheader">заголовок переданный по сети</param>
        /// <returns>возращает размер переданных данных</returns>

        private static int GetLengthByHeader(IList<byte> byteheader)
        {
            //string header = length.ToString();
            //if (header.Length < 9)
            //{
            //    string zeros = null;
            //    for (int i = 0; i < (9 - header.Length); i++)
            //    {
            //        zeros += "0";
            //    }
            //    header = zeros + header;
            //}

            var headersize = (byteheader[0] << 24) | (byteheader[1] << 16) | (byteheader[2] << 8) | (byteheader[3]);
            return headersize;
        }

        /// <summary>
        /// Функция для передачи списка файлов
        /// </summary>
        /// <param name="files">Список файлов</param>
        public void SendMultipleFiles(object files)
        {
            if (files == null) throw new ArgumentNullException("files");
            var listoffiles = (List<string>) files;
            foreach (var file in listoffiles)
            {
                //SendFileName = file;
                var param = new SendDataParam
                {
                    BufferSize = 0,
                    Filename = file,
                    IsFileCrypt = IsCrypt,
                    Message = null,
                    Rawbuffer = null
                };
                SendData(param);
            }
            DisconnectClient();
        }

        //public string SendFileName = null;
        /// <summary>
        /// Функция для передачи данных по сети
        /// </summary>
        /// <param name="senddataparam">Структура для параметров передаваемых данных</param>
        public void SendData(object senddataparam)
        {
            // Состав отсылаемого универсального сообщения
            // 1. Заголовок о следующим объектом класса подробной информации дальнейших байтов
            // 2. Объект класса подробной информации о следующих байтах
            // 3. Байты непосредственно готовых к записи в файл или для чего-то иного.


            var si = new SendInfo();
            var param = (SendDataParam) senddataparam;
            si.Message = param.Message;

            //  Если нет сообщения и отсылаемого файла продолжать процедуру отправки нет смысла.
            if (string.IsNullOrEmpty(si.Message) && string.IsNullOrEmpty(param.Filename)) return;

            if (param.Filename != null)
            {
                var fi = new FileInfo(param.Filename);
                if (fi.Exists)
                {
                    si.Filesize = (int)fi.Length;
                    si.Filename = fi.Name;
                }
            }

            if (param.BufferSize > 0 && param.Rawbuffer != null)
            {
                si.BufferSize = param.BufferSize;
            }


            var paddinForCrypt = 0;
            if (IsCrypt && si.Filesize > 0)
            {
                //посчитаем дополнение длины файла
                paddinForCrypt = si.Filesize%16;
                paddinForCrypt = 16 - paddinForCrypt;
                si.Padding = paddinForCrypt;
            }
            else
            {
                si.Padding = 0;
            }


            si.IsFileCrypt = IsCrypt;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, si);
            ms.Position = 0;
            var infobuffer = new byte[ms.Length];
            ms.Read(infobuffer, 0, infobuffer.Length);
            ms.Close();

            var header = GetHeader(infobuffer.Length);
            var total = new byte[header.Length + infobuffer.Length + si.Filesize + paddinForCrypt + si.BufferSize];

            Buffer.BlockCopy(header, 0, total, 0, header.Length);
            Buffer.BlockCopy(infobuffer, 0, total, header.Length, infobuffer.Length);

            // Если путь файла указан, добавим его содержимое в отправляемый массив байтов
            if (si.Filesize > 0)
            {
                var fs = new FileStream(param.Filename, FileMode.Open, FileAccess.Read);
                fs.Read(total, header.Length + infobuffer.Length, si.Filesize);
                fs.Close();
                if (IsCrypt)
                {
                    //шифруем
                    byte[] data;
                    var iv = new byte[16];
                    using (var msEncrypt = new MemoryStream())
                    {
                        var cipherStream = (CryptoStream)_engine.EncryptStream(msEncrypt, DiffieHellman.SecretKey, iv);
                        using (var plainTextStream = new MemoryStream(total, header.Length + infobuffer.Length, si.Filesize + paddinForCrypt))
                        {
                            plainTextStream.WriteTo(cipherStream);
                        }
                        data = msEncrypt.ToArray();
                    }
                    Buffer.BlockCopy(data, 0, total, header.Length + infobuffer.Length, si.Filesize + paddinForCrypt);
                }
            }

            if (si.BufferSize > 0)
            {
                Buffer.BlockCopy(param.Rawbuffer, 0, total, header.Length + infobuffer.Length + si.Filesize, si.BufferSize);
            }
            // Отправим данные подключенным клиентам
            var ns = _tcpClient.TcpClient.GetStream();
            // Так как данный метод вызывается в отдельном потоке рациональней использовать синхронный метод отправки
            ns.Write(total, 0, total.Length);
            
            // Обнулим все ссылки на многобайтные объекты и попробуем очистить память
            //SendFileName = null;
            //Parent.labelFileName.Text = "";
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Подтверждение успешной отправки
            //Parent.ShowReceiveMessage("Данные успешно отправлены!");
            if (ShowStatusMessage != null) ShowStatusMessage("Данные успешно отправлены!");
        }


        /// <summary>
        /// Универсальный метод останавливающий работу сервера и закрывающий все сокетыю
        /// вызывается в событии закрытия родительской формы.
        /// </summary>
        public void CloseSocket()
        {
            StopServer();
            DisconnectClient();
        }

        /// <summary>
        /// Звуковое сопровождение ошибок.
        /// </summary>
        private void SoundError()
        {
            Console.Beep(3000, 30);
            Console.Beep(1000, 30);
        }

        #endregion


        #region Асинхронные методы сетевой работы TCP модуля


        /// <summary>
        /// Обратный метод завершения принятия клиентов
        /// </summary>
        public void AcceptCallback(IAsyncResult ar)
        {
            if (ModeNetwork == Mode.Indeterminately) return;

            var listener = (TcpListener)ar.AsyncState;
            try
            {
                _tcpClient = new TcpClientData {TcpClient = listener.EndAcceptTcpClient(ar)};


                // Немедленно запускаем асинхронный метод извлечения сетевых данных
                // для акцептированного TCP клиента
                var ns = _tcpClient.TcpClient.GetStream();
                _tcpClient.Buffer = new byte[Global.Lengthheader];
                ns.BeginRead(_tcpClient.Buffer, 0, _tcpClient.Buffer.Length, ReadCallback, _tcpClient);


                // Продолжаем ждать запросы на подключение
                listener.BeginAcceptTcpClient(AcceptCallback, listener);

                // Активация события успешного подключения клиента
                if (Accept != null)
                {
                    Accept.BeginInvoke(this, null, null);
                }
            }
            catch
            {
                // Обработка исключительных ошибок возникших при акцептирования клиента.
                SoundError();
            }
        }


        /// <summary>
        /// Метод вызываемый при завершении попытки поключения клиента
        /// </summary>
        public void ConnectCallback(IAsyncResult ar)
        {
            var result = "Подключение успешно!";
            try
            {
                // Получаем подключенного клиента
                TcpClientData myTcpClient = (TcpClientData)ar.AsyncState;
                NetworkStream ns = myTcpClient.TcpClient.GetStream();
                myTcpClient.TcpClient.EndConnect(ar);

                // Запускаем асинхронный метод чтения сетевых данных для подключенного TCP клиента
                myTcpClient.Buffer = new byte[Global.Lengthheader];
                ns.BeginRead(myTcpClient.Buffer, 0, myTcpClient.Buffer.Length, ReadCallback, myTcpClient);
                //Parent.ChangeBackColor(Color.Goldenrod);

            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message);
                // Обработка ошибок подключения
                DisconnectClient();
                result = "Подключение провалено!";
                SoundError();
            }

            // Активация события успешного или неуспешного подключения к серверу,
            // здесь серверу можно отослать ознакомительные данные о себе (например, имя клиента)
            if (Connected != null)
                Connected.BeginInvoke(this, result, null, null);
        }


        /// <summary>
        /// Метод асинхронно вызываемый при наличие данных в буферах приема.
        /// </summary>

        public void ReadCallback(IAsyncResult ar)
        {
            if (ModeNetwork == Mode.Indeterminately) return;

            var myTcpClient = (TcpClientData) ar.AsyncState;

            try
            {
                var ns = myTcpClient.TcpClient.GetStream();

                var r = ns.EndRead(ar);

                if (r > 0)
                {
                    // Из главного заголовка получим размер массива байтов информационного объекта
                    var sendinfolen = GetLengthByHeader(myTcpClient.Buffer);
                    // Получим и десериализуем объект с подробной информацией о содержании получаемого сетевого пакета
                    var ms = new MemoryStream(sendinfolen);
                    var temp = new byte[sendinfolen];
                    r = ns.Read(temp, 0, temp.Length);
                    ms.Write(temp, 0, r);
                    var bf = new BinaryFormatter();
                    ms.Position = 0;
                    var sc = (SendInfo) bf.Deserialize(ms);

                    ms.Close();

                    if (sc.Filesize > 0)
                    {
                        var templength = sc.Filesize + sc.Padding;
                        // Создадим файл на основе полученной информации и массива байтов следующих за объектом информации
                        if (DstDirectory.LastIndexOf("\\", StringComparison.Ordinal) + 1 != DstDirectory.Length)
                            DstDirectory += "\\";
                        var fs = new FileStream(DstDirectory + sc.Filename, FileMode.Create, FileAccess.ReadWrite,
                            FileShare.ReadWrite, sc.Filesize);
                        //если файл шифрованный, то сначала проще собрать в памяти

                        var tempms = new MemoryStream();
                        temp = new byte[Global.Maxbuffer];
                        do
                        {
                            while (templength > 0)
                            {
                                if (templength < temp.Length)
                                {
                                    //считываем по длине файла
                                    r = ns.Read(temp, 0, templength);
                                    templength -= r;
                                }
                                else
                                {
                                    //считываем по длине буффера
                                    r = ns.Read(temp, 0, temp.Length);
                                    templength -= r;
                                }

                                // Записываем строго столько байтов сколько прочтено методом Read()
                                //fs.Write(temp, 0, r);
                                tempms.Write(temp, 0, r);
                            }
                            // Как только получены все байты файла, останавливаем цикл,
                            // иначе он заблокируется в ожидании новых сетевых данных
                            if (tempms.Length != sc.Filesize + sc.Padding) continue;
                            //теперь если файл шифрованный, то расшифруем его
                            tempms.Seek(0, SeekOrigin.Begin);

                            if (sc.IsFileCrypt)
                            {
                                var iv = new byte[16];
                                var cipherStream =
                                    (CryptoStream) _engine.DecryptStream(tempms, DiffieHellman.SecretKey, iv);
                                using (var plainTextStream = new BinaryReader(cipherStream))
                                {
                                    var data = plainTextStream.ReadBytes(sc.Filesize);
                                    fs.Write(data, 0, sc.Filesize);
                                }
                            }
                            else
                            {
                                //просто скопируем из памяти
                                tempms.CopyTo(fs);
                            }
                            tempms.Close();
                            fs.Close();
                            break;
                        } while (r > 0);

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    if (Receive != null)
                        Receive.BeginInvoke(this, new ReceiveEventArgs(sc), null, null);
                    myTcpClient.Buffer = new byte[Global.Lengthheader];
                    ns.BeginRead(myTcpClient.Buffer, 0, myTcpClient.Buffer.Length, ReadCallback,
                        myTcpClient);
                }
                else
                {
                    DeleteClient(myTcpClient);

                    // Событие клиент отключился
                    if (Disconnected != null)
                        Disconnected.BeginInvoke(this, "Клиент отключился!", null, null);
                }
            }
            catch (Exception)
            {

                DeleteClient(myTcpClient);

                // Событие клиент отключился
                if (Disconnected != null)
                    Disconnected.BeginInvoke(this, "Клиент отключился аварийно!", null, null);

                SoundError();

            }
        }


        #endregion

    }

    /// <summary>
    /// Класс для работы с протоколом Диффи-Хелмана
    /// </summary>
    internal class DiffieHellmanWorker
    {
        public byte[] PublicKey;
        public byte[] SecretKey;
        private readonly ECDiffieHellmanCng _kng;

        public DiffieHellmanWorker()
        {
            _kng = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256
            };
            PublicKey = _kng.PublicKey.ToByteArray();
        }

        /// <summary>
        /// Функция генерации секретного ключа
        /// </summary>
        /// <param name="publicKeyInBase64">Публичный ключ второй стороны в кодировке Base64</param>
        public void CreateSecretKey(string publicKeyInBase64)
        {
            var bytePublic = Convert.FromBase64String(publicKeyInBase64);
            SecretKey = _kng.DeriveKeyMaterial(CngKey.Import(bytePublic, CngKeyBlobFormat.EccPublicBlob));
        }
    }
    ///////////////////////////////////////////////////////////////////////////
    // ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ ДЛЯ ОРГАНИЗАЦИИ СЕТЕВОЙ РАБОТЫ TCP МОДУЛЯ
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Класс для организации непрерывного извлечения сетевых данных,
    /// для чего необходимо, как минимум, одновременно TcpClient
    /// и буфер приема.
    /// </summary>
    internal class TcpClientData
    {
        public TcpClient TcpClient = new TcpClient();

        // Буфер для чтения и записи данных сетевого потока
        public byte[] Buffer;

        public TcpClientData()
        {
            TcpClient.ReceiveBufferSize = Global.Maxbuffer;
        }
    }


    /// <summary>
    /// Класс для отправки текстового сообщения и 
    /// информации о пересылаемых байтах следующих последними в потоке сетевых данных.
    /// </summary>
    [Serializable]
    internal class SendInfo
    {
        public string Message;
        public string Filename;
        public int Filesize;
        public int Padding;
        public bool IsFileCrypt;
        public int BufferSize;
    }

    internal class SendDataParam
    {
        public string Message;
        public string Filename;
        public bool IsFileCrypt;
        public int BufferSize;
        public byte[] Rawbuffer;

        public SendDataParam()
        {
            Message = null;
            Filename = null;
            IsFileCrypt = false;
            BufferSize = 0;
            Rawbuffer = null;
        }
    }
    ///////////////////////////////////////////////////////////////////////////

}
