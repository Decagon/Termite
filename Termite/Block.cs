using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extract_Blocks
{
    class Block
    {

        public string id { get; set; }
        public string layer { get; set; }
        public string bmd { get; set; }
        public string shop_name { get; set; }
        public string description { get; set; }
        public string item_tab { get; set; }
        public bool bool_1 { get; set; } // not sure what this does
        public bool bool_2 { get; set; } // not sure what this does
        public string sprite_offset {get;set;} // (have to multiply by 16)
        public string block_package { get; set; }
        public string color { get; internal set; }
    }
}
