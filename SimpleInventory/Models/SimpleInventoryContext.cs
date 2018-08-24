using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.Models
{
    public class SimpleInventoryContext:DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Code_Set> Code_Sets { get; set; }
        public DbSet<Code_Value> Code_Values { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=localhost,1445,Database=SimpleInventoryDb,User Id=sa,Password=Password123!");

        //}
        public SimpleInventoryContext(DbContextOptions<SimpleInventoryContext> options) : base(options) { }
    }
}
