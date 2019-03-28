using Emulators.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Client client = new Client();
            client.Open("127.0.0.1", 2323);

            string request = "";
            while ((request = Console.ReadLine()) != "exit")
            {
                var response = client.SendText(request);
            }
        }
    }
}
