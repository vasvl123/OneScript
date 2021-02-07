/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.Net.Sockets;
using System.Text;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.IO;

using ScriptEngine.HostedScript.Library.Binary;

namespace ScriptEngine.HostedScript.Library.Net
{
    /// <summary>
    /// Соединение по протоколу TCP. Позволяет отправлять и принимать данные с использованием TCP сокета.
    /// </summary>
    [ContextClass("TCPСоединение","TCPConnection")]
    public class TCPClient : AutoContext<TCPClient>, IDisposable
    {
        private readonly TcpClient _client;

        private string status = "Готов";
        private const int BUFFERSIZE = 4096;
        private byte[] m_Buffer;
        private MemoryStream data = new MemoryStream();
        private MemoryStream headers = new MemoryStream();
        private bool _readheaders = false;


        public TCPClient(TcpClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Прочитать данные из сокета в виде строки.
        /// </summary>
        /// <param name="encoding">КодировкаТекста или Строка. Указывает в какой кодировке интерпретировать входящий поток байт.
        /// Значение по умолчанию: utf-8</param>
        /// <returns>Строка. Данные прочитанные из сокета</returns>
        [ContextMethod("ПрочитатьСтроку","ReadString")]
        public string ReadString(string encoding = null)
        {
            const int NO_LIMIT = 0;
            var memStream = ReadAllData(_client.GetStream(), NO_LIMIT);
            var enc = GetEncodingByName(encoding);
            if (memStream.Length == 0)
                return "";

            using(var reader = new StreamReader(memStream, enc))
            {
                return reader.ReadToEnd();
            }

        }

        /// <summary>
        /// Читает сырые байты из сокета.
        /// </summary>
        /// <param name="len">Количество байт, которые требуется прочитать. 0 - читать до конца потока.
        /// Значение по умолчанию: 0</param>
        /// <returns>ДвоичныеДанные</returns>
        [ContextMethod("ПрочитатьДвоичныеДанные", "ReadBinaryData")]
        public BinaryDataContext ReadBinaryData(int len = 0)
        {
            var stream = _client.GetStream();
            var ms = ReadAllData(stream, len);
            var data = ms.ToArray();

            return new BinaryDataContext(data);
        }

        private MemoryStream ReadAllData(NetworkStream source, int limit)
        {
            const int BUF_SIZE = 1024;
            byte[] readBuffer = new byte[BUF_SIZE];

            bool useLimit = limit > 0;
            var ms = new MemoryStream();

            do
            {
                int portion = useLimit ? Math.Min(limit, BUF_SIZE) : BUF_SIZE;
                int numberOfBytesRead = source.Read(readBuffer, 0, portion);
                ms.Write(readBuffer, 0, numberOfBytesRead);
                if (useLimit)
                    limit -= numberOfBytesRead;
            } while (source.DataAvailable);
            
            if(ms.Length > 0)
                ms.Position = 0;

            return ms;

        }

        /// <summary>
        /// Читает сырые байты из сокета асинхронно.
        /// </summary>
        [ContextMethod("ПрочитатьДвоичныеДанныеАсинхронно", "ReadBinaryDataAsync")]
        public void ReadBinaryDataAsync()
        {
            var stream = _client.GetStream();
            data = new MemoryStream();
            m_Buffer = new byte[BUFFERSIZE];
            status = "Занят";
            try
            {
                stream.BeginRead(m_Buffer, 0, m_Buffer.Length, new AsyncCallback(OnDataReceive), null);
            }
            catch
            {
                status = "Ошибка";
            }

        }

        private void OnDataReceive(IAsyncResult iar)
        {
            try
            {
                var stream = _client.GetStream();
                int ret = stream.EndRead(iar);
                if (ret > 0)
                {
                    int n = 0;
                    if (_readheaders)
                    {
                        for (n = 0;  n < ret; n++) {
                            if (m_Buffer[n] == 10)
                                if (n > 3)
                                    if (m_Buffer[n - 1] == 13 && m_Buffer[n - 2] == 10 && m_Buffer[n - 3] == 13)
                                    {
                                        headers.Write(m_Buffer, 0, n);
                                        _readheaders = false;
                                        if (n < ret) n++;
                                        break;
                                    }
                        }
                        status = "Заголовки";
                        System.Threading.Thread.Sleep(3);
                    }
                    data.Write(m_Buffer, n, ret - n);
                    if (stream.DataAvailable)
                    {
                        status = "Занят";
                        m_Buffer = new byte[BUFFERSIZE];
                        stream.BeginRead(m_Buffer, 0, m_Buffer.Length, new AsyncCallback(OnDataReceive), null);
                    }
                    else
                        status = "Данные";
                }
            }
            catch
            {
                status = "Ошибка";
            }

        }

        /// <summary>
        /// Возвращает полученные сырые байты из сокета.
        /// </summary>
        /// <returns>ДвоичныеДанные</returns>
        [ContextMethod("ПолучитьДвоичныеДанные", "GetBinaryData")]
        public BinaryDataContext GetBinaryData()
        {
            var a = data.ToArray();
            data = new MemoryStream();
            return new BinaryDataContext(a);
        }

        /// <summary>
        /// Прочитать данные из сокета в виде строки.
        /// </summary>
        /// <param name="encoding">КодировкаТекста или Строка. Указывает в какой кодировке интерпретировать входящий поток байт.
        /// Значение по умолчанию: utf-8</param>
        /// <returns>Строка. Данные прочитанные из сокета</returns>
        [ContextMethod("ПолучитьСтроку", "GetString")]
        public string GetString(string encoding = null)
        {
            var enc = GetEncodingByName(encoding);
            if (data.Length == 0)
                return "";

            data.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(data, enc))
            {              
                var str = reader.ReadToEnd();
                data = new MemoryStream();
                return str;
            }

        }


