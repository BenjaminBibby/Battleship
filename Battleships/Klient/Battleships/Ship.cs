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
        public void Destroy()
        {
            
        }
        public void DeployShip(Map map, int posX, int posY, bool horizontal)
        { 
            // Deploy ship.
        }
        public void Draw()
        { 
            // Draw ship.
            for (int i = 0; i < size; i++)
            {
                int nextPosX = (isHorizontal == true ? (posX + i) : posX);
                int nextPosY = (isHorizontal == true ? posY : (posY + i));

                Console.SetCursorPosition(nextPosX, nextPosY);
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(" ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
    }
}
