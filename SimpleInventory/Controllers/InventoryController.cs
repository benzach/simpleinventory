using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Functional.Lib.Functional;
//using SimpleInventory.Functional;
using SimpleInventory.DL.Models;
using SimpleInventory.BL;
using SimpleInventory.Utility;

namespace SimpleInventory.Controllers
{

    [Produces("application/json")]
    [Route("api/Inventory")]
    public class InventoryController : Controller
    {
        Func<Option<IEnumerable<Item>>> GetAllItems;
        Func<long,Option<Item>>GetItemByID;
        private Func<Repository<Item,long>> GetItemRepo;
        private InventoryBusinessRepo _InventoryRepo;
        public InventoryController(
            Func<Option<IEnumerable<Item>>> getallItems,
            Func<long,Option<Item>>getItembyID,
            Func<Repository<Item, long>> getitemRepo,
            InventoryBusinessRepo ivRepo)
        {
            GetAllItems = getallItems;
            GetItemByID = getItembyID;
            GetItemRepo = getitemRepo;
            _InventoryRepo = ivRepo;
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> Items()
        {
            //var t=await Task.Run(() => GetAllItems());
            //var o=t.Match(
            //    () => NoContent() as IActionResult, 
            //    res => Ok(res) as IActionResult);
            //return o;
            var t = await Task.Run(() => GetItemRepo());
            var rr=t.Bind(lst => lst.Select(v => { var cp = Util.Copy(v.Item1); cp.Quantity += 10; return (cp, cp.Id); }).ToBusinessRepoIII());
            var o = rr.Data.Select(x => x.Value());
            return Ok(o);
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> List()
        {
            var t = await Task.Run(() => _InventoryRepo.GetRepo());
            var r = t.Data.Select(x=>x.Value).SelectMany(x => x().AsEnumberable());
            return Ok(r);
        }
        [HttpGet]
        [Route("[Action]/{id}")]
        public async Task<IActionResult> Item(int id)
        {
            var t = await Task.Run(() => GetItemByID(id));
            var o = t.Match(
                () => NoContent() as IActionResult,
                res => Ok(res) as IActionResult);
            return o;
        }

    }
}