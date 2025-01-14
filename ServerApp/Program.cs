
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    class Program
    {
        static List<TcpClient> list = new List<TcpClient>();

        static void Broadcast(string msg, TcpClient sender)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            lock (list)
            {
                foreach (var client in list)
                {
                    if (client != sender)
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        static void ExecuteServer(string host, int port)
        {
            int Count = 0;
            TcpListener server = null;
            try
            {
                Console.Title = "Server Application";
                IPAddress localAddr = IPAddress.Parse(host);
                server = new TcpListener(localAddr, port);
                server.Start();
                Console.WriteLine(new string('*', 40));
                Console.WriteLine("Waiting for a connection...");
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    list.Add(client);
                    Console.WriteLine($"Number of clients connected: {++Count}");
                    Console.WriteLine(new string('*', 40));

                    Thread thread = new Thread(new ParameterizedThreadStart(ProcessMessage));
                    thread.Start(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server stopped. Press any key to exit! ");
            }
            Console.Read();
        }

        static void ProcessMessage(object parm)
        {
            string data;
            int count;
            bool first = true;

            try
            {
                TcpClient client = parm as TcpClient;
                Byte[] bytes = new Byte[1024];
                NetworkStream stream = client.GetStream();

                while ((count = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, count);
                    if (first)
                    {
                        Console.Write($"Received {data} at {DateTime.Now:t}: ");
                        Broadcast(data, client);
                        first = false;
                    }
                    else
                    {
                        Console.WriteLine($"{data}");
                        Broadcast(data, client);
                        first = true;
                    }
                }
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.Message);
                Console.WriteLine("Waiting...");
            }
        }
        static void Main()
        {
            string host = "127.0.0.1";
            int port = 8080;
            ExecuteServer(host, port);
        }
    }
}
