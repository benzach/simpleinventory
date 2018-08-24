using System;
using Unit = System.ValueTuple;
namespace SimpleInventory.BL.Functional
{
    public static partial class F
    {
        public static Option.None None => Option.None.Default;
        public static Option<T> Some<T>(T val) => new Option.Some<T>(val);
        public static Unit Unit => default(Unit);
        public static Func<Unit> ToFunc(this Action @this) => () => { @this(); return Unit; };
        public static Func<T, Unit> ToFunc<T>(this Action<T> @this) => val => { @this(val); return Unit; };
        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> f) => t1 => t2 => f(t1, t2);
        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> f) => t1 => t2 => t3 => f(t1, t2, t3);
        public static Func<T1, Func<T2, T3, R>> CurryFirst<T1, T2, T3, R>(this Func<T1, T2, T3, R> f) => t1 => (t2, t3) => f(t1, t2, t3);
        public static Func<T1, Func<T2, T3, T4, R>> CurryFirst<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f) => t1 => (t2, t3, t4) => f(t1, t2, t3, t4);
        public static Func<R> Apply<T1, R>(this Func<T1, R> f, T1 t1) => () => f(t1);
        public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> f, T1 t1) => t2 => f(t1, t2);
        public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> f, T1 t1) => (t2, t3) => f(t1, t2, t3);
    }
}
