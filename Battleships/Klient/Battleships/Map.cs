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
        private int posX, posY;
        // Constructor
        public Map(int posX, int posY, int width, int height)
        {
            this.posX = posX;
            this.posY = posY;
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
            GetVerticalLetter(this.posX - 1, this.posY, height);
            GetHorizontalDigits(this.posX, this.posY - 1, width);
        }
        public void GetVerticalLetter(int posX, int posY, int length)
        {
            Char c = 'A';
            for (int i = 0; i < length; i++ )
            {
                Console.SetCursorPosition(posX, posY + i);
                Console.Write("(" + c +")");
                c++;
            }
        }
        public void GetHorizontalDigits(int posX, int posY, int length)
        {
            Console.SetCursorPosition(posX, posY);
            for (int i = 1; i < length + 1; i++)
            {
                Console.Write("(" + i.ToString() + ")");
            }
        }
    }
}
