/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;

namespace ScriptEngine.HostedScript.Library.Net
{
    /// <summary>
    /// Простой однопоточный tcp-сокет. Слушает входящие соединения на определенном порту
    /// </summary>
    [ContextClass("TCPСервер", "TCPServer")]
    public class TCPServer : AutoContext<TCPServer>
    {
        private readonly TcpListener _listener;
        private Thread th;
        private string _Active = "none";
        private readonly Queue<TCPClient> _Conn = new Queue<TCPClient>();
        private bool _readheaders = false;

        public TCPServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Метод инициализирует TCP-сервер и подготавливает к приему входящих соединений
        /// </summary>
        [ContextMethod("Запустить", "Start")]
        public void Start()
        {
            _listener.Start();
        }

        /// <summary>
        /// Метод инициализирует TCP-сервер и подготавливает к приему входящих соединений в отдельном потоке
        /// </summary>
        [ContextMethod("ЗапуститьАсинхронно", "StartAsync")]
        public void StartAsync()
        {
            _listener.Start();
            th = new Thread(new ThreadStart(StartList));
            th.Start();
            while (_Active != "true")
            {
                Thread.Sleep(5);
            }
        }

        private void StartList()
        {
            _Active = "true";
            while (_Active == "true")
            {
                try
                {
                    var client = _listener.AcceptTcpClient();
                    var _client = new TCPClient(client);
                    _client.ReadHeaders = _readheaders;
                    _client.ReadBinaryDataAsync();
                    _Conn.Enqueue(_client);
                }
                catch
                {
                }
            }
            _Active = "none";
        }

        /// <summary>
        /// Останавливает прослушивание порта.
        /// </summary>
        [ContextMethod("Остановить", "Stop")]
        public void Stop()
        {
            _Active = "false";
            _listener.Stop();
        }

        /// <summary>
        /// Приостановить выполнение скрипта и ожидать соединений по сети.
        /// После получения соединения выполнение продолжается
        /// </summary>
        /// <param name="timeout">Значение таймаута в миллисекундах.</param>
        /// <returns>TCPСоединение. Объект, позволяющий обмениваться данными с удаленным хостом.</returns>
        [ContextMethod("ОжидатьСоединения","WaitForConnection")]
        public TCPClient WaitForConnection(int timeout = 0)
        {
            while (0 < timeout && !_listener.Pending())
            {
                Thread.Sleep(5);
                timeout -= 5;
            }

            if (!_listener.Pending())
                return null;

            var client = _listener.AcceptTcpClient();
            return new TCPClient(client);
        }

        [ContextMethod("ПолучитьСоединение", "GetConnection")]
        public TCPClient GetConnection(int timeout = 0)
        {
            while (5 < timeout && _Conn.Count == 0)
            {
                Thread.Sleep(5);
                timeout -= 5;
            }
            if (_Conn.Count != 0)
            {
                TCPClient val = _Conn.Dequeue();
                return val;
            }
            
            return null;
        }

        [ContextProperty("ПриниматьЗаголовки", "ReadHeaders")]
        public bool ReadHeaders
        {
            get { return _readheaders; }
            set { _readheaders = value; }
        }

        /// <summary>
        /// Создает новый сокет с привязкой к порту.
        /// </summary>
        /// <param name="port">Порт, который требуется слушать.</param>
        [ScriptConstructor]
        public static TCPServer ConstructByPort(IValue port)
        {
            return new TCPServer((int)port.AsNumber());
        }
    }
}
