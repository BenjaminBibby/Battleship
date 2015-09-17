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
        private Map map, enemyMap;
        public GameWorld()
        {
            this.map = new Map(1, 1, 10, 10);
            map.Draw();
            this.enemyMap = new Map(13, 1, 10, 10);
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
                    ships[i] = new Minesweeper(1, 1, true, map);
                }
                else if (i < 3)// Feel the magic
                {
                    ships[i] = new Frigate(1, 1, true, map);
                }
                else if (i < 4)
                {
                    ships[i] = new Cruiser(1, 1, true, map);
                }
                else if (i < 5)
                {
                    ships[i] = new Battleship(1, 1, true, map);
                }
                ships[i].Draw();
                while (!placed)
                {
                    ConsoleKey key = (ConsoleKey)Console.ReadKey(true).Key;
                    placed = ships[i].MoveShip(key);
                }
                // Place ships            
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
