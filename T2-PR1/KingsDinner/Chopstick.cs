using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsDinner
{
    public class Chopstick
    {
        private readonly object lockObj = new object();
        public object Lock => lockObj;
    }
}
