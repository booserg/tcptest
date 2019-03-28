using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.Common
{
    public class Server
    {
        private TcpListener server;

        public event EventHandler StatusChanged;
        private bool isWorking;
        public bool IsWorking
        {
            get
            {
                return isWorking;
            }
            private set
            {
                isWorking = value;
                StatusChanged?.Invoke(this, new EventArgs());
            }
        }

        List<ServerSession> sessions = new List<ServerSession>();
        public event EventHandler ConnectionQuantityChanged;
        public int ConnectionQuantity
        {
            get
            {
                return sessions.Count;
            }
        }

        private IPAddress ip;
        public string IP
        {
            get
            {
                return ip.ToString();
            }
            set
            {
                ip = IPAddress.Parse(value);
            }
        }

        private IRequestProcessor processor;

        public Server(IRequestProcessor processor)
        {
            IP = "127.0.0.1";
            this.processor = processor;
        }

        public async Task Start(int port)
        {
            server = new TcpListener(ip, port);
            server.Start();

            IsWorking = true;
            while (IsWorking)
            {
                try
                {
                    var client = await server.AcceptTcpClientAsync();
                    var session = new ServerSession(client, processor);
                    sessions.Add(session);
                    ConnectionQuantityChanged?.Invoke(this, new EventArgs());
                    session.Start();
                }
                catch
                {
                    IsWorking = false;
                }
            }
        }

        private void Session_ConnectionClosed(object sender, EventArgs e)
        {
            sessions.Remove(sender as ServerSession);
            ConnectionQuantityChanged?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            IsWorking = false;
            if (server != null)
            {
                try
                {
                    foreach (var session in sessions)
                        session.Stop();

                    server.Stop();
                }
                catch
                {
                    IsWorking = false;
                }
            }
        }
    }
}
