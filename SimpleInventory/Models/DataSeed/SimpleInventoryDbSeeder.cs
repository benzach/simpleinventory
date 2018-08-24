using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.Models.DataSeed
{
    public class SimpleInventoryDbSeeder
    {
        
        public static void SeedData(DbContext context)
        {
            string[,] states = new string[,] {
                { "ALABAMA", "AL" },
                {"ALASKA","AK" },
                {"ARIZONA", "AZ" },
                {"ARKANSAS","AR" },
                {"CALIFORNIA","CA" },
                {"COLORADO","CO" },
                {"CONNECTICUT","CT" },
                {"DELAWARE","DE" },
                {"FLORIDA","FL" },
                {"GEORGIA","GA" },
                {"HAWAII","HI" },
                {"IDAHO","ID" },
                {"ILLINOIS","IL" },
                {"INDIANA","IN" },
                {"IOWA","IA" },
                {"KANSAS","KS" },
                {"KENTUCKY","KY" },
                {"LOUISIANA","LA" },
                {"MAINE","ME" },
                {"MARYLAND","MD" },
                {"MASSACHUSETTS","MA" },
                {"MICHIGAN","MI" },
                {"MINNESOTA","MN" },
                {"MISSISSIPPI","MS" },
                {"MISSOURI","MO" },
                {"MONTANA","MT" },
                {"NEBRASKA","NE" },
                {"NEVADA","NV" },
                {"NEW HAMPSHIRE","NH" },
                {"NEW JERSEY","NJ" },
                {"NEW MEXICO","NM" },
                {"NEW YORK","NY" },
                {"NORTH CAROLINA","NC" },
                {"NORTH DAKOTA","ND" },
                {"OHIO","OH" },
                {"OKLAHOMA","OK" },
                {"OREGON","OR" },
                {"PENNSYLVANIA","PA" },
                {"RHODE ISLAND","RI" },
                {"SOUTH CAROLINA","SC" },
                {"SOUTH DAKOTA","SD" },
                {"TENNESSEE","TN" },
                {"TEXAS","TX" },
                {"UTAH","UT" },
                {"VERMONT","VT" },
                {"VIRGINIA","VA" },
                {"WASHINGTON","WA" },
                {"WEST VIRGINIA","WV" },
                {"WISCONSIN","WI" },
                {"WYOMING","WY" }};

            var simpleInventoryDbContext = context as SimpleInventoryContext;
            if(!simpleInventoryDbContext.Code_Sets.Any())
            {
                var codeset = new List<Code_Set> {
                    new Code_Set{ Name="States", Description="States"},
                    new Code_Set{ Name="Countries", Description="Countries"},

                };
                simpleInventoryDbContext.Code_Sets.AddRange(codeset);
                simpleInventoryDbContext.SaveChanges();

                var state = simpleInventoryDbContext.Code_Sets.SingleOrDefault(x => x.Name.ToLower() == "states");
                for(int i=0;i<states.GetUpperBound(0);i++)
                {
                    simpleInventoryDbContext.Code_Values.Add(new Code_Value { Name = states[i, 1], Description = states[i, 0], CodeSetId = state.Id });
                }
                simpleInventoryDbContext.SaveChanges();
            }

        }
    }
}
