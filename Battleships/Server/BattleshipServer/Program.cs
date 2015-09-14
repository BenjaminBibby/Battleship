using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace BattleshipServer
{
    class Program
    {
        private const int port = 11000;
        private static bool done = false;
        private static string udpIP;
        private static bool isRunning;
        private static TcpListener server;
        private static List<IPEndPoint> connectedUsers = new List<IPEndPoint>();
        private static object connectedUsersLock = new object();
        static void Main(string[] args)
        {
            Thread UDPthread = new Thread(UDPServer);
            UDPthread.Start();
            //UDPServer();
            TcpServer(port);
            //TCPServer();
            TCPClient("10.131.164.249","11000");
            //UDPServer();
            UDPServer();
            //TCPServer();

            Console.ReadLine();
        }
        static void TcpServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            LoopClient();
        }

        public static void LoopClient()
        {
            while (isRunning)
            {
                TcpClient newClient = server.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }
                
                IPAddress localAddress = IPAddress.Parse("10.131.74.125");
                int port = 11000;
                IPAddress localAddress = IPAddress.Parse("10.131.164.249");
                server = new TcpListener(localAddress, port);
                Byte[] bytes = new Byte[256];
                string data = null;

        public static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);

            string sData = null;
            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            IPEndPoint localEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
            Console.WriteLine(endPoint + " connected!");
            lock(connectedUsersLock)
            {
                connectedUsers.Add(endPoint);
            }
            Console.WriteLine("Connected users: {0}", connectedUsers.Count);

            while (client.Connected)
            {
                try
                {                   
                    sData = sReader.ReadLine();
                    Console.WriteLine(sData);
                }
                catch (Exception)
                {
                    Console.WriteLine(client.Client.RemoteEndPoint + " disconnected!");

                    lock (connectedUsersLock)
                    {
                        connectedUsers.Remove(endPoint);
                    } 
                    Console.WriteLine("Connected users: {0}", connectedUsers.Count);
                    Thread.CurrentThread.Abort();
                }
                //You could write something back to the client here.
                sWriter.WriteLine("Et eller andet hej eller whatever");
                sWriter.Flush();

            }
        }
        static void TCPClient()
        {
            string msg = "Connected!";
            
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
            while (true)
            {
                if (udpIP != null)
                {
                    try
                    {
                        TcpClient client = new TcpClient(udpIP, port);
                        byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine("Message sent: {0}", msg);
                        data = new byte[256];
                        string responseString = string.Empty;
                        //int bytes = stream.Read(data, 0, data.Length);
                        //responseString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        stream.Close();
                        client.Close();
                        Thread.CurrentThread.Abort();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("TCP Client: " + e.Message);
                    }     
                }
            }
        }
        static void UDPServer()
        {
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), port);

            while (!done)
            {
                Console.WriteLine("Venter på opkald..");
                byte[] bytes = listener.Receive(ref groupEP);
                udpIP = groupEP.Address.ToString();
                Console.WriteLine("Recieved broadcast from {0}: \n {1}\n", groupEP.ToString(), Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                Thread clientThread = new Thread(() => TCPClient());
                clientThread.Start();
            }
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
    }
}
