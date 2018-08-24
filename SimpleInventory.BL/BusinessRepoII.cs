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
    internal class EntityIIComparer<T> : EqualityComparer<Option<T>>
    {
        //public override bool Equals((T val, TKey key) x, (T val, TKey key) y)
        //{
        //    return x.key.Equals(y.key);
        //}

        //public override int GetHashCode((T val, TKey key) obj)
        //{
        //    //throw new NotImplementedException();
        //    return obj.key.GetHashCode();
        //}
        public override bool Equals(Option<T> x, Option<T> y)
        {
            return x == y;
        }


        public override int GetHashCode(Option<T> obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct BusinessRepoII<T>
    {
        public IEnumerable<Option<T>> Data { get; }
        internal BusinessRepoII(Option<T> value)
        {
            Data = new[] { value };
        }
         
        internal BusinessRepoII(IEnumerable<Option<T>> entities)
        {
            Data = entities.Distinct(new EntityIIComparer<T>()).AsEnumerable();
        }
        public static implicit operator BusinessRepoII<T>(T val)=>new BusinessRepoII<T>(OptionExt.FromNullable(val));
        public static implicit operator BusinessRepoII<T>(List<Option<T>> entities)=>new BusinessRepoII<T>(entities);

        public BusinessRepoII<R> Map<R>(Func<IEnumerable<Option<T>>, IEnumerable<Option<R>>> f)
            => f(this.Data).ToList();
        public BusinessRepoII<R> Bind<R>(Func<IEnumerable<Option<T>>, BusinessRepoII<R>> f)
            => f(this.Data);
    }
    public static class BusinessRepoIIExt
    {
        public static BusinessRepoII<T> ToBusinessRepoII<T>(this IEnumerable<Option<T>> @this) 
            => new BusinessRepoII<T>(@this);
        public static BusinessRepoII<R> Apply<T, R>(
            this BusinessRepoII<Func<T, R>> fthis,
            BusinessRepoII<T> BT)
            => (from ft in fthis.Data
                from t in BT.Data
                select ft.Apply(t))
                .ToBusinessRepoII();

        //public static BusinessRepo<R, RKey> Apply<T, TKey, R, RKey>(
        //    this BusinessRepo<Func<T, R>, Func<TKey, RKey>> fthis, 
        //    BusinessRepo<T, TKey> BT)
        //    => (from ft in fthis.Values
        //        from t in BT.Values
        //        select new Entity<R, RKey>(ft.Value.Value(t.Value.Value), new Key<RKey>(ft.Key(t.Key)))).ToList();
        //select new Entity<R,RKey>(ft.Key(t.Value.Value),new Key<RKey>(ft.Value.Value(t.Key)))).ToList();

        public static BusinessRepoII<Func<T2, R>> Apply<T1, T2, R>(this BusinessRepoII<Func<T1, T2, R>> fthis, BusinessRepoII<T1> BT1)
        => (from ft in fthis.Data
            from t in BT1.Data
            select (ft.Apply(t)))
            .ToBusinessRepoII();
        public static BusinessRepoII<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this BusinessRepoII<Func<T1, T2, T3, R>> fthis, BusinessRepoII<T1> BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (ft.Apply(t)))
            .ToBusinessRepoII();
        public static BusinessRepoII<Func<T2,T3,T4,R>>Apply<T1, T2, T3,T4, R>(this BusinessRepoII<Func<T1,T2,T3,T4,R>> fthis,BusinessRepoII<T1>BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (ft.Apply(t)))
                .ToBusinessRepoII();

        //public static BusinessRepo<Func<T2, R>, Func<TKey2, RKey>> Apply<T1, T2, R, TKey1, TKey2, RKey>(this BusinessRepo<Func<T1, T2, R>, Func<TKey1, TKey2, RKey>> fthis, BusinessRepo<T1, TKey1> BT1)
        //=> (from ft in fthis.Values
        //    from t in BT1.Values
        //    select new Entity<Func<T2, R>, Func<TKey2, RKey>>(F.Apply(ft.Value.Value, t.Value.Value), new Key<Func<TKey2, RKey>>(F.Apply(ft.Key, t.Key)))).ToList();
    }

    //public struct BusinessRepo<T>
    //{
    //    public IEnumerable<T> Value { get; }
    //    internal BusinessRepo(IEnumerable<T> val)
    //    {
    //        Value = val;
    //    }
    //    internal BusinessRepo(T val)
    //    {
    //        Value = new List<T>(new[] { val });
    //    }
    //    public static implicit operator BusinessRepo<T>(List<T> val) => new BusinessRepo<T>(val);
    //    public BusinessRepo<R> Bind<R>(Func<IEnumerable<T>, BusinessRepo<R>> f) => f(this.Value);
    //    public BusinessRepo<R> Map<R>(Func<T, R> f) => BusinessRepoExt.BusinessRepo(Value.Select(x => f(x)));
    //}
    //public static class BusinessRepoExt
    //{
    //    public static BusinessRepo<T> BusinessRepo<T>(this IEnumerable<T> @this)
    //        => new BusinessRepo<T>(@this);
    //    public static BusinessRepo<R> Apply<T, R>(this BusinessRepo<Func<T, R>> @fthis, BusinessRepo<T> BT)
    //        => BusinessRepo(from ft in @fthis.Value
    //                        from t in BT.Value
    //                        select ft(t));
    //    public static BusinessRepo<Func<T2, R>> Apply<T1, T2, R>(this BusinessRepo<Func<T1, T2, R>> @fthis, BusinessRepo<T1> BT)
    //        => Apply(@fthis.Map(F.Curry), BT);
    //    public static BusinessRepo<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this BusinessRepo<Func<T1, T2, T3, R>> @fthis, BusinessRepo<T1> BT)
    //        => Apply(@fthis.Map(F.CurryFirst), BT);
    //    public static BusinessRepo<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this BusinessRepo<Func<T1, T2, T3, T4, R>> BF, BusinessRepo<T1> BT)
    //        => Apply(BF.Map(F.CurryFirst), BT);

    //    //Linq
    //    public static BusinessRepo<R> Select<T, R>(this BusinessRepo<T> @this, Func<T, BusinessRepo<R>> f)
    //        => BusinessRepo(from a in @this.Value.Select(x => f(x))               
    //           from b in a.Value
    //           select b);
    //}
}
