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
            Server server = new Server(new ByteProcessor());
            await server.Start(2323);

            Console.ReadKey();
        }

        class ByteProcessor : IRequestProcessor
        {
            byte[] data = null;

            public byte[] ProcessRawRequest(string request)
            {
                if(data == null)
                    data = File.ReadAllBytes(@"C:\tmp\source\green\01.bmp");

                return data;
            }
        }

        //class StringProcessor : IRequestProcessor
        //{
        //    public string ProcessRequest(string request)
        //    {
        //        return "Hello";
        //    }
        //}
    }
}
