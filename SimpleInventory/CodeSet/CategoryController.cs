using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Functional.Lib.Functional;
using SimpleInventory.DL.Models;

namespace SimpleInventory.CodeSet
{
    [Produces("application/json")]
    [Route("api/Category")]
    public class CategoryController : Controller
    {
        Func<Option<IEnumerable<Code_Set>>> GetAllCodesets;
        Func<int, Option<Code_Set>> GetCodeSetByID;
        Func<NewCategoryDTO, Validation<Exceptional<Code_Set>>> SaveCodeSet;
        public CategoryController(
            Func<Option<IEnumerable<Code_Set>>> getallcodesets,
            Func<int,Option<Code_Set>> getCodesetbyID,
            Func<NewCategoryDTO,Validation<Exceptional<Code_Set>>> saveCodeset)
        {
            GetAllCodesets = getallcodesets;
            GetCodeSetByID = getCodesetbyID;
            SaveCodeSet = SaveCodeSet;
        }
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> List()
        {
            var t = await Task.Run(()=> GetAllCodesets());
            return t.Match(
                () => NoContent() as IActionResult,
                r => Ok(r.Select(x => CategoryDTO.ToDTO(x)) as IActionResult));

        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(NewCategoryDTO category)
        {
            var res = await Task.Run(()=>SaveCodeSet(category));
            var ret=res.Match(
                errs => BadRequest(string.Join(",", errs)) as IActionResult,
                possibleerr => possibleerr.Match(
                    er => StatusCode(500) as IActionResult,
                    val => Ok(CategoryDTO.ToDTO(val)) as IActionResult
                    )
                );
            return ret;
        }
    }
}