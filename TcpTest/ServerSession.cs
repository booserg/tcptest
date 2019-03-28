using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.Common
{
    internal class ServerSession : StreamBase
    {
        private TcpClient client { get; set; }
        private IRequestProcessor processor { get; set; }

        public ServerSession(TcpClient client, IRequestProcessor processor)
        {
            this.client = client;
            this.processor = processor;
        }

        public void Start()
        {
            var task = new Task(() =>
            {
                var stream = client.GetStream();

                try
                {
                    while (true)
                    {
                        var request = ReadRequest(stream);

                        var dt = DateTime.Now;
                        var response = processor.ProcessRequest(request);
                        WriteStream(response, stream);
                        Console.WriteLine((DateTime.Now - dt).TotalMilliseconds);
                    }
                }
                catch (Exception exc)
                {

                }
                finally
                {
                    client.Close();
                }
            });

            task.Start();
        }

        private string ReadRequest(NetworkStream stream)
        {
            String request = "";

            try
            {
                request = ReadStream(stream, "server");
            }
            catch
            {

            }

            return request;
        }

        public void Stop()
        {
            client.Close();
        }
    }
}
