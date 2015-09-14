using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Battleships
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPClient();
            TCPServer();
            Console.ReadLine();
            TCPServer();
        }
        static void TCPServer()
        {
            TcpListener server = null;
            try
            {
                int port = 11000;
                IPAddress localAddress = IPAddress.Parse("192.168.43.168");
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
        static void UDPClient()
        {
            string ascii = "11000";
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress beaconIP = IPAddress.Parse("192.168.43.255");
            byte[] sendBuf = Encoding.ASCII.GetBytes(ascii);
            IPEndPoint ep = new IPEndPoint(beaconIP, 11000);
            socket.SendTo(sendBuf, ep);
        }

    }
}
