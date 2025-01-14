
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

        static void Broadcast(string msg, TcpClient sender) // phat lai thong bao
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
                Console.WriteLine($"Exception: {e.Message}");
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
            try
            {
                TcpClient client = parm as TcpClient;
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    //tra chuoi
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    string[] part = receivedData.Split('*');
                    string name = part[0];
                    string message = part[1];
                    string file = part[2];

                    //luu duoi dinh dang YYYYMMddHHmmss.txt
                    
                    if (file != "")
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        File.WriteAllText(fileName, file);
                        Console.WriteLine($"File received and saved as {fileName}");
                    }

                    //in thong bao
                    Console.WriteLine($"Received message from {name} at {DateTime.Now:t}: {message}");
                    Broadcast(name + ": " + message, client);
                }
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
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
