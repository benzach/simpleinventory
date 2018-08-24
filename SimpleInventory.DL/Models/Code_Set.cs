using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.DL.Models
{
    public class Code_Set
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Code_Value> Code_values { get; set; }
    }
}
