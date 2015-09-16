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
        private StreamWriter _sWriter;
        private bool _isConnected;
        private StreamReader _sReader;
        private static string sha256Calc;
        private string incomingData = null;

        public TCPClient(string ipAddress, int portNum)
        {
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
            Thread readThread = new Thread(Read);
            readThread.Start();

            while (_isConnected)
            {
               /* string md5Encrypt = CipherUtility.Encrypt<AesManaged>(sha256Calc, "password", "salt");
                _sWriter.WriteLine(md5Encrypt);
                _sWriter.Flush();*/

                sData = Console.ReadLine();
                string encrypted = CipherUtility.Encrypt<AesManaged>(sData, "password", "salt");
                _sWriter.WriteLine(encrypted);
                _sWriter.Flush();
         
                try
                {
                    _sWriter.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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
                    Console.WriteLine(decrypted);
                }
                catch (Exception e)
                {
                    Console.WriteLine("/n"+e.Message);
                    Console.ReadLine();
                }
            }
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
