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
        Map yourMap;
        Map enemyMap;
        public Map YourMap
        {
            get { return yourMap; }
            set { yourMap = value; }
        }
        public Map EnemyMap
        {
            get { return enemyMap; }
            set { enemyMap = value; }
        }
        public GameWorld()
        {
            Console.WriteLine("  Your map    Enemy map");
            yourMap = new Map(1, 2, 10, 10);
            enemyMap = new Map(13, 2, 10, 10);
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
