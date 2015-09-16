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
        private static string udpIP;
        private static int port = 11000;
        static void Main(string[] args)
        {
           // Map map = new Map(1, 1, 10, 10);
            Console.Title = "Client";
            UDPClient();
            TCPServer();
            TCPClient c = new TCPClient(udpIP, port);
            Console.ReadLine();
        }

        static void TCPServer()
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddress = IPAddress.Parse(GetLocalIPAddress());
                server = new TcpListener(localAddress, port);
                Byte[] bytes = new Byte[256];
                string data = null;

                server.Start();
                
                Console.WriteLine("Waiting..");

                TcpClient client = server.AcceptTcpClient(); //Three way handshake
                Console.WriteLine("Connection established\nIP: {0}", client.Client.RemoteEndPoint.ToString());
                udpIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                data = null;
                NetworkStream stream = client.GetStream();
                int i = 0;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Data recieved: {0}", data);
                }

                client.Close();                
            }

            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            finally
            {
                server.Stop();
            }
        }
        static void UDPClient()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress beaconIP = IPAddress.Parse(GetBroadcastIPAdress());
            byte[] sendBuf = Encoding.ASCII.GetBytes("tom besked");
            IPEndPoint ep = new IPEndPoint(beaconIP, port);
            socket.SendTo(sendBuf, ep);
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public static string GetBroadcastIPAdress()
        {
            string broadcastIp = "";
            string[] ip = GetLocalIPAddress().Split('.');

            for (int i = 0; i < ip.Length - 1; i++)
			{
                broadcastIp += ip[i] + ".";
			}

            broadcastIp += "255";        
            return broadcastIp;
        }

    }
}
