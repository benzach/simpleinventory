using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.Models
{
    public class Code_Value
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderNo { get; set; }
        public int CodeSetId { get; set; }
        public Code_Set CodeSet { get; set; }
    }
}
