using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipServer
{
    abstract class Ship
    {
        protected int size, posX, posY;
        protected bool isHorizontal;
        private Map map;
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
        public Ship(int posX, int posY, int size, bool horizontal, Map map)
        {
            this.posX = posX;
            this.posY = posY;
            this.size = size;
            this.isHorizontal = horizontal;
            this.map = map;
        }
        public void Destroy()
        {
            
        }
        public void DeployShip(int posX, int posY, bool horizontal)
        {
            /*if (isHorizontal)
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
            }*/

            // CHECK SURROUNDING TILES BEFORE DEPLOYING!!! 

            // Deploy ship.
            for (int i = 0; i < size; i++)
            { 
                int nextPosX = (isHorizontal == true ? posX + i : posX);
                int nextPosY = (isHorizontal == true ? posY : posY + i);

                map.OccupyTile(posX, posY);
                Draw();
            }
        }
        public void Draw()
        { 
            // Draw ship.
            for (int i = 0; i < size; i++)
            {
                int nextPosX = (isHorizontal == true ? posX + i : posX);
                int nextPosY = (isHorizontal == true ? posY : posY + i);

                map.MarkTile(nextPosX, nextPosY, ' ', ConsoleColor.White);
            }
        }
        public void Hide()
        {
            for (int i = 0; i < size; i++)
            {
                int nextPosX = (isHorizontal == true ? posX + i : posX);
                int nextPosY = (isHorizontal == true ? posY : posY + i);

                map.MarkTile(nextPosX, nextPosY, ' ', ConsoleColor.Blue);
            }
        }
        public void MoveShip(int x, int y)
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
        public void TurnShip()
        {
            if ((isHorizontal && PlaceFree_Y(size)) || (!isHorizontal && PlaceFree_X(size)))
            {
                Hide();
                isHorizontal = !isHorizontal;
                Draw();
            }
        }
        public bool PlaceFree_X(int dir)
        {
            int width = isHorizontal == true ? size : 0;
            // Check that the wanted position is within the boundaries of the map
            if ((dir > 0 && (posX + width) < map.Width) || (dir < 0 && posX > (map.PosX + dir)))
            {
                // Check that none of tiles are occupied
                for (int i = 0; i < size; i++)
                {
                    if(map.CheckTile(posX + dir + i, posY))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public bool PlaceFree_Y(int dir)
        {
            int height = isHorizontal == false ? size : 0;
            if ((dir > 0 && (posY + height) < map.Height) || (dir < 0 && posY > (map.PosY + dir)))
            {
                for (int i = 0; i < size; i++)
                {
                    if (map.CheckTile(posX, posY + dir + i))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
