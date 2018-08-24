using SimpleInventory.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.CodeSet
{
    public class NewCategoryDTO
    {        
        public string Name { get; set; }
        public string Description { get; set; }
        public Code_Set ToModel()
        {
            return new Code_Set
            {
                Name = this.Name,
                Description = this.Description
            };
        }
    }
}
