using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.Utility
{
    public static class Util
    {
        public static T Copy<T>(T val)
        {
            var properties = val.GetType().GetProperties();
            T ret = (T)Activator.CreateInstance(typeof(T));
            var copyProperties = ret.GetType().GetProperties();
            foreach (var p in properties)
            {
                var targetProp = ret.GetType().GetProperty(p.Name);
                if (!p.CanRead || !p.CanWrite || val == null)
                {
                    continue;
                }
                targetProp.SetValue(ret, p.GetValue(val, null), null);

            }
            return ret;
        }

    }
}
