using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.DL.Models
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
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Code_Set>(entiybuilder => {
        //        entiybuilder.ToTable("Code_Set");
        //        entiybuilder.Property(c => c.Id)
        //                    .UseSqlServerIdentityColumn()
        //                    .IsRequired(true);
        //        entiybuilder.Property(c => c.Name)
        //                    .IsRequired(true)
        //                    .HasMaxLength(50);
        //        entiybuilder.Property(c => c.Description)
        //                    .IsRequired(false);
        //        entiybuilder.HasMany(c => c.Code_values)
        //                    .WithOne(cv => cv.CodeSet);
        //    });
        //    modelBuilder.Entity<Code_Value>(eb => {
        //        eb.ToTable("Code_Value");
        //        eb.Property(c => c.Id)
        //            .UseSqlServerIdentityColumn()
        //            .IsRequired(true);
        //        eb.Property(c => c.Name)
        //            .IsRequired(true)
        //            .HasMaxLength(50);
        //        eb.Property(c => c.OrderNo)
        //            .HasDefaultValue(0)
        //            .IsRequired(true);
        //        eb.Property(c => c.Description)
        //            .IsRequired(false)
        //            .HasMaxLength(300);
        //        eb.HasOne(c => c.CodeSet)
        //            .WithMany()
        //            .HasForeignKey(c => c.CodeSetId);
        //    });
        //    modelBuilder.Entity<Address>(eb=> {
        //        eb.ToTable("Address");
        //        eb.Property(c => c.Id)
        //            .UseSqlServerIdentityColumn()
        //            .IsRequired(true);
        //        eb.Property(c => c.Note)
        //            .IsRequired(false)
        //            .HasMaxLength(100);
        //        eb.Property(c => c.Address1)
        //            .IsRequired(true)
        //            .HasMaxLength(200);
        //        eb.Property(c => c.Address2)
        //            .IsRequired(false)
        //            .HasMaxLength(200);
        //        eb.Property(c => c.ContactPhone)
        //            .IsRequired(true)
        //            .HasMaxLength(10);
        //        eb.Property(c => c.Zip)
        //            .IsRequired(true)
        //            .HasMaxLength(10);
        //        eb.HasOne(c => c.State)
        //            .WithMany()
        //            .HasForeignKey(cv => cv.State_Code_Value);
        //        eb.HasOne(c => c.Country)
        //            .WithMany()
        //            .HasForeignKey(cv => cv.Country_Code_Value);                    
        //    });
        //    modelBuilder.Entity<Supplier>(eb => {
        //        eb.ToTable("Supplier");
        //        eb.Property(c => c.Id)
        //            .UseSqlServerIdentityColumn()
        //            .IsRequired(true);
        //        eb.Property(c => c.Name)
        //            .IsRequired(true)
        //            .HasMaxLength(50);
        //        eb.HasOne(c => c.Address)
        //            .WithMany()
        //            .HasForeignKey(s => s.AddressId);
        //    });
        //    modelBuilder.Entity<Item>(eb => {
        //        eb.ToTable("Item");
        //        eb.Property(c => c.Id)
        //            .UseSqlServerIdentityColumn()
        //            .IsRequired(true);
        //        eb.Property(c => c.Name)
        //            .IsRequired(true)
        //            .HasMaxLength(30);
        //        eb.Property(c => c.PricePerUnit)
        //            .HasDefaultValue(0.0);
        //        eb.Property(c => c.Quantity)
        //            .HasDefaultValue(0);
        //        eb.Property(c => c.Description)
        //            .HasMaxLength(100)
        //            .IsRequired(false);
        //        eb.HasOne(c => c.Category)
        //            .WithMany()
        //            .HasForeignKey(it => it.CategoryId);
        //        eb.HasOne(c => c.Supplier)
        //            .WithMany()
        //            .HasForeignKey(it => it.SupplierId);
        //    });
        //}
    }
}
