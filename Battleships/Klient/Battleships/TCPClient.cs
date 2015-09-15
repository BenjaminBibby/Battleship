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

    class TCPClient
    {
        private TcpClient _client;
        private StreamWriter _sWriter;
        private bool _isConnected;
        private StreamReader _sReader;

        public TCPClient(string ipAddress, int portNum)
        {
            _client = new TcpClient();
            _client.Connect(ipAddress, portNum);
            HandleCommunication();
        }
        public void HandleCommunication()
        {
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _isConnected = true;
            string sData = null;
            string incomingData = null;
            while (_isConnected)
            {
                Console.Write(">");
                sData = Console.ReadLine();
                _sWriter.WriteLine(sData);
                _sWriter.Flush();

                incomingData = _sReader.ReadLine();
                Console.WriteLine("Server: " + incomingData);
            }
        }
    }
}
