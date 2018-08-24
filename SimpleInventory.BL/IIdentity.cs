using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleInventory.BL
{
    public struct Key<T>
    {
        public T Value { get; }
        public Key(T val)
        {
            Value = val;
        }
    }
    public interface IIdentity<T>
    {
        Key<T> Key { get; }
    }
}
