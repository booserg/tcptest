using System;
using System.Net.Sockets;
using System.Text;

namespace Emulators.Common
{
    public class Client : StreamBase
    {
        private TcpClient client;
        private NetworkStream nwStream;

        public Client()
        {

        }

        public void Open(string hostname, int port)
        {
            this.client = new TcpClient();
            this.client.Connect(hostname, port);
            nwStream = client.GetStream();
        }

        public string SendText(string commandText)
        {
            try
            {
                var data = Encoding.ASCII.GetBytes(commandText);

                WriteStreamBytes(data, nwStream);

                var dt = DateTime.Now;
                data = ReadStreamBytes(nwStream, "client");
                Console.WriteLine("whole read time: " + (DateTime.Now - dt).TotalMilliseconds);

                var response = Encoding.ASCII.GetString(data);
                return response;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public void Close()
        {
            if (client != null && client.Connected)
            {
                client.Close();
            }
        }
    }
}
