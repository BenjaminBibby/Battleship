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
            private ConsoleColor col = ConsoleColor.Blue;
            private char sprite;
            private bool occupied;
            public int posX, posY;
            public void SetCharacter(int x, int y, char c)
            {
                sprite = c;
                Console.BackgroundColor = col;  // Set background color to blue
                Console.SetCursorPosition(x, y);
                Console.Write(c);
                Console.BackgroundColor = ConsoleColor.Black;   // Reset background color to black
            }
        }
        private Tile[,] tiles;
        private int posX, posY, width, height;
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
                    tiles[j, i].SetCharacter(posX + j, posY + i, ' ');
                }
            }
        }
        public void DrawVerticalLetter(int posX, int posY, int length)
        {
            Char c = 'A';
            for (int i = 0; i < length; i++ )
            {
                Console.SetCursorPosition(posX, posY + i);
                Console.Write("(" + c +")");
                c++;
            }
        }
        public void DrawHorizontalDigits(int posX, int posY, int length)
        {
            Console.SetCursorPosition(posX, posY);
            for (int i = 1; i < length + 1; i++)
            {
                Console.Write("(" + i.ToString() + ")");
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
                    tiles[j, i].SetCharacter(posX + j, posY + i, ' ');
                }
            }
        }
        public void MarkTile(int posX, int posY, ConsoleColor col)
        {
            Console.SetCursorPosition(this.posX + posX, this.posY + posY);
            Console.BackgroundColor = col;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
