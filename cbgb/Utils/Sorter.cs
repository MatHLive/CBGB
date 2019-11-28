using cbgb.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace cbgb.Utils
{
    class Sorter : IComparer<SheetItem>
    {
        public int Compare(SheetItem x, SheetItem y)
        {
           if (x.Row == 0 || y.Row == 0)
            {
                return 0;
            }
            return x.Row.CompareTo(y.Row);
        }
    }
}
