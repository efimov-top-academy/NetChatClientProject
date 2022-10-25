using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Reflection.Metadata;

namespace NetChatClientProject
{
    internal class Program
    {
        static int port = 10001;
        static string name;
        static TcpClient client;
        static NetworkStream stream;

        string addressServer = "192.168.0.48";
        static IPAddress address = IPAddress.Loopback;

        static void SendMessage()
        {
            while(true)
            {
                Console.Write("Imput message: ");
                string message = Console.ReadLine()!;

                byte[] buffer = Encoding.Default.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);

                if (message?.IndexOf("quit") != -1)
                    break;
            }
        }

        static void ReceiveMessage()
        {
            while(true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    StringBuilder stringBuilder = new StringBuilder();
                    int bufferSize = 0;

                    do
                    {
                        bufferSize = stream.Read(buffer, 0, buffer.Length);
                        stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bufferSize));
                    } while (stream.DataAvailable);

                    Console.WriteLine($">>> {DateTime.Now.ToShortTimeString()} > {stringBuilder.ToString()}");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Disconnect from server");
                    Close();
                }
            }
        }

        static void Close()
        {
            if (stream is not null)
                stream.Close();
            if (client is not null)
                client.Close();
        }

        static void Main(string[] args)
        {
            Console.Write("Input your name: ");
            name = Console.ReadLine()!;
            client = new();

            try
            {
                client.Connect(address, port);
                stream = client.GetStream();

                string message = name;
                byte[] buffer = Encoding.Default.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);

                Thread receiveThread = new(ReceiveMessage);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                SendMessage();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Close();
            }
        }
    }
}