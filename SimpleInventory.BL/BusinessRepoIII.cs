using Functional.Lib.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  static SimpleInventory.BL.BusinessRepoExt;

namespace SimpleInventory.BL
{
    internal class EntityComparerIII<T, TKey> : EqualityComparer<(T val, TKey key)>
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
    public sealed class RepoSink<T,TKey>
    {
        private RepoSink(Action reportAll, Action<TKey> reportByKey,Action<T>reportByT)
        {
            ReportAll = reportAll;
            ReportByKey = reportByKey;
            ReportByT = reportByT;
        }
        public static RepoSink<T,TKey> FromAllByKeyByT(Action reportAll, Action<TKey> reportByKey, Action<T> reportByT) =>
            new RepoSink<T, TKey>(reportAll, reportByKey, reportByT);
        public Action ReportAll;
        public Action<TKey> ReportByKey;
        public Action<T> ReportByT;
    }
    public struct BusinessRepoIII<T, TKey>
    {
        private Dictionary<TKey, Func<T>> Values { get; }
        internal BusinessRepoIII(T value, TKey key)
        {
            Values = new Dictionary<TKey, Func<T>>();
            Values.Add(key, () => value);

        }
        internal BusinessRepoIII(IEnumerable<(T val, TKey key)> entities)
        {
            //Data = new Dictionary<TKey, T>(entities.Distinct(new EntityComparer<T,TKey>()).Select(x =>KeyValuePair.Create(x.key, x.val)));
            Values = new Dictionary<TKey, Func<T>>(entities.Distinct(new EntityComparerIII<T, TKey>()).Select(x => KeyValuePair.Create(x.key, (Func<T>)(() => x.val))));
            //foreach(var kv in entities)
            //{
            //    Data.Add(() => kv.key, () => kv.val);
            //}
        }
        public static implicit operator BusinessRepoIII<T, TKey>((T val, TKey key) entity) => new BusinessRepoIII<T, TKey>(entity.val, entity.key);
        public static implicit operator BusinessRepoIII<T, TKey>(List<(T val, TKey key)> entities) => new BusinessRepoIII<T, TKey>(entities);
        public static implicit operator BusinessRepoIII<T, TKey>(List<KeyValuePair<TKey, T>> entities) => new BusinessRepoIII<T, TKey>(entities.Select(x => (x.Value, x.Key)).ToList());
        public BusinessRepoIII<R, RKey> Map<R, RKey>(Func<List<(T, TKey)>, List<(R, RKey)>> f)
            => f(this.Values.Select(kv => (kv.Value(), kv.Key)).ToList());
        public BusinessRepoIII<R, RKey> Bind<R, RKey>(Func<List<(T, TKey)>, BusinessRepoIII<R, RKey>> f)
            => f(this.Values.Select(kv => (kv.Value(), kv.Key)).ToList());
        //public IEnumerable<KeyValuePair<TKey, T>> Result(Func<(T, TKey), bool> Predicate)
        //{
        //    foreach (var kv in this.Data)
        //    {
        //        if (Predicate((kv.Value(), kv.Key)))
        //        {
        //            yield return new KeyValuePair<TKey, T>(kv.Key, kv.Value());
        //        }
        //    }
        //}
        public IEnumerable<(TKey Key, Func<T> Value)> Data
            => this.Values.Select(x=>(x.Key,x.Value)) ;
        public IEnumerable<(TKey Key, Func<T> Value)> GetByKey(Func<TKey, bool> predicate)
            => this.Values.Where(x => predicate(x.Key)).Select(y=>(y.Key,y.Value));
        public IEnumerable<(TKey Key, Func<T> Value)> GetByT(Func<T, bool> Pred)
            => this.Values.Where(x => Pred(x.Value())).Select(y=>(y.Key,y.Value));
    }
    //public static class BusinessRepoIIIExt
    //{
    //    public static BusinessRepoIII<T, TKey> ToBusinessRepoIII<T, TKey>(this IEnumerable<(T, TKey)> @this)
    //        => new BusinessRepoIII<T, TKey>(@this);
    //    public static BusinessRepoIII<R, RKey> Apply<T, TKey, R, RKey>(
    //        this BusinessRepoIII<Func<T, R>, Func<TKey, RKey>> fthis,
    //        BusinessRepoIII<T, TKey> BT)
    //        => (from ft in fthis.Data
    //            from t in BT.Data
    //            select (ft.Value()(t.Value()), ft.Key(t.Key)))
    //            .ToBusinessRepoIII();

    //    public static BusinessRepoIII<Func<T2, R>, Func<TKey2, RKey>> Apply<T1, T2, R, TKey1, TKey2, RKey>(this BusinessRepoIII<Func<T1, T2, R>, Func<TKey1, TKey2, RKey>> fthis, BusinessRepoIII<T1, TKey1> BT1)
    //    => (from ft in fthis.Data
    //        from t in BT1.Data
    //        select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
    //        .ToBusinessRepoIII();
    //    public static BusinessRepoIII<Func<T2, T3, R>, Func<TKey2, TKey3, RKey>> Apply<T1, T2, T3, R, TKey1, TKey2, TKey3, RKey>(this BusinessRepoIII<Func<T1, T2, T3, R>, Func<TKey1, TKey2, TKey3, RKey>> fthis, BusinessRepoIII<T1, TKey1> BT1)
    //        => (from ft in fthis.Data
    //            from t in BT1.Data
    //            select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
    //        .ToBusinessRepoIII();
    //    public static BusinessRepoIII<Func<T2, T3, T4, R>, Func<TKey2, TKey3, TKey4, RKey>> Apply<T1, T2, T3, T4, R, TKey1, TKey2, TKey3, TKey4, RKey>(this BusinessRepoIII<Func<T1, T2, T3, T4, R>, Func<TKey1, TKey2, TKey3, TKey4, RKey>> fthis, BusinessRepoIII<T1, TKey1> BT1)
    //        => (from ft in fthis.Data
    //            from t in BT1.Data
    //            select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
    //            .ToBusinessRepoIII();
    //    public static BusinessRepoIII<RR, RRKey> SelectMany<T, R, RR, TKey, RKey, RRKey>(
    //        this BusinessRepoIII<T, TKey> @this,
    //        Func<(T, TKey), BusinessRepoIII<R, RKey>> bind,
    //        Func<(T, TKey), (R, RKey), (RR, RRKey)> proj)

    //    {
    //        var a1 = @this.Data.Select(x =>
    //        {
    //            var a2 = bind((x.Value(), x.Key));
    //            var a3 = a2.Data.ToList().Select(y => proj((x.Value(), x.Key), (y.Value(), y.Key)));
    //            return a3.ToBusinessRepoIII();
    //        });
    //        List<(RR, RRKey)> res = new List<(RR, RRKey)>();
    //        foreach (var b1 in a1)
    //        {
    //            res.AddRange(b1.Data.Select(y => (y.Value(), y.Key)));
    //        }
    //        return res.ToBusinessRepoIII();
    //    }
    //    public static BusinessRepoIII<R, RKey> Select<T, R, TKey, RKey>(this BusinessRepoIII<T, TKey> @this, Func<List<(T, TKey)>, BusinessRepoIII<R, RKey>> f)
    //        => @this.Bind(y => f(y));

    //}

    public static class BusinessRepoIIIExt
    {
        public static BusinessRepoIII<T, TKey> ToBusinessRepoIII<T, TKey>(this IEnumerable<(T, TKey)> @this)
            => new BusinessRepoIII<T, TKey>(@this);

        public static BusinessRepoIII<R, RKey> Apply<T, TKey, R, RKey>(
            this BusinessRepoIII<Func<T, R>, Func<TKey, RKey>> fthis,
            BusinessRepoIII<T, TKey> BT)
            => (from ft in fthis.Data
                from t in BT.Data
                select (ft.Value()(t.Value()), ft.Key(t.Key)))
                .ToBusinessRepoIII();

        //public static BusinessRepoIII<R, RKey> Apply<T, TKey, R, RKey>(
        //    this BusinessRepoIII<Func<T, R>, Func<TKey, RKey>> fthis,
        //    BusinessRepoIII<T, TKey> BT)
        //    => (from ft in fthis.GetAll()
        //        from t in BT.GetAll()
        //        select (ft.Item1(t.Item1), ft.Item2(t.Item2)))
        //        .ToBusinessRepoIII();

        public static BusinessRepoIII<Func<T2, R>, Func<TKey2, RKey>> Apply<T1, T2, R, TKey1, TKey2, RKey>(this BusinessRepoIII<Func<T1, T2, R>, Func<TKey1, TKey2, RKey>> fthis, BusinessRepoIII<T1, TKey1> BT1)
        => (from ft in fthis.Data
            from t in BT1.Data
            select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
            .ToBusinessRepoIII();
        public static BusinessRepoIII<Func<T2, T3, R>, Func<TKey2, TKey3, RKey>> Apply<T1, T2, T3, R, TKey1, TKey2, TKey3, RKey>(this BusinessRepoIII<Func<T1, T2, T3, R>, Func<TKey1, TKey2, TKey3, RKey>> fthis, BusinessRepoIII<T1, TKey1> BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
            .ToBusinessRepoIII();
        public static BusinessRepoIII<Func<T2, T3, T4, R>, Func<TKey2, TKey3, TKey4, RKey>> Apply<T1, T2, T3, T4, R, TKey1, TKey2, TKey3, TKey4, RKey>(this BusinessRepoIII<Func<T1, T2, T3, T4, R>, Func<TKey1, TKey2, TKey3, TKey4, RKey>> fthis, BusinessRepoIII<T1, TKey1> BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
                .ToBusinessRepoIII();
        public static BusinessRepoIII<RR, RRKey> SelectMany<T, R, RR, TKey, RKey, RRKey>(
            this BusinessRepoIII<T, TKey> @this,
            Func<(T value, TKey key), BusinessRepoIII<R, RKey>> bind,
            Func<(T value, TKey key), (R value, RKey key), (RR value, RRKey key)> proj)
            =>@this.Data
                    .SelectMany(x 
                                => bind((x.Value(), x.Key))
                                    .Data
                                    .Select(y
                                             =>proj((x.Value(), x.Key),(y.Value(),y.Key))
                                            )
                                ).ToBusinessRepoIII();

        //{
        //    var a1 = @this.Data.SelectMany(x => bind((x.Value(),x.Key)).Data.Select(y=>proj((x.Value(),x.Key),(y.Value(),y.Key)))).ToBusinessRepoIII()
        //    {
        //        var a2 = bind((x.Value(), x.Key));
        //        var a3 = a2.Data.Select(y => proj((x.Value(), x.Key), (y.Value(), y.Key)));
        //        //return a3.ToBusinessRepoIII();
        //        return a3;
        //    });
        //    return a1.ToBusinessRepoIII();
        //    //List<(RR, RRKey)> res = new List<(RR, RRKey)>();
        //    //foreach (var b1 in a1)
        //    //{
        //    //    res.AddRange(b1.Data.Select(y => (y.Value(), y.Key)));
        //    //}
        //    //return res.ToBusinessRepoIII();
        //}
        public static BusinessRepoIII<R, RKey> Select<T, R, TKey, RKey>(this BusinessRepoIII<T, TKey> @this, Func<(T, TKey), BusinessRepoIII<R, RKey>> f)
            => @this.Data.SelectMany(x => f((x.Value(), x.Key)).Data.Select(y => (y.Value(), y.Key))).ToBusinessRepoIII();  //.Bind(y => f(y));

    }
}
