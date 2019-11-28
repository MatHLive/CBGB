using System;
using System.Collections.Generic;
using System.Text;

namespace cbgb.Model
{
    class SheetItem : Item
    {
        public string Column { get; set; }
        public int Row { get; set; }
        public string Sheet { get; set; }
    }
}
