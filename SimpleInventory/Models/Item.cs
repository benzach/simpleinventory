using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.Models
{
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public int Category_Code_Value { get; set; }
        public Code_Value Category { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName="decimal(5,2)")]
        public decimal PricePerUnit { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
