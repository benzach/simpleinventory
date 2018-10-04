using Functional.Lib.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using  static SimpleInventory.BL.BusinessRepoExt;
using SimpleInventory.DL.Models;
using static Functional.Lib.Functional.F;

namespace SimpleInventory.BL
{
    internal class RepoEntityComparer<T, TKey> : EqualityComparer<(T val, TKey key)>
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
    public struct Repository<T, TKey>
    {
        private Dictionary<TKey, Func<T>> Values { get; }
        internal Repository(T value, TKey key)
        {
            Values = new Dictionary<TKey, Func<T>>();
            Values.Add(key, () => value);

        }
        internal Repository(IEnumerable<(T val, TKey key)> entities)
        {
            //Data = new Dictionary<TKey, T>(entities.Distinct(new EntityComparer<T,TKey>()).Select(x =>KeyValuePair.Create(x.key, x.val)));
            Values = new Dictionary<TKey, Func<T>>(entities.Distinct(new RepoEntityComparer<T, TKey>()).Select(x => KeyValuePair.Create(x.key, (Func<T>)(() => x.val))));
            //foreach(var kv in entities)
            //{
            //    Data.Add(() => kv.key, () => kv.val);
            //}
        }
        public static implicit operator Repository<T, TKey>((T val, TKey key) entity) => new Repository<T, TKey>(entity.val, entity.key);
        public static implicit operator Repository<T, TKey>(List<(T val, TKey key)> entities) => new Repository<T, TKey>(entities);
        public static implicit operator Repository<T, TKey>(List<KeyValuePair<TKey, T>> entities) => new Repository<T, TKey>(entities.Select(x => (x.Value, x.Key)).ToList());
        public Repository<R, RKey> Map<R, RKey>(Func<List<(T, TKey)>, List<(R, RKey)>> f)
            => f(this.Values.Select(kv => (kv.Value(), kv.Key)).ToList());
        public Repository<R, RKey> Bind<R, RKey>(Func<List<(T, TKey)>, Repository<R, RKey>> f)
            => f(this.Values.Select(kv => (kv.Value(), kv.Key)).ToList());
        public IEnumerable<(TKey Key, Func<T> Value)> Data
            => this.Values.Select(x=>(x.Key,x.Value)) ;
        public IEnumerable<(TKey Key, Func<T> Value)> GetByKey(Func<TKey, bool> predicate)
            => this.Values.Where(x => predicate(x.Key)).Select(y=>(y.Key,y.Value));
        public IEnumerable<(TKey Key, Func<T> Value)> GetByT(Func<T, bool> Pred)
            => this.Values.Where(x => Pred(x.Value())).Select(y=>(y.Key,y.Value));
        //public IEnumerable<(TKey Key,T Value) > ReconcileWithDataStore(EFDataStoreSink  sink,Func<DbContext,IEnumerable<(TKey Key,T Value)>> f)
        //{
        //    var a = sink.ReportAll(f)();
        //    this.Values.
        //}
        public static Repository<T, TKey> ReconcileWithDataStore<DB>(EFDataStoreSink<DB> sink, Func<DB, IEnumerable<(T val, TKey key)>> f)
            => sink.ReportAll(f)().ToRepository();
        
    }
    public sealed class EFDataStoreSink<DB>:IDisposable
    {
        private readonly DB _DB;
        private readonly Option<Action> mCloseupIfStillAvailable;
        private readonly Action<Action> mLock = NewLock();
        public EFDataStoreSink(DB dbcontext)
        {
            _DB = dbcontext; 
            //_SimpleInventoryDbContext = dbcontext as SimpleInventoryContext;
        }
        public void Close() => mCloseupIfStillAvailable.Match(() => new Action(() => throw new Exception("already close")), x => x);

        public void Dispose() => Close();
        
        public  Func<IEnumerable<(T Value,TKey Key)>> ReportAll<T,TKey>(Func<DB, IEnumerable<(T Value, TKey Key)>> f)
        {
            //var a = f(_SimpleInventoryContext);
            var ret = Enumerable.Empty< (T Value,TKey Key)>(); //IEnumerable<(TKey key,T Value)>
            mLock(()=>
                ret=f(_DB)
            );
            return () => ret;
        }
    }
    public static class RepositoryExt
    {
        public static Repository<T, TKey> ToRepository<T, TKey>(this IEnumerable<(T, TKey)> @this)
            => new Repository<T, TKey>(@this);

        public static Repository<R, RKey> Apply<T, TKey, R, RKey>(
            this Repository<Func<T, R>, Func<TKey, RKey>> fthis,
            Repository<T, TKey> BT)
            => (from ft in fthis.Data
                from t in BT.Data
                select (ft.Value()(t.Value()), ft.Key(t.Key)))
                .ToRepository();

        public static Repository<Func<T2, R>, Func<TKey2, RKey>> Apply<T1, T2, R, TKey1, TKey2, RKey>(this Repository<Func<T1, T2, R>, Func<TKey1, TKey2, RKey>> fthis, Repository<T1, TKey1> BT1)
        => (from ft in fthis.Data
            from t in BT1.Data
            select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
            .ToRepository();
        public static Repository<Func<T2, T3, R>, Func<TKey2, TKey3, RKey>> Apply<T1, T2, T3, R, TKey1, TKey2, TKey3, RKey>(this Repository<Func<T1, T2, T3, R>, Func<TKey1, TKey2, TKey3, RKey>> fthis, Repository<T1, TKey1> BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
            .ToRepository();
        public static Repository<Func<T2, T3, T4, R>, Func<TKey2, TKey3, TKey4, RKey>> Apply<T1, T2, T3, T4, R, TKey1, TKey2, TKey3, TKey4, RKey>(this Repository<Func<T1, T2, T3, T4, R>, Func<TKey1, TKey2, TKey3, TKey4, RKey>> fthis, Repository<T1, TKey1> BT1)
            => (from ft in fthis.Data
                from t in BT1.Data
                select (F.Apply(ft.Value(), t.Value()), F.Apply(ft.Key, t.Key)))
                .ToRepository();
        public static Repository<R, RKey> SelectMany<T, R, TKey, RKey>(this Repository<T, TKey> @this, Func<(T value, TKey), Repository<R, RKey>> bind)
            => @this.SelectMany(bind, (tvalkey, rvalkey) => rvalkey);
        public static Repository<RR, RRKey> SelectMany<T, R, RR, TKey, RKey, RRKey>(
            this Repository<T, TKey> @this,
            Func<(T value, TKey key), Repository<R, RKey>> bind,
            Func<(T value, TKey key), (R value, RKey key), (RR value, RRKey key)> proj)
            =>@this.Data
                    .SelectMany(x 
                                => bind((x.Value(), x.Key))
                                    .Data
                                    .Select(y
                                             =>proj((x.Value(), x.Key),(y.Value(),y.Key))
                                            )
                                ).ToRepository();

        public static Repository<R, RKey> Select<T, R, TKey, RKey>(this Repository<T, TKey> @this, Func<(T, TKey), Repository<R, RKey>> f)
            => @this.Data.SelectMany(x => f((x.Value(), x.Key)).Data.Select(y => (y.Value(), y.Key))).ToRepository();  //.Bind(y => f(y));

    }
}
