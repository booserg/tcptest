using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Emulators.Common
{
    public class StreamBase
    {
        protected int bufferSize = 33554432;
        protected int maxMessageLength = 33554432;
        protected string escapeChars = "\r\n";

        protected string ReadStream(NetworkStream stream, string name)
        {
            DateTime whole = DateTime.Now;

            DateTime dt = DateTime.Now;
            byte[] rawData = new byte[bufferSize];
            int bytesCnt;
            StringBuilder response = new StringBuilder();
            while (true)
            {
                if (stream.DataAvailable)
                {
                    Console.WriteLine("intitialization: " + (DateTime.Now - dt).TotalMilliseconds);

                    dt = DateTime.Now;
                    bytesCnt = stream.Read(rawData, 0, rawData.Length);
                    Console.WriteLine("read from stream: " + (DateTime.Now - dt).TotalMilliseconds);

                    if (bytesCnt > 0)
                    {
                        dt = DateTime.Now;
                        string chunk = Encoding.ASCII.GetString(rawData);
                        chunk = chunk.Substring(0, bytesCnt);
                        Console.WriteLine("byte to string: " + (DateTime.Now - dt).TotalMilliseconds);

                        dt = DateTime.Now;
                        var escapeIndex = chunk.LastIndexOf(escapeChars);
                        Console.WriteLine("look for escape: " + (DateTime.Now - dt).TotalMilliseconds);

                        if (escapeIndex > 0)
                        {
                            dt = DateTime.Now;
                            var lastChunk = chunk.Substring(0, escapeIndex);
                            Console.WriteLine("substring last chunk: " + (DateTime.Now - dt).TotalMilliseconds);

                            dt = DateTime.Now;
                            response.Append(lastChunk);
                            Console.WriteLine("append last chunk: " + (DateTime.Now - dt).TotalMilliseconds);
                            break;
                        }
                        else
                        {
                            dt = DateTime.Now;
                            response.Append(chunk);
                            Console.WriteLine("append chunk: " + (DateTime.Now - dt).TotalMilliseconds);
                            if (response.Length > maxMessageLength)
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }

                Thread.Sleep(10);
            }

            dt = DateTime.Now;
            var result = response.ToString();
            Console.WriteLine("conver result to string: " + (DateTime.Now - dt).TotalMilliseconds);
            Console.WriteLine("inner whole time: " + (DateTime.Now - whole).TotalMilliseconds);
            return result;
        }

        protected byte[] ReadStreamBytes(NetworkStream stream, string name)
        {
            DateTime whole = DateTime.Now;

            DateTime dt = DateTime.Now;
            byte[] rawData = new byte[bufferSize];
            int bytesCnt;

            int messageLengthRemains = 0;
            int messageLengthRead = 0;

            byte[] data = null;

            while (true)
            {
                if (stream.DataAvailable)
                {
                    Console.WriteLine("intitialization: " + (DateTime.Now - dt).TotalMilliseconds);

                    dt = DateTime.Now;
                    bytesCnt = stream.Read(rawData, 0, rawData.Length);
                    Console.WriteLine("read from stream: " + (DateTime.Now - dt).TotalMilliseconds);

                    if (bytesCnt > 0)
                    {
                        if (messageLengthRead == 0 && messageLengthRemains == 0)
                        {
                            messageLengthRemains = ConvertToInt(rawData);
                            data = new byte[messageLengthRemains];

                            rawData.Skip(8).Take(bytesCnt).ToArray().CopyTo(data, 0);
                            messageLengthRemains -= bytesCnt - 8;
                            messageLengthRead += bytesCnt - 8;
                        }
                        else if (messageLengthRemains == 0 && messageLengthRead > 0)
                        {
                            break;
                        }
                        else
                        {
                            rawData.CopyTo(data, messageLengthRead);
                        }
                        
                        //dt = DateTime.Now;
                        //string chunk = Encoding.ASCII.GetString(rawData);
                        //chunk = chunk.Substring(0, bytesCnt);
                        //Console.WriteLine("byte to string: " + (DateTime.Now - dt).TotalMilliseconds);

                        //dt = DateTime.Now;
                        //var escapeIndex = chunk.LastIndexOf(escapeChars);
                        //Console.WriteLine("look for escape: " + (DateTime.Now - dt).TotalMilliseconds);

                        //if (escapeIndex > 0)
                        //{
                        //    dt = DateTime.Now;
                        //    var lastChunk = chunk.Substring(0, escapeIndex);
                        //    Console.WriteLine("substring last chunk: " + (DateTime.Now - dt).TotalMilliseconds);

                        //    dt = DateTime.Now;
                        //    response.Append(lastChunk);
                        //    Console.WriteLine("append last chunk: " + (DateTime.Now - dt).TotalMilliseconds);
                        //    break;
                        //}
                        //else
                        //{
                        //    dt = DateTime.Now;
                        //    response.Append(chunk);
                        //    Console.WriteLine("append chunk: " + (DateTime.Now - dt).TotalMilliseconds);
                        //    if (response.Length > maxMessageLength)
                        //    {
                        //        throw new ArgumentOutOfRangeException();
                        //    }
                        //}
                    }
                }

                Thread.Sleep(10);
            }

            //dt = DateTime.Now;
            //var result = response.ToString();
            //Console.WriteLine("conver result to string: " + (DateTime.Now - dt).TotalMilliseconds);
            //Console.WriteLine("inner whole time: " + (DateTime.Now - whole).TotalMilliseconds);
            return data;
        }

        private Int32 ConvertToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        private byte[] ConvertToBytes(int size)
        {
            return BitConverter.GetBytes(size);
        }

        protected void WriteStreamBytes(byte[] message, NetworkStream stream)
        {
            byte[] lengthData = ConvertToBytes(message.Length);
            byte[] data = new byte[lengthData.Length + message.Length];

            lengthData.CopyTo(data, 0);
            message.CopyTo(data, lengthData.Length);

            stream.Write(data, 0, data.Length);
        }

        protected void WriteStream(string response, NetworkStream stream)
        {
            var answer = Encoding.ASCII.GetBytes(response + "\r\n");
            stream.Write(answer, 0, answer.Length);
        }

        protected void WriteStream(byte[] response, NetworkStream stream)
        {
            stream.Write(response, 0, response.Length);

            stream.Write(response, 0, response.Length);
        }
    }
}
