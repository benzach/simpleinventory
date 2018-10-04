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
        private readonly SimpleInventoryContext _simpleDBContext;
        public InventoryBusinessRepo(
            Func<Option<IEnumerable<Item>>> getallItems,
            Func<long,Option<Item>> getItemByID,
            Func<string,Option<IEnumerable<Item>>> getItemByName,
            Func<int,Option<IEnumerable<Code_Value>>> getAllItemCategoriesType,
            Func<Option<IEnumerable<Supplier>>> getAllSuppliers,
            SimpleInventoryContext simpledbcontext
            )
        {
            GetAllItems = getallItems;
            GetItemByID = getItemByID;
            GetItemByName = getItemByName;
            GetAllItemCategoriesType = getAllItemCategoriesType;
            GetAllSuppliers = getAllSuppliers;
            _simpleDBContext = simpledbcontext;
        }
        public Func<Repository<Option<InventoryItem>, string>> GetRepo => () =>
               {
                   //var res = GetAllItems();
                   //var itemRepo = GetAllItems().Match(
                   //    () => (new List<(Item, long)>()).ToRepository(),
                   //    lst => lst.Select(x => (x, x.Id)).ToRepository()
                   //    );
                   //var suppplierRepo = GetAllSuppliers().Match(
                   //    () => (new List<(Supplier, int)>()).ToRepository(),
                   //    lst => lst.Select(x => (x, x.Id)).ToRepository()
                   //    );
                   //var CategoriesRepo = GetAllItemCategoriesType(7).Match(
                   //    () => (new List<(Code_Value, int)>()).ToRepository(),
                   //    lst => lst.Select(x => (x, x.Id)).ToRepository()
                   //    );
                   var sink = new EFDataStoreSink<SimpleInventoryContext>(_simpleDBContext);
                   var itemRepo = Repository<Item, long>.ReconcileWithDataStore<SimpleInventoryContext>(sink, (db) => db.Items.Where(y=>y.Quantity>0).ToList().Select(x=>(val:x,key:x.Id)));
                   var suppplierRepo = Repository<Supplier, int>.ReconcileWithDataStore<SimpleInventoryContext>(sink, db => db.Suppliers.ToList().Select(x => (val: x, key: x.Id)));
                   var CategoriesRepo = Repository<Code_Value, int>.ReconcileWithDataStore<SimpleInventoryContext>(sink, db => db.Code_Values.Where(x => x.CodeSetId == 7).ToList().Select(y => (val: y, key: y.Id)));


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
