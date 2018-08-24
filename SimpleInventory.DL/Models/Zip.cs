using System;
using System.Text.RegularExpressions;

namespace SimpleInventory.DL.Models
{
    public class Zip
    {
        public string Value { get; private set; }
        internal Zip(string val)
        {
            Regex pattern = new Regex(@"^[0-9]{5}(?:-[0-9]{4})?$");
            var match = pattern.Match(val);
            if (!match.Success)
                throw new ArgumentException();
            Value = val;
        }
        public static implicit operator Zip(string val)=>new Zip(val);
    }
}