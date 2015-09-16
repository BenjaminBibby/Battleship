using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace BattleshipServer
{
    class Program
    {
        private const int port = 11000;
        private static bool done = false;
        private static string udpIP;
        private static bool isRunning;
        private static bool dataReceived;
        private static string sData;
        private static Dictionary<IPEndPoint, StreamWriter> infoSender = new Dictionary<IPEndPoint, StreamWriter>();
        private static Dictionary<StreamWriter, string> msgs = new Dictionary<StreamWriter, string>();
        private static Dictionary<IPEndPoint, string> usernames = new Dictionary<IPEndPoint, string>();
        private static TcpListener server;
        private static List<IPEndPoint> connectedUsers = new List<IPEndPoint>();
        private static List<IPEndPoint> matchedUsers = new List<IPEndPoint>();
        private static object connectedUsersLock = new object();
        private static object msgsLock = new object();
        private static object usernameLock = new object();
        private static TcpClient client;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            Thread MasterSenderThread = new Thread(MasterSender);
            MasterSenderThread.Start();
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

                    if(connectedUsers.Count >= 2)
                    {
                        matchedUsers.Add(connectedUsers[1]);
                        Console.WriteLine("Added user: {0}", connectedUsers[1]);
                        connectedUsers.RemoveRange(0, 2);
                    }
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
                //IPAddress localAddress = IPAddress.Parse("10.0.0.202");
                Byte[] bytes = new Byte[256];
                // string data = null;
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
            lock (connectedUsersLock)
            {
                connectedUsers.Add(endPoint);
            }
            Console.WriteLine("Queued users: {0}", connectedUsers.Count);
            infoSender.Add(endPoint, sWriter);

            string simon = "Enter username: ";
            string encryptedd = CipherUtility.Encrypt<AesManaged>(simon, "password", "salt");
            sWriter.WriteLine(encryptedd);
            sWriter.Flush();
            try
            {
                sData = sReader.ReadLine();
                string dedcrypt = CipherUtility.Decrypt<AesManaged>(sData, "password", "salt");
                usernames.Add(endPoint, dedcrypt);
            }
            catch (Exception e)
            {
                Console.WriteLine(client.Client.RemoteEndPoint + " disconnected!");
                UserDisconnected(endPoint);
            }

            while (client.Connected)
            {
                try
                {
                    sData = sReader.ReadLine();
                    //Console.WriteLine("Encrypted data recieved: " + sData);
                    string decrypted = CipherUtility.Decrypt<AesManaged>(sData, "password", "salt");
                    #region
                    //TIL SIDST I PROJEKTET SKAL DET VIRKE
                    /* if (!dataReceived)
                    {
                        if (decrypted != "f4cefed49fb2f58655cde4c216f4e52a1f2aaaea0b5809664a97f075026f92bc")
                        {
                            dataReceived = true;
                            Console.WriteLine("Client doesnt have the right MD5!");
                            string errorOne = "Your MD5 is not RIGHT!, disconnecting.";
                            string MD5Error = CipherUtility.Encrypt<AesManaged>(errorOne, "password", "salt");
                            sWriter.WriteLine(MD5Error);
                            sWriter.Flush();
                            client.Close();
                            break;
                        }
                    }
                    */
#endregion
                    Console.WriteLine("Data recieved and decrypted: " + decrypted);

                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    Console.WriteLine(client.Client.RemoteEndPoint + " disconnected!");
                    UserDisconnected(endPoint);
                }
                //You could write something back to the client here.
                //string msg = "Message";
                //string encrypted = CipherUtility.Encrypt<AesManaged>(msg, "password", "salt");
                ////Console.WriteLine("Encrypted data sent: " + encrypted);
                //sWriter.WriteLine(encrypted);
                //sWriter.Flush();

                if (matchedUsers.Contains(endPoint))
                {
                    try
                    {
                        IPEndPoint tmpLocateUser = LocateUser(endPoint);
                        sData = CipherUtility.Decrypt<AesManaged>(sData, "password", "salt");
                        sData = CipherUtility.Encrypt<AesManaged>(usernames[endPoint] + "> " + sData, "password", "salt");
                        lock (msgsLock)
                        {
                            msgs.Add(infoSender[tmpLocateUser], sData);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private static void MasterSender()
        {
            Dictionary<StreamWriter, string> tmp = new Dictionary<StreamWriter, string>();
            while (true)
            {
                lock (msgsLock)
                {
                    foreach (var item in msgs.Keys)
                    {
                        tmp.Add(item, msgs[item]);
                    }
                }

                foreach (var key in tmp.Keys)
                {
                    key.WriteLine(tmp[key]);
                    key.Flush();
                    lock (msgsLock)
                    {
                        msgs.Remove(key);
                    }
                }
                tmp.Clear();
            }

        }

        static void TCPClient()
        {
            string msg = "Connected";

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
                catch (Exception)
                {
                    //Console.WriteLine("TCP Client: " + e.Message);
                }
            }
        }                 
            
        static IPEndPoint LocateUser(IPEndPoint locateUserEP)
        {

            for (int i = 0; i < matchedUsers.Count; i++)
            {
                if (matchedUsers[i] == locateUserEP && matchedUsers.Count >= 2)
                {
                    if (i % 2 == 0)
                    {
                        return matchedUsers[i + 1];
                    }
                    else if (i % 2 == 1)
                    {
                        return matchedUsers[i - 1];
                    }
                }

            }

            return matchedUsers[0];

        }
                    
        static void UDPServer()
        {
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), port);

            while (!done)
            {
                Console.WriteLine("Awaiting call...");
                byte[] bytes = listener.Receive(ref groupEP);
                udpIP = groupEP.Address.ToString();
                Console.WriteLine("Recieved broadcast from {0}> {1}", groupEP.ToString(), Encoding.ASCII.GetString(bytes, 0, bytes.Length));
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

        public static void UserDisconnected(IPEndPoint endPoint)
        {
            lock (connectedUsersLock)
            {
                if (matchedUsers.Contains(endPoint))
                {
                    connectedUsers.Add(LocateUser(endPoint));
                    matchedUsers.Remove(LocateUser(endPoint));
                }
                lock (usernameLock)
                {
                    usernames.Remove(endPoint);
                }
                connectedUsers.Remove(endPoint);
                infoSender.Remove(endPoint);
                matchedUsers.Remove(endPoint);
            }

            Console.WriteLine("Queued users: {0}", connectedUsers.Count);
            Thread.CurrentThread.Abort();
        }
    }
}

