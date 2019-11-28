using System;
using System.Collections.Generic;
using System.Text;

namespace cbgb.Model
{
    class Range
    {
        public string Column { get; set; }
        public int MinRow { get; set; }
        public int MaxRow { get; set; }
        public List<SheetItem> Values { get; set; }
    }
}
