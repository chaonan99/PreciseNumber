using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreciseNumber
{
    class AdvancedCalculate
    {
        public static PreciseNumber Func(int src)
        {
            PreciseNumber result = new PreciseNumber("1");
            for (int i = 1; i <= src; i++)
            {
                result = result * i;
            }
            return result;
        }
    }
}
