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

        public bool PlayerOneTurn
        {
            get { return playerOneTurn; }
            set { playerOneTurn = value; }
        }
        public GameWorld(IPEndPoint playerOneEP, IPEndPoint playerTwoEP)
        {
            playerOneTurn = true;
            this.playerOneEP = playerOneEP;
            this.playerTwoEP = playerTwoEP;
            Map playerOneMap = new Map(1, 1, 10, 10);
            Map playerTwoMap = new Map(13, 1, 10, 10);
        }
        public void TurnMaster(IPEndPoint endPoint)
        {
            if (endPoint == playerOneEP)
            {
                playerOneTurn = false;
            }
            else if (endPoint == playerTwoEP)
            {
                playerOneTurn = true;
            }
        }

    }
}
