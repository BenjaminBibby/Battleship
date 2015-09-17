using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class Frigate : Ship
    {
        public Frigate(int posX, int posY, bool horizontal, Map map)
            : base(posX, posY, 3, horizontal, map)
        { 
            
        }
    }
}
