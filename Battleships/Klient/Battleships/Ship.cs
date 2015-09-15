using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    abstract class Ship
    {
        protected int size, posX, posY;
        protected bool isHorizontal;
        public int PosX
        {
            get { return posX; }
            set { value = posX; }
        }
        public int PosY
        {
            get { return posY; }
            set { value = posY; }
        }
        public Ship(int posX, int posY, int size, bool horizontal)
        {
            this.posX = posX;
            this.posY = posY;
            this.size = size;
            this.isHorizontal = horizontal;
        }
        public void DestroyShip()
        {
            // Destroy ship.
        }
    }
}
