using SimpleInventory.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.CodeSet
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static CategoryDTO ToDTO(Code_Set codeset)
        {
            return new CategoryDTO
            {
                Id = codeset.Id,
                Name = codeset.Name,
                Description = codeset.Description
            };
        }
    }
}
