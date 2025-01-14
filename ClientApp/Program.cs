using System.Net.Sockets;
using System.Text;

namespace ClientApp
{
    class Program
    {

        static void ConnectServer(String server, int port)
        {
            try
            {
                TcpClient client = new TcpClient(server, port);
                Console.Title = "Client Application";
                NetworkStream stream = client.GetStream();

                Console.Write("Input name:");
                string name = Console.ReadLine(); // tạo name 

                bool first = true;
                Thread thread = new Thread(() =>
                {
                    while (true)
                    {
                        byte[] buffer = new byte[256];
                        int byteRead = stream.Read(buffer, 0, buffer.Length);
                        string data = Encoding.ASCII.GetString(buffer, 0, byteRead);
                        if (first)
                        {
                            Console.Write($"{data}:");
                            first = false;
                        }
                        else
                        {
                            Console.WriteLine($"{data}");
                            first = true;
                        }
                    }
                });
                thread.Start();

                Console.WriteLine("Input message <press Enter to exit>:");

                while (true)
                {
                    string message = Console.ReadLine();
                    if (message == string.Empty)
                    {
                        break;
                    }

                    Byte[] msg = System.Text.Encoding.ASCII.GetBytes($"{message}");
                    Byte[] nme = System.Text.Encoding.ASCII.GetBytes($"{name}");

                    stream.Write(nme, 0, nme.Length);
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
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
