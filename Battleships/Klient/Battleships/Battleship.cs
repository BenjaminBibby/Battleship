using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class Battleship : Ship
    {
        public Battleship(int posX, int posY, int size, bool horizontal)
            : base(posX, posY, 5, horizontal)
        { 
            
        }
    }
}
