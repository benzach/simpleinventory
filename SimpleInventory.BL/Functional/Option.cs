using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SimpleInventory.BL.Functional.F;
using Unit = System.ValueTuple;
namespace SimpleInventory.BL.Functional
{
    //public class Option
    //{
    //}
    public struct Option<T>:IEquatable<Option<T>>,IEquatable<Option.None>
    {
        internal T Value { get; }
        internal bool IsSome { get; }
        internal bool IsNone => !IsSome;
        private Option(T value)
        {
            Value = value;
            IsSome = true;
        }
        public static implicit operator Option<T>(Option.None _)=>new Option<T>();
        public static implicit operator Option<T>(T value)=>value==null?None:Some(value);
        public static implicit operator Option<T>(Option.Some<T> some)=>new Option<T>(some.Value);

        public bool Equals(Option<T> other)
        {
            //throw new NotImplementedException();
            return this.IsSome == other.IsSome && (this.IsNone || this.Value.Equals(other.Value));
        }

        public bool Equals(Option.None _)
        {
            //throw new NotImplementedException();
            return this.IsNone;
        }
        public R Match<R>(Func<R> none, Func<T, R> some) => this.IsNone ? none() : some(this.Value);
        public Unit Match(Action anone, Action<T> asome) => Match(anone.ToFunc(), asome.ToFunc());
        public IEnumerable<T> AsEnumerable()
        {
            if(this.IsSome)
            {
                yield return this.Value;
            }
        }
        public static bool operator == (Option<T> @this, Option<T> other) => @this.Equals(other);
        public static bool operator !=(Option<T> @this, Option<T> other) => !@this.Equals(other);

    }

    public static class Option
    {
        public struct None
        {
            internal static readonly None Default = new None();
        }
        public struct Some<T>
        {
            internal T Value { get; }
            public Some(T val)
            {
                if(val==null)
                {
                    throw new ArgumentNullException();
                }
                Value = val;
            }
        }
    }
    public static class OptionExt
    {
        public static Option<T> FromNullable<T>(T val) => val==null?None:Some(val);
        public static Option<T> FromPossibleZeroLength<T,T1>(T val) where T: IEnumerable<T1> => val.Count()>0?Some(val):None;
        public static Option<R> Map<T, R>(this Option<T> @this, Func<T, R> f) => 
            @this.Match(() => None, val => Some(f(val)));
        public static Option<R> Map<T, R>(this Option.None @this, Func<T, R> f) =>
            None;
        public static Option<R> Map<T, R>(this Option.Some<T> @this, Func<T, R> f) =>
            Some(f(@this.Value));
        public static Option<Func<T2, R>> Map<T1, T2, R>(this Option<T1> @this, Func<T1, T2, R> f) =>
            @this.Map(f.Curry());
        public static Option<Func<T2, T3, R>> Map<T1, T2, T3, R>(this Option<T1> @this, Func<T1, T2, T3, R> f) =>
            @this.Map(f.CurryFirst());

        public static Option<R> Apply<T, R>(this Option<Func<T, R>> OF, Option<T> OT) =>
            OF.Match(
                () => None,
                F => OT.Match(
                    () => None,
                    v => Some(F(v))
                    ));
        public static Option<Func<T2, R>> Apply<T1, T2, R>(this Option<Func<T1, T2, R>> OF, Option<T1> OT) =>
            Apply(OF.Map(F.Curry), OT);
        public static Option<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Option<Func<T1, T2, T3, R>> OF, Option<T1> OT) =>
            Apply(OF.Map(F.CurryFirst), OT);
        public static Option<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Option<Func<T1, T2, T3, T4, R>> OF, Option<T1> OT) =>
            Apply(OF.Map(F.CurryFirst), OT);
            //@this.Match(
            //    () => None,
            //    F => Some<Func<T2,R>>(F.Curry().Apply(t1)())
            //    );

        public static Option<R> Bind<T, R>(this Option<T> @this, Func<T, Option<R>> f) => 
            @this.Match(() => None, val => f(val));

        public static Option<R> Select<T, R>(this Option<T> @this, Func<T, Option<R>> f) =>
            @this.Match(() => None, t => f(t));
        public static Option<RR> SelectMany<T, R, RR>(this Option<T> @this, Func<T, Option<R>> bind, Func<T, R, RR> proj) =>
            @this.Match(
                () => None,
                val => bind(val)
                     .Match(
                        () => None,
                        val1 => Some(proj(val, val1))
                    ));
    }
}
