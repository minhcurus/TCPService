using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ClientApp
{
    class Program
    {

        static void ConnectServer(String server, int port)
        {
            try
            {
                Console.Title = "Client Application";
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = null;

                Thread thread = new Thread(() =>
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            stream = client.GetStream();
                            int byteRead = stream.Read(buffer, 0, buffer.Length);
                            if (byteRead == 0) break;

                            //tra chuoi
                            string data = Encoding.ASCII.GetString(buffer, 0, byteRead);
                            Console.WriteLine(data);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while reading from server: " + e.Message);
                    }

                });
                thread.Start();


                //nhap ten
                Console.Write("What's your name?");
                string name = Console.ReadLine();

                //nhap tin nhan
                Console.WriteLine("Type message or type 'upload_file:<FileName>':");
                while (true)
                {
                    string message = Console.ReadLine();

                    string file = "";
                    if(message.Contains("upload_file:"))
                    {
                        string fileName = message.Substring(12);
                        if (File.Exists(fileName))
                        {
                           file = File.ReadAllText(fileName);
                            Console.WriteLine($"Sent file: {fileName}");
                        }
                        else
                        {
                            Console.WriteLine($"File {fileName} not found!");
                            break;
                        }
                    }
                    //gộp lại thành data
                    string combine = name + "*" + message + "*" + file;
                    byte[] data = Encoding.ASCII.GetBytes(combine);
                    stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            int port = 8080;
            ConnectServer(server, port);
        }
    }
}
