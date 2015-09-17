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
            
            Map yourMap = new Map(1, 1, 10, 10);
            Map enemyMap = new Map(13, 1, 10, 10);
            yourMap.Draw();
            enemyMap.Draw();
            
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
