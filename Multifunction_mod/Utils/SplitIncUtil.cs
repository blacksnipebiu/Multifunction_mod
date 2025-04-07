using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multifunction_mod.Utils
{
    internal class SplitIncUtil
    {
        public static int split_inc(ref int itemcount, ref int iteminc, int splitcount)
        {
            if (itemcount == 0)
            {
                iteminc = 0;
                return 0;
            }
            int num = iteminc / itemcount;
            int num2 = iteminc - num * itemcount;
            itemcount -= splitcount;
            num2 -= itemcount;
            num = ((num2 > 0) ? (num * splitcount + num2) : (num * splitcount));
            iteminc -= num;
            return num;
        }
    }
}
