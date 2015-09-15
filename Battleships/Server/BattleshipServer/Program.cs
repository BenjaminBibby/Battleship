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
        private static bool dataReceived;
        private static TcpListener server;
        private static List<IPEndPoint> connectedUsers = new List<IPEndPoint>();
        private static List<IPEndPoint> matchedUsers = new List<IPEndPoint>();
        private static object connectedUsersLock = new object();
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Thread UDPthread = new Thread(UDPServer);
            UDPthread.Start();
            Thread TCPthread = new Thread(TcpServer);
            TCPthread.Start();
            Thread TCPclientThread = new Thread(TCPClient);
            TCPclientThread.Start();
            MatchMaking();

            Console.ReadLine();
        }


        static void MatchMaking()
        {
            while (true)
            {
                if (connectedUsers.Count >= 2)
                {
                    matchedUsers.Add(connectedUsers[0]);
                    Console.WriteLine("Added user: {0}", connectedUsers[0]);
                    matchedUsers.Add(connectedUsers[1]);
                    Console.WriteLine("Added user: {0}", connectedUsers[1]);
                    connectedUsers.RemoveRange(0, 2);
                }   
            }
        }
        static void TcpServer()
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
                IPAddress localAddress = IPAddress.Parse("10.131.74.125");
                Byte[] bytes = new Byte[256];
                string data = null;
                TcpClient newClient = server.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }
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
                    dataReceived = true;
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
                sWriter.WriteLine("Server received your message!");
                sWriter.Flush();

                if (matchedUsers.Count >= 2)
                {
                    try
                    {
                        server.Server.SendTo(Encoding.ASCII.GetBytes(sData), new IPEndPoint(LocateUser(), port));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        
        static void TCPClient()
        {
            string msg = "Connected!";
            
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
                        //Console.WriteLine("TCP Client: " + e.Message);
                    }     
                }
            }
        }
        static IPAddress LocateUser()
        {
            for (int i = 0; i < matchedUsers.Count; i++)
            {
                if(matchedUsers[i].Address.ToString() == udpIP)
                {
                    if(i%2 == 0)
                    {
                        return matchedUsers[i + 1].Address;
                    }
                    else
                    {
                        return matchedUsers[i - 1].Address;
                    }
                }
            }
            return matchedUsers[0].Address;
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
