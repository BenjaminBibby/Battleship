using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class Map
    {
        public class Tile
        {
            private bool occupied;
        }
        private Tile[,] tiles;
        private int posX, posY;
        public Map(int posX, int posY, int width, int height)
        {
            this.posX = posX;
            this.posY = posY;
            tiles = new Tile[width, height];
        }
    }
}
