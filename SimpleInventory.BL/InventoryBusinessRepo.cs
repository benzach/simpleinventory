using Functional.Lib.Functional;
using SimpleInventory.BL.Models;
using SimpleInventory.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Functional.Lib.Functional.F;

namespace SimpleInventory.BL
{
    public class InventoryBusinessRepo
    {
        private Func<Option<IEnumerable<Item>>> GetAllItems;
        private Func<long, Option<Item>> GetItemByID;
        private Func<string, Option<IEnumerable<Item>>> GetItemByName;
        private Func<int,Option<IEnumerable<Code_Value>>> GetAllItemCategoriesType;
        private Func<Option<IEnumerable<Supplier>>> GetAllSuppliers;
        public InventoryBusinessRepo(
            Func<Option<IEnumerable<Item>>> getallItems,
            Func<long,Option<Item>> getItemByID,
            Func<string,Option<IEnumerable<Item>>> getItemByName,
            Func<int,Option<IEnumerable<Code_Value>>> getAllItemCategoriesType,
            Func<Option<IEnumerable<Supplier>>> getAllSuppliers
            )
        {
            GetAllItems = getallItems;
            GetItemByID = getItemByID;
            GetItemByName = getItemByName;
            GetAllItemCategoriesType = getAllItemCategoriesType;
            GetAllSuppliers = getAllSuppliers;
        }
        public Func<Repository<Option<InventoryItem>, string>> GetRepo => () =>
               {
                   //var res = GetAllItems();
                   var itemRepo = GetAllItems().Match(
                       () => (new List<(Item, long)>()).ToBusinessRepoIII(),
                       lst => lst.Select(x => (x, x.Id)).ToBusinessRepoIII()
                       );
                   var suppplierRepo = GetAllSuppliers().Match(
                       () => (new List<(Supplier, int)>()).ToBusinessRepoIII(),
                       lst => lst.Select(x => (x, x.Id)).ToBusinessRepoIII()
                       );
                   var CategoriesRepo = GetAllItemCategoriesType(7).Match(
                       () => (new List<(Code_Value, int)>()).ToBusinessRepoIII(),
                       lst => lst.Select(x => (x, x.Id)).ToBusinessRepoIII()
                       );
                   Func<Item, Code_Value, Supplier, Option<InventoryItem>> createInventory = (itm, cv, sup)
                     => itm.CategoryId == cv.Id && itm.SupplierId == sup.Id ?
                        Some(new InventoryItem(itm.Id, itm.Name, itm.Description, cv, itm.Quantity, itm.PricePerUnit, sup)) :
                        None;
                   Func<long, int, int, string> createKey = (itemid, cvid, supid)
                        => $"{itemid}-{cvid}-{supid}";
                   Repository<Func<Item, Code_Value, Supplier, Option<InventoryItem>>, Func<long, int, int, string>> CreateInventRepo =
                   (createInventory, createKey);

                   var resRepo = CreateInventRepo
                           .Apply(itemRepo)
                           .Apply(CategoriesRepo)
                           .Apply(suppplierRepo);

                   return resRepo;
               };
    }
}
