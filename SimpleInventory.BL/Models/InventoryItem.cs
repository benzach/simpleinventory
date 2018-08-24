using SimpleInventory.DL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleInventory.BL.Models
{
    public struct InventoryItem
    {
        public long Id { get;  }
        public string Name { get;  }
        public string Description { get; }
        //public int Category_Code_Value { get; set; }
        public Code_Value Category { get;}
        public int Quantity { get;  }
        public decimal PricePerUnit { get; }
        public Supplier Supplier { get; }
        public InventoryItem(long id,string name,string description,Code_Value category,int Qty,decimal priceperunit,Supplier supplier)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            PricePerUnit = priceperunit;
            Quantity = Qty;
            Supplier = supplier;
        }
    }
}
