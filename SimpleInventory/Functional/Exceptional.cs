using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SimpleInventory.Functional.ExceptionalExt;

namespace SimpleInventory.Functional
{

    public class ErrorException:Exception
    {
        public ErrorException(string msg):base(msg)
        {
        }
        public static implicit operator ErrorException(string msg)=>new ErrorException(msg);
    }
    public struct Exceptional<T>
    {
        internal T Value { get; }
        internal ErrorException Error { get; }
        private bool IsSuccessful { get; }
        private bool IsFail => !IsSuccessful;
        internal Exceptional(T val)
        {
            IsSuccessful = true;
            Value = val;
            Error = default(ErrorException);
        }
        internal Exceptional(ErrorException ex)
        {
            Value = default(T);
            Error = ex;
            IsSuccessful = false;
        }
        public static implicit operator Exceptional<T>(T val)=>new Exceptional<T>(val);
        public static implicit operator Exceptional<T>(ErrorException ex)=>new Exceptional<T>(ex);

        public R Match<R>(Func<ErrorException, R> fail, Func<T, R> success) => this.IsSuccessful ? success(this.Value) : fail(this.Error);
        public Unit Match(Action<ErrorException> fail, Action<T> success) => Match(fail.ToFunc(), success.ToFunc());

        public Exceptional<R> Bind<R>(Func<T, Exceptional<R>> f) => this.IsSuccessful ? f(this.Value) : this.Error;
        public Exceptional<R> Map<R>(Func<T, R> f) => this.IsSuccessful ? Success(f(this.Value)) : this.Error;

    }
    public static class ExceptionalExt
    {
        public static Exceptional<T> Success<T>(T val) => new Exceptional<T>(val);
        public static Exceptional<R> Apply<T, R>(this Exceptional<Func<T, R>> AF, Exceptional<T> ET) =>
            AF.Match(err => err,
                f => ET.Map(f));
        public static Exceptional<Func<T2, R>> Apply<T1, T2, R>(this Exceptional<Func<T1, T2, R>> AF, Exceptional<T1> ET1) =>
            Apply(AF.Map(F.Curry), ET1);
        public static Exceptional<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Exceptional<Func<T1, T2, T3, R>> AF, Exceptional<T1> ET1) =>
            Apply(AF.Map(F.CurryFirst), ET1);
        public static Exceptional<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Exceptional<Func<T1, T2, T3, T4, R>> AF, Exceptional<T1> ET1) =>
            Apply(AF.Map(F.CurryFirst), ET1);

        public static Exceptional<R> Select<T, R>(this Exceptional<T> @this, Func<T, Exceptional<R>> f) => @this.Bind(f);
        public static Exceptional<RR> SelectMany<T, R, RR>(this Exceptional<T> @this, Func<T, Exceptional<R>> bind, Func<T, R, RR> proj) =>
            @this.Match(er => er,
                t => bind(t)
                        .Match(e2=>e2,
                                r=>Success(proj(t,r))
                    )
                );
    }


}
