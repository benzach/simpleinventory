using System;
using System.Diagnostics;
using System.Threading;
using Unit = System.ValueTuple;
namespace Functional.Lib.Functional
{
    public static partial class F
    {
        public static Action<Action> NewLock()
        {
            object thelock = new string[] { };
            return
                tr =>
                {
                    lock (thelock)
                    {
                        tr();
                    }
                };
        }
        public static void WithLock(Action a)
            => WithLock<Unit>(a.ToFunc());
        public static Out WithLock<Out>(Func<Out> f)
        {
            var mlock = NewLock();
            var res = default(Out);
            int setCount = 0;
            Debug.WriteLine("outside " + Thread.CurrentThread.ManagedThreadId);
            mlock(() => {
                Debug.WriteLine("inside " + Thread.CurrentThread.ManagedThreadId);
                if (setCount>1)
                {
                    throw new Exception("Withlock<> setcount=" + setCount);
                }
                ++setCount;
                res = f();
            });
            return res;
        }
        public static Out With<Out>(Action<Action> rw, Func<Out> tr)
        {
            //var ret = default(Out);
            //rw(() => { ret = tr(); });
            //return ret;
            return With<Unit, Out>(
                trw=>rw(()=>trw(Unit)),
                Unit=>tr());
        }
        public static Out With<In, Out>(Action<Action<In>> rw, Func<In, Out> tr)
            => WithCore(f => { rw(x => f(x)); },
                tr,
                _=>Unit,
                (r,setcount)=>
                {
                    if(!(setcount==1))
                    {
                        throw new Exception("With<> failure setcount=" + setcount);
                    }
                    return r;
                }
                );
        public static OutFinal WithCore<In,OutInner,OutInterim,OutFinal>(
            Action<Func<In,OutInner>> rw,
            Func<In,OutInterim> tr,
            Func<OutInterim,OutInner>fi,
            Func<OutInterim,int,OutFinal> resultOnREsult
            )
        {
            OutInterim result = default(OutInterim);
            int setCount = 0;
            rw(i =>
            {
                result = tr(i);
                ++setCount;
                return fi(result);
            });
            return resultOnREsult(result, setCount);
        }
        public static R Resolve<T,R>(this Func<T,R> f,T t) => f(t);
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
        public static Func<T2,T3,T4,R> Apply<T1,T2,T3,T4,R>(this Func<T1,T2,T3,T4,R>f,T1 t1)=>(t2,t3,t4)=>f(t1, t2, t3, t4);
    }
}
