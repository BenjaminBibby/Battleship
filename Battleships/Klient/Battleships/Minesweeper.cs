using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class Minesweeper : Ship
    {
        public Minesweeper(int posX, int posY, bool horizontal)
            : base(posX, posY, 2, horizontal)
        { 
            
        }
    }
}
