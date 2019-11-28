
using System;

namespace cbgb.Model
{
    class Item
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Webaddress { get; set; }
        public string ImageAddress { get; set; }
        public int StackSize { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsInSheet { get; set; }
        public string Sheet { get; set; }

        public Item()
        {
        }

        public Item(int id,int quantity)
        {
            Id = id;
            Quantity = quantity;
        }
    }
}
