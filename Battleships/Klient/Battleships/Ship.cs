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
            if (isHorizontal)
            {
                if (((posX + size) > map.Width && posX < map.PosX) && (posY > map.Height && posY < map.PosY))
                {
                    return;
                }
            }
            else if (!isHorizontal)
            {
                if (((posX + size) > map.Width && posX < map.PosX) && (posY > map.Height && posY < map.PosY))
                {
                    return;
                }
            }
            // Deploy ship.
            for (int i = 0; i < size; i++)
            { 
                int nextPosX = (isHorizontal == true ? posX + i : posX);
                int nextPosY = (isHorizontal == true ? posY : posY + i);

                map.OccupyTile(posX, posY, this);
                Draw(map);
            }
        }
        public void Draw(Map map)
        { 
            // Draw ship.
            for (int i = 0; i < size; i++)
            {
                int nextPosX = (isHorizontal == true ? posX + i : posX);
                int nextPosY = (isHorizontal == true ? posY : posY + i);

                map.MarkTile(nextPosX, nextPosY, ' ', ConsoleColor.White);
            }
        }
        public void MoveShip(Map map, int x, int y)
        {
            if (isHorizontal)
            {
                if (((x + size) > map.Width && x < map.PosX) && (y > map.Height && y < map.PosY))
                {
                    return;
                }
            }
            else if(!isHorizontal)
            {
                if (((x + size) > map.Width && x < map.PosX) && (y > map.Height && y < map.PosY))
                {
                    return;
                }
            }
            posX = x;
            posY = y;
        }
    }
}
