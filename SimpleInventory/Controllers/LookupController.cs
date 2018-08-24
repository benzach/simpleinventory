using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using SimpleInventory.BL;
using SimpleInventory.DL.Models;

namespace SimpleInventory.Controllers
{
    [Produces("application/json")]
    [Route("api/Lookup")]
    public class LookupController : Controller
    {
        private readonly SimpleInventoryContext _SimpleInventoryContext;
        private readonly IOptionsSnapshot<SimpleInventorySettings> _Settings;
        public LookupController(SimpleInventoryContext context,IOptionsSnapshot<SimpleInventorySettings>settings)
        {
            _SimpleInventoryContext = context;
            _Settings = settings;
            string url = _Settings.Value.ExternalCodeSetBaseUrl;
            //prevent context to track modified record
            ((DbContext)_SimpleInventoryContext).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult>CodeSets()
        {
            var items = await _SimpleInventoryContext.Code_Sets.ToListAsync();
            return Ok(items);
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult>CodeValues()
        {
            var items = await _SimpleInventoryContext.Code_Values.ToListAsync();
            return Ok(items);
        }
        [HttpGet]
        [Route("[Action]/{id}")]
        public async Task<IActionResult>Codeset(int id)
        {
            var item = await _SimpleInventoryContext.Code_Sets.SingleOrDefaultAsync(x => x.Id == id);
            var upitems = await _SimpleInventoryContext.Code_Values.Where(x => x.CodeSetId == id).ToListAsync();
            item.Code_values = new List<Code_Value>();
            item.Code_values.AddRange(upitems);
            return Ok(item);
        }
    }
}