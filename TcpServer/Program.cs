using Emulators.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Server server = new Server(new Processor());
            await server.Start(2323);

            Console.ReadKey();
        }

        class Processor : IRequestProcessor
        {
            public string ProcessRequest(string request)
            {
                var data = File.ReadAllBytes(@"C:\tmp\source\green\01.bmp");

                var str = Convert.ToBase64String(data) + "\r\n";

                return str;
            }
        }
    }
}
