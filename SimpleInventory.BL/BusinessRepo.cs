using Functional.Lib.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  static SimpleInventory.BL.BusinessRepoExt;

namespace SimpleInventory.BL
{
    //public struct Key<T>
    //{
    //    public T Value { get; }
    //    public Key(T val)
    //    {
    //        Value = val;
    //    }
    //    public static implicit operator Key<T>(T val)=>new Key<T>(val);
    //}
    public struct Entity<T,TKey>:IEqualityComparer<TKey>
    {
        public T Value { get; }
        //public Key<TKey> Key { get; }
        //public Entity(T val,Key<TKey> key)
        //{
        //    Value = val;
        //    Key = key;
        //}

        public bool Equals(TKey x, TKey y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(TKey obj)
        {
            return obj.GetHashCode();
        }
    }
    internal class EntityComparer<T , TKey > : EqualityComparer<(T val, TKey key)>
    {
        public override bool Equals((T val, TKey key) x, (T val, TKey key) y)
        {
            return x.key.Equals(y.key);
        }

        public override int GetHashCode((T val, TKey key) obj)
        {
            //throw new NotImplementedException();
            return obj.key.GetHashCode();
        }
    }
    public struct BusinessRepo<T, TKey>
    {
        public Dictionary<TKey,T> Data { get; }
        internal BusinessRepo(T value,TKey key)
        {
            Data = new Dictionary<TKey, T>(new[] { KeyValuePair.Create(key, value) });
        }
        private Func<KeyValuePair<TKey, T>> CreateKeyVal(T val, TKey key)
            => ()=>KeyValuePair.Create(key, val);
        internal BusinessRepo(IEnumerable<(T val,TKey key)> entities)
        {
            Data = new Dictionary<TKey, T>(entities.Distinct(new EntityComparer<T,TKey>()).Select(x =>KeyValuePair.Create(x.key, x.val)));
        }
        public static implicit operator BusinessRepo<T,TKey>((T val,TKey key) entity)=>new BusinessRepo<T, TKey>(entity.val,entity.key);
        public static implicit operator BusinessRepo<T,TKey>(List<(T val,TKey key)> entities)=>new BusinessRepo<T, TKey>(entities);
        public BusinessRepo<R, RKey> Map<R, RKey>(Func<List<(T, TKey)>, List<(R, RKey)>> f)
            => f(this.Data.Select(kv => (kv.Value, kv.Key)).ToList());
        public BusinessRepo<R, RKey> Bind<R, RKey>(Func<List<(T, TKey)>, BusinessRepo<R, RKey>> f)
            => f(this.Data.Select(kv => (kv.Value, kv.Key)).ToList());
    }
    public static class BusinessRepoExt
    {
        public static BusinessRepo<T, TKey> ToBusinessRepo<T, TKey>(this IEnumerable<(T, TKey)> @this) 
            => new BusinessRepo<T, TKey>(@this);
        public static BusinessRepo<R, RKey> Apply<T, TKey, R, RKey>(
            this BusinessRepo<Func<T, R>, Func<TKey, RKey>> fthis,
            BusinessRepo<T, TKey> BT)
            => (from ft in fthis.Data
                from t in BT.Data
                select (ft.Value(t.Value), ft.Key(t.Key)))
                .ToBusinessRepo();

        public static BusinessRepo<Func<T2, R>, Func<TKey2, RKey>> Apply<T1, T2, R, TKey1, TKey2, RKey>(this BusinessRepo<Func<T1, T2, R>, Func<TKey1, TKey2, RKey>> fthis, BusinessRepo<T1, TKey1> BT1)
        => (from ft in fthis.Data
            from t in BT1.Data
            select (F.Apply(ft.Value, t.Value), F.Apply(ft.Key, t.Key)))
            .ToBusinessRepo();
        public static BusinessRepo<Func<T2, T3, R>, Func<TKey2, TKey3, RKey>> Apply<T1, T2, T3, R, TKey1, TKey2, TKey3, RKey>(this BusinessRepo<Func<T1, T2, T3, R>, Func<TKey1, TKey2, TKey3, RKey>> fthis, BusinessRepo<T1, TKey1> BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (F.Apply(ft.Value, t.Value), F.Apply(ft.Key, t.Key)))
            .ToBusinessRepo();
        public static BusinessRepo<Func<T2,T3,T4,R>,Func<TKey2,TKey3,TKey4,RKey>>Apply<T1, T2, T3,T4, R, TKey1, TKey2, TKey3,TKey4, RKey>(this BusinessRepo<Func<T1,T2,T3,T4,R>,Func<TKey1,TKey2,TKey3,TKey4,RKey>> fthis,BusinessRepo<T1,TKey1>BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (F.Apply(ft.Value, t.Value), F.Apply(ft.Key, t.Key)))
                .ToBusinessRepo();

    }

}
