﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    class Cruiser : Ship
    {
        public Cruiser(int posX, int posY, int size, bool horizontal)
            : base(posX, posY, 4, horizontal)
        { 
            
        }
    }
}