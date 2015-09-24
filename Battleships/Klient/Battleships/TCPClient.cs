using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Battleships
{

    class TCPClient
    {
        private TcpClient _client;
        private GameWorld gw;
        private StreamWriter _sWriter;
        private bool _isConnected;
        private StreamReader _sReader;
        private static string sha256Calc;
        private string incomingData = null;
        private static string username;
        private static string[] letterArray;
        private static string[] numberArray;

        public TCPClient(string ipAddress, int portNum)
        {
            gw = new GameWorld();
            letterArray = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            numberArray = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            CalculateSHA256();
            _client = new TcpClient();
            _client.Connect(ipAddress, portNum);
            HandleCommunication();            
        }
        public void HandleCommunication()
        {
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);
            _sReader = new StreamReader(_client.GetStream(), Encoding.UTF8);
            _isConnected = true;
            string sData = null;
            sData = sha256Calc;
            Console.WriteLine(sha256Calc);
            string encrypted = CipherUtility.Encrypt<AesManaged>(sData, "password", "salt");
            _sWriter.WriteLine(encrypted);
            _sWriter.Flush();
            Thread readThread = new Thread(Read);
            readThread.Start();
            sData = Console.ReadLine(); // Username
            username = sData;
            encrypted = CipherUtility.Encrypt<AesManaged>(sData, "password", "salt");
            _sWriter.WriteLine(encrypted);
            try
            {
                _sWriter.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            while (_isConnected)
            {
               /* string md5Encrypt = CipherUtility.Encrypt<AesManaged>(sha256Calc, "password", "salt");
                _sWriter.WriteLine(md5Encrypt);
                _sWriter.Flush();*/
                if (gw.ShipsPlaced)
                {
                    sData = Console.ReadLine();


                    if (Program.Matched)
                    {
                        if (sData.Length > 1)
                        {
                            string letter = sData.Remove(1);
                            string number = sData.Substring(1);

                            if (letterArray.Contains(letter) && numberArray.Contains(number))
                            {
                                encrypted = CipherUtility.Encrypt<AesManaged>(sData, "password", "salt");
                                _sWriter.WriteLine(encrypted);
                                if (!Program.Matched)
                                {
                                    Console.WriteLine("Waiting for opponent..");
                                }

                                try
                                {
                                    _sWriter.Flush();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong input format! Enter coordinates in this format 'a5', 'c3' etc.");
                            }
                        }
                    }
                }

            }
        }

        public void Read()
        {
            string incomingData = null;

            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);

            while (_isConnected)
            {
                try
                {
                    incomingData = _sReader.ReadLine();
                    string decrypted = CipherUtility.Decrypt<AesManaged>(incomingData, "password", "salt");
                    if (decrypted == "Matched")
                    {
                        Console.Clear();
                        Console.SetCursorPosition(40, 40);
                        Program.Matched = true;
                        Console.Clear();
                        gw.GameSetup();
                        gw.Play();
                        string sData = CipherUtility.Encrypt<AesManaged>(gw.YourMap.ReadMap(), "password", "salt");
                        _sWriter.WriteLine(sData);
                        _sWriter.Flush();
                    }
                    else
                    {
                        string tileShot = decrypted.Substring(decrypted.Length - 2);
                        int posY = 0;
                        #region Switch
                        switch (tileShot.Remove(1))
                        {
                            case "a":
                                posY = 0;
                                break;
                            case "b":
                                posY = 1;
                                break;
                            case "c":
                                posY = 2;
                                break;
                            case "d":
                                posY = 3;
                                break;
                            case "e":
                                posY = 4;
                                break;
                            case "f":
                                posY = 5;
                                break;
                            case "g":
                                posY = 6;
                                break;
                            case "h":
                                posY = 7;
                                break;
                            case "i":
                                posY = 8;
                                break;
                            case "j":
                                posY = 9;
                                break;
                        }
                        #endregion

                        if (decrypted.Contains("missed"))
                        {
                            if(decrypted.Contains(username))
                            {
                                gw.EnemyMap.MarkTile(int.Parse(tileShot.Substring(1)),posY,'M', ConsoleColor.Red);
                            }
                            else
                            {
                                gw.YourMap.MarkTile(int.Parse(tileShot.Substring(1)), posY, 'M', ConsoleColor.Green);
                            }
                        }
                        else if(decrypted.Contains("hit"))
                        {
                            if (decrypted.Contains(username))
                            {
                                gw.EnemyMap.MarkTile(int.Parse(tileShot.Substring(1)), posY, 'H', ConsoleColor.Green);
                            }
                            else
                            {
                                gw.YourMap.MarkTile(int.Parse(tileShot.Substring(1)), posY, 'H', ConsoleColor.Red);
                            }
                        }

                        Console.WriteLine(decrypted);
                        ClearCurrentConsoleLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n"+e.Message);
                    Console.ReadLine();
                }
            }
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void CalculateSHA256()
        {
            SHA256Managed sha256 = new SHA256Managed();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.IO.FileStream stream = new System.IO.FileStream(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            sha256.ComputeHash(stream);

            stream.Close();

            System.Text.StringBuilder sha256Calculated = new System.Text.StringBuilder();
            for (int i = 0; i < sha256.Hash.Length; i++)
            {
                sha256Calculated.Append(sha256.Hash[i].ToString("x2"));
            }

            //Console.WriteLine(sha256Calculated);
            sha256Calc = sha256Calculated.ToString();

        }
    }
}
