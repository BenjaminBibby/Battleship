﻿using System;
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
        #region Fields
        private const int port = 11000;
        private static bool done = false;
        private static string udpIP;
        private static bool isRunning;
        private static bool dataReceived;
        private static string sData;
        private static Dictionary<IPEndPoint, StreamWriter> infoSender = new Dictionary<IPEndPoint, StreamWriter>();
        private static TcpListener server;
        private static List<IPEndPoint> connectedUsers = new List<IPEndPoint>();

        public static List<IPEndPoint> ConnectedUsers
        {
            get { return Program.connectedUsers; }
            set { Program.connectedUsers = value; }
        }
        private static List<IPEndPoint> matchedUsers = new List<IPEndPoint>();

        public static List<IPEndPoint> MatchedUsers
        {
            get { return Program.matchedUsers; }
            set { Program.matchedUsers = value; }
        }
        private static object connectedUsersLock = new object();

        public static object ConnectedUsersLock
        {
            get { return Program.connectedUsersLock; }
            set { Program.connectedUsersLock = value; }
        }
        private static object msgsLock = new object();
        private static object usernameLock = new object();
        private static TcpClient client;
        private static List<GameWorld> gwList = new List<GameWorld>();
        #endregion

        #region Properties
        public static Dictionary<IPEndPoint, StreamWriter> InfoSender
        {
            get { return Program.infoSender; }
        }
        private static Dictionary<StreamWriter, string> msgs = new Dictionary<StreamWriter, string>();

        public static Dictionary<StreamWriter, string> Msgs
        {
            get { return Program.msgs; }
            set { Program.msgs = value; }
        }
        private static Dictionary<IPEndPoint, string> usernames = new Dictionary<IPEndPoint, string>();

        public static object MsgsLock
        {
            get { return Program.msgsLock; }
            set { Program.msgsLock = value; }
        }
        public static Dictionary<IPEndPoint, string> Usernames
        {
            get { return Program.usernames; }
        }
        #endregion

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

                    sData = CipherUtility.Encrypt<AesManaged>("Matched", "password", "salt");
                    lock (msgsLock)
                    {
                        msgs.Add(infoSender[connectedUsers[0]], sData);
                    }
                    if(connectedUsers.Count >= 2)
                    {
                        matchedUsers.Add(connectedUsers[1]);
                        Console.WriteLine("Added user: {0}", connectedUsers[1]);

                        sData = CipherUtility.Encrypt<AesManaged>("Matched", "password", "salt");
                        lock (msgsLock)
                        {
                            msgs.Add(infoSender[connectedUsers[1]], sData);
                        }
                        gwList.Add(new GameWorld(connectedUsers[0], connectedUsers[1]));
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
            lock (connectedUsersLock)
            {
                connectedUsers.Add(endPoint);
                Console.WriteLine("Queued users: {0}", connectedUsers.Count);
            }
            while (client.Connected)
            {
                try
                {
                    sData = sReader.ReadLine();
                    sData = CipherUtility.Decrypt<AesManaged>(sData, "password", "salt");
                    //Console.WriteLine("Encrypted data recieved: " + sData);
                    if (sData.Length > 2)
                    {
                        foreach (GameWorld gw in gwList)
                        {
                            if (gw.playerOneEP == endPoint || gw.playerTwoEP == endPoint)
                            {
                                gw.StringToMap(endPoint, sData);
                                
                            }
                        }
                    }
                    
                    #region
                    if (!dataReceived)
                    {
                        if (sData != "baae866ac1607d6fe25b3b57406d22d28e1bd2f9027daabfbbc44cf11e5e6050")
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
                    #endregion
                    Console.WriteLine("Data recieved and decrypted: " + sData);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(client.Client.RemoteEndPoint + " disconnected!");
                    UserDisconnected(endPoint);
                }

                if (matchedUsers.Contains(endPoint))
                {
                    try
                    {
                        foreach (GameWorld gw in gwList)
                        {
                            if (gw.playerOneEP == endPoint)
                            {
                                if (gw.PlayerOneTurn)
                                {
                                    gw.TurnMaster(endPoint,sData);
                                }
                                break;
                            }
                            else if (gw.playerTwoEP == endPoint)
                            {
                                if (!gw.PlayerOneTurn)
                                {
                                    gw.TurnMaster(endPoint,sData);
                                }
                                break;
                            }
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

