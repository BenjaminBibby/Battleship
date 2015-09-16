using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class GameWorld
    {
        private List<Ship> ships;
        private bool turn;
        public GameWorld()
        {
            Map[] maps = new Map[2];
            maps[0] = new Map(1, 1, 10, 10);
            maps[1] = new Map(13, 1, 10, 10);
        }
        private void Update()
        {
            while (true)
            {
                if (turn)
                { 
                    
                }
            }
        }

    }
}