        [ContextMethod("ПолучитьЗаголовки", "GetHeaders")]
        public string GetHeaders(string encoding = null)
        {
            var enc = GetEncodingByName(encoding);
            if (headers.Length == 0)
                return "";

            headers.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(headers, enc))
            {
                var str = reader.ReadToEnd();
                headers = new MemoryStream();
                return str;
            }

        }


        /// <summary>
        /// Отправка строки на удаленный хост
        /// </summary>
        /// <param name="data">Строка. Данные для отправки</param>
        /// <param name="encoding">КодировкаТекста или Строка. Кодировка в которой нужно записать данные в поток. По умолчанию utf-8</param>
        [ContextMethod("ОтправитьСтроку","SendString")]
        public void SendString(string data, string encoding = null)
        {
            if(data == String.Empty)
                return;

            var enc = GetEncodingByName(encoding);
            byte[] bytes = enc.GetBytes(data);
            var stream = _client.GetStream();
            
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }

        /// <summary>
        /// Отправка сырых двоичных данных на удаленный хост.
        /// </summary>
        /// <param name="data">ДвоичныеДанные которые нужно отправить.</param>
        [ContextMethod("ОтправитьДвоичныеДанные", "SendBinaryData")]
        public void SendBinaryData(BinaryDataContext data)
        {
            if (data.Buffer.Length == 0)
                return;

            var stream = _client.GetStream();
            stream.Write(data.Buffer, 0, data.Buffer.Length);
            stream.Flush();

        }

        //// завершение асинхронной отправки
        private void OnWriteComplete(IAsyncResult ar)
        {
            try
            {
                var stream = _client.GetStream();
                stream.EndWrite(ar);
                status = "Успех";
            }
            catch
            {
                status = "Ошибка";
            }
        }

        /// <summary>
        /// Отправка сырых двоичных данных на удаленный хост асинхронно.
        /// </summary>
        /// <param name="data">ДвоичныеДанные которые нужно отправить.</param>
        [ContextMethod("ОтправитьДвоичныеДанныеАсинхронно", "SendBinaryDataAsync")]
        public void SendBinaryDataAsync(BinaryDataContext data)
        {
            if (data.Buffer.Length == 0)
                return;

            try
            {
                var stream = _client.GetStream();
                status = "Занят";
                stream.BeginWrite(data.Buffer, 0, data.Buffer.Length, new AsyncCallback(this.OnWriteComplete), null);
                //stream.Flush();
            }
            catch
            {
                status = "Ошибка";
            }

        }

        /// <summary>
        /// Состояние выполнения асинхронной операции.
        /// </summary>
        [ContextProperty("Статус", "Status")]
        public string Status
        {
            get {return status;}
        }

        /// <summary>
        /// Признак активности соединения.
        /// Данный признак не является надежным признаком существования соединения. 
        /// Он говорит лишь о том, что на момент получения значения данного свойства соединение было активно.
        /// </summary>
        [ContextProperty("Активно","IsActive")]
        public bool IsActive
        {
            get 
            {
                const int POLL_INTERVAL = 500;
                var socket = _client.Client;
                
                return !((socket.Poll(POLL_INTERVAL, SelectMode.SelectRead) && (socket.Available == 0)) || !socket.Connected);
            }
        }

        /// <summary>
        /// Таймаут, в течение которого система ожидает отправки данных. Если таймаут не установлен, то скрипт будет ждать начала отправки бесконечно.
        /// </summary>
        [ContextProperty("ТаймаутОтправки", "WriteTimeout")]
        public int WriteTimeout
        {
            get { return _client.GetStream().WriteTimeout; }
            set { _client.GetStream().WriteTimeout = value; }
        }

        /// <summary>
        /// Таймаут чтения данных. Если таймаут не установлен, то скрипт будет ждать начала приема данных бесконечно.
        /// </summary>
        [ContextProperty("ТаймаутЧтения", "ReadTimeout")]
        public int ReadTimeout
        {
            get { return _client.GetStream().ReadTimeout; }
            set { _client.GetStream().ReadTimeout = value; }
        }


        [ContextProperty("ПриниматьЗаголовоки", "ReadHeaders")]
        public bool ReadHeaders
        {
            get { return _readheaders; }
            set { _readheaders = value; }
        }


        private static Encoding GetEncodingByName(string encoding)
        {
            Encoding enc;
            if (encoding == null)
            {
                enc = new UTF8Encoding();
            }
            else
            {
                enc = Encoding.GetEncoding(encoding);
            }

            return enc;
        }

        /// <summary>
        /// Закрывает соединение с удаленным хостом.
        /// </summary>
        [ContextMethod("Закрыть","Close")]
        public void Close()
        {
            _client.Close();
        }

        /// <summary>
        /// Возвращает адрес удаленного узла соединения
        /// </summary>
        [ContextProperty("УдаленныйУзел", "RemoteEndPoint")]
        public string RemoteEndPoint => _client.Client.RemoteEndPoint.ToString();


        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Подключение к удаленному TCP-сокету
        /// </summary>
        /// <param name="host">адрес машины</param>
        /// <param name="port">порт сокета</param>
        [ScriptConstructor]
        public static TCPClient Constructor(IValue host, IValue port)
        {
            var client = new TcpClient(host.AsString(), (int)port.AsNumber());
            return new TCPClient(client);
        }
    }
}
