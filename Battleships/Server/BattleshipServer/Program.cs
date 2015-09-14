using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace BattleshipServer
{
    class Program
    {
        private const int port = 11000;
        private static bool done = false;
        static void Main(string[] args)
        {
            TCPServer();
           // TCPClient("IP","PORT");
            UDPServer();
        }
        static void TCPServer()
        {
            TcpListener server = null;
            try
            {
                int port = 11000;
                IPAddress localAddress = IPAddress.Parse("10.131.66.252");
                server = new TcpListener(localAddress, port);
                Byte[] bytes = new Byte[256];
                string data = null;

                server.Start();
                //Console.WriteLine("Type 's' to start the TCPServer");
                while (true)
                {
                    Console.WriteLine("Waiting..");

                    TcpClient client = server.AcceptTcpClient(); //Three way handshake
                    Console.WriteLine("Connection established\nIP: {0}", client.Client.RemoteEndPoint.ToString());

                    data = null;
                    NetworkStream stream = client.GetStream();
                    int i = 0;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Data recieved: {0}", data);
                        //data = data.ToUpper();
                        //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                        //stream.Write(msg, 0, msg.Length);
                        //Console.WriteLine("Message sent: {0}", data);
                        //File.AppendAllText(@"C:\Temp\text.txt", Environment.NewLine + "Besked:" + data + " \\ IP:Port: " + client.Client.RemoteEndPoint.ToString() + " \\ Time when recieved:" + DateTime.Now.ToString());
                    }

                    client.Close();
                }
            }

            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
            Console.WriteLine("Press any key to close");
            Console.ReadLine();
        }
        static void TCPClient(string IP, string msg)
        {
            int port = 13000;
            TcpClient client = new TcpClient(IP, port);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Message sent: {0}", msg);
            data = new byte[256];
            string responseString = string.Empty;
            int bytes = stream.Read(data, 0, data.Length);
            responseString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            stream.Close();
            client.Close();
        }
        static void UDPServer()
        {
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
            while (!done)
            {
                Console.WriteLine("Venter på opkald..");
                byte[] bytes = listener.Receive(ref groupEP);
                Console.WriteLine("Recieved broadcast from {0}: \n {1}\n", groupEP.ToString(), Encoding.ASCII.GetString(bytes, 0, bytes.Length));

            }
        }
    }
}
