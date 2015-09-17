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
            if (PlaceFree_X(0) || PlaceFree_Y(0))
            {
                // Deploy ship.
                for (int i = 0; i < size; i++)
                {
                    int nextPosX = (isHorizontal == true ? posX + i : posX);
                    int nextPosY = (isHorizontal == true ? posY : posY + i);

                    map.OccupyTile(nextPosX, nextPosY, this);
                    Draw();
                    map.PrintAllOccupiedTiles();

                }
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
        public bool MoveShip(ConsoleKey key)
        {
            Console.SetCursorPosition(0, 14);
            
            int x = 0, y = 0;
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    {
                        x = 0;
                        y = 1;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    {
                        x = 0;
                        y = -1;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    {
                        x = -1;
                        y = 0;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    {
                        x = 1;
                        y = 0;
                    }
                    break;
                case ConsoleKey.Enter:
                    {
                        DeployShip(posX, posY, isHorizontal);
                        return true;
                    }
                case ConsoleKey.T:
                    {
                        TurnShip();
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
            if (PlaceFree_X(x) && PlaceFree_Y(y))
            {
                Hide();
                posX = posX + x;
                posY = posY + y;
                Draw();
                Console.SetCursorPosition(0, 14);
                Console.Write("[" + posX + "," + posY + "]\n" + "Size: " + size);
            }
            return false;
        }
        public void TurnShip()
        {
            if ((isHorizontal && PlaceFree_Y(size)) || (!isHorizontal && PlaceFree_X(size)))
            {
                Hide();
                isHorizontal = !isHorizontal;
                Draw();
                Console.SetCursorPosition(0, 14);
            }
        }
        public bool PlaceFree_X(int dir)
        {
            int width = isHorizontal == true ? size : 0;
            // Check that the wanted position is within the boundaries of the map
            if ((dir >= 0 && ((posX + width + dir) < (map.PosX + map.Width - 1))) || (dir <= 0 && ((posX + dir) >= map.PosX - 1)))
            {
                if (dir > 0)
                {
                // Check that none of tiles are occupied
                    for (int i = 0; i < size; i++)
                    {
                        if (map.CheckTile(posX + dir + i, posY))
                        {
                            return false;
                        }   
                    }
                }
                else if (dir < 0)
                {
                    if (map.CheckTile(posX + dir, posY))
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
            int height = isHorizontal == true ? 0 : size;
            if ((dir >= 0 && ((posY + height + dir) < (map.PosY + map.Height - 1))) || (dir <= 0 && ((posY + dir) >= map.PosY - 1)))
            {
                if (dir > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (map.CheckTile(posX, posY + dir + i))
                        {
                            return false;
                        }
                    }
                }
                else if (dir < 0)
                {
                    if (map.CheckTile(posX, posY + dir))
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
