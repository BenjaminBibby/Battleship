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
    class GameWorld
    {
        public IPEndPoint playerOneEP;
        public IPEndPoint playerTwoEP;
        private bool playerOneTurn;
        Map playerOneMap = new Map(1, 1, 10, 10);
        Map playerTwoMap = new Map(13, 1, 10, 10);

        public bool PlayerOneTurn
        {
            get { return playerOneTurn; }
        }
        public GameWorld(IPEndPoint playerOneEP, IPEndPoint playerTwoEP)
        {
            playerOneTurn = true;
            this.playerOneEP = playerOneEP;
            this.playerTwoEP = playerTwoEP;
        }
        public void TurnMaster(IPEndPoint endPoint, string targetTile)
        {

            #region bogstavertiltal
            string letter = targetTile.Remove(1);
            string number = targetTile.Substring(1);
            int posY = 123;
            switch (letter)
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
            if (endPoint == playerOneEP)
            {
                if (!playerTwoMap.CheckTile(int.Parse(number), posY))
                {                    
                    string sData = CipherUtility.Encrypt<AesManaged>(Program.Usernames[playerOneEP]+" missed at position: " + letter + number , "password", "salt");
                    lock (Program.MsgsLock)
                    {
                        Program.Msgs.Add(Program.InfoSender[playerOneEP], sData);
                        Program.Msgs.Add(Program.InfoSender[playerTwoEP], sData);
                    }
                    playerOneTurn = false;
                }
            }
            else if (endPoint == playerTwoEP)
            {
                if (!playerOneMap.CheckTile(int.Parse(number), posY))
                {
                    string sData = CipherUtility.Encrypt<AesManaged>(Program.Usernames[playerTwoEP]+ " missed at position: " + letter + number , "password", "salt");
                    lock (Program.MsgsLock)
                    {
                        Program.Msgs.Add(Program.InfoSender[playerOneEP], sData);
                        Program.Msgs.Add(Program.InfoSender[playerTwoEP], sData);
                    }
                    playerOneTurn = true;
                }
                
            }
        }

    }
}
