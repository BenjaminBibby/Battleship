using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class Map
    {
        class Tile
        {
            private char sprite;
            public bool occupied = false;
            public Ship occupiedShip;
            public int posX, posY;
            public void SetCharacter(char c, ConsoleColor col)
            {
                sprite = c;
                Console.BackgroundColor = col;  // Set background color to blue
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(posX, posY);
                Console.Write(c);
                Console.BackgroundColor = ConsoleColor.Black;   // Reset background color to black
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private Tile[,] tiles;
        private int posX, posY, width, height;
        public int PosX
        {
            get { return posX; }
            set { posX = value; }
        }
        public int PosY
        { 
            get { return posY; }
            set { posY = value; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        // Constructor
        public Map(int posX, int posY, int width, int height)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            tiles = new Tile[width, height];
            // Set position of all tiles in grid
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[j, i] = new Tile();
                    tiles[j, i].posX = (posX + j);
                    tiles[j, i].posY = (posY + i);
                }
            }
        }
        public void DrawVerticalLetter(int posX, int posY, int length)
        {
            Char c = 'A';
            for (int i = 0; i < length; i++ )
            {
                Console.SetCursorPosition(posX, posY + i);
                Console.Write(c);
                c++;
            }
        }
        public void DrawHorizontalDigits(int posX, int posY, int length)
        {
            Console.SetCursorPosition(posX, posY);
            for (int i = 1; i < length + 1; i++)
            {
                Console.Write(i.ToString());
            }
        }
        public void Draw()
        {
            DrawVerticalLetter(posX - 1, posY, height);
            DrawHorizontalDigits(posX, posY - 1, width);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[j, i].SetCharacter(' ', ConsoleColor.Blue);
                }
            }
        }
        public void MarkTile(int posX, int posY, char c, ConsoleColor col)
        {
            tiles[posX, posY].SetCharacter(c, col);
        }
        public void OccupyTile(int posX, int posY, Ship ship)
        {
            tiles[posX, posY].occupied = true;
            tiles[posX, posY].occupiedShip = ship;
        }
        public bool CheckTile(int posX, int posY)
        {
            return tiles[posX, posY].occupied;
        }
    }
}
