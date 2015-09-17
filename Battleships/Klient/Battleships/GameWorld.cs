using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class GameWorld
    {
        private bool turn, move;

        private List<Ship> ships;
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
        public void Play()
        {
            // Wait on pairings
            Ship[] ships = new Ship[5];
            bool placed = false;
            for (int i = 0; i < ships.Length; i++ )
            {
                placed = false;
                if (i < 2)
                {
                    ships[i] = new Minesweeper(1, 1, true, yourMap);
                }
                else if (i < 3)// Feel the magic
                {
                    ships[i] = new Frigate(1, 1, true, yourMap);
                }
                else if (i < 4)
                {
                    ships[i] = new Cruiser(1, 1, true, yourMap);
                }
                else if (i < 5)
                {
                    ships[i] = new Battleship(1, 1, true, yourMap);
                }
                ships[i].Draw();
                while (!placed)
                {
                    ConsoleKey key = (ConsoleKey)Console.ReadKey(true).Key;
                    placed = ships[i].MoveShip(key);
                }
                // Place ships            
            }
            foreach (Ship s in ships)
            {
                s.Draw();
            }
            // Send map to server
            while (true)
            {
                // Place coordinate on enemy map
                // Send point to server
                // Recieve hit
                // Recieve enemy hit
                // If enemy ships/your ships destroyed - Break
            }
        }

    }
}
