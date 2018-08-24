using System;
using System.Text.RegularExpressions;

namespace SimpleInventory.DL.Models
{
    public class Phone
    {
        public string Value { get; private set; }
        public Phone(string val)
        {
            Regex pattern = new Regex(@"\b(?:[0-9]{7}|[0-9]{10})\b");
            var match = pattern.Match(val);
            if (!match.Success)
                throw new ArgumentException();
            Value = val;
        }
        public static implicit operator Phone(string val)=>new Phone(val);
    }
}