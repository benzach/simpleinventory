using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleInventory.Functional
{
    public class Error
    {
        public string Messsage { get; }
        private Error(string message)
        {
            Messsage = message;
        }
        public static implicit operator Error(string msg)=>new Error(msg);
    }
    public struct Validation<T>
    {
        internal T Value { get; }
        internal IEnumerable<Error> Errors { get; }
        private bool IsValid { get; }
        private bool IsInvalid => !IsValid;
        internal Validation(T val)
        {
            IsValid = true;
            Value = val;
            Errors = Enumerable.Empty<Error>();
        }
        internal Validation(IEnumerable<Error> errors)
        {
            Errors = errors;
            Value = default(T);
            IsValid = false;
        }
        public static implicit operator Validation<T>(T value)=>new Validation<T>(value);
        public static implicit operator Validation<T>(Error error)=>new Validation<T>(new[] { error });
        public static implicit operator Validation<T>(Validation.Invalid invalid)=>new Validation<T>(invalid.Errors);

        public R Match<R>(Func<IEnumerable<Error>,R> invalid, Func<T, R> valid) =>
            IsValid ? valid(this.Value) : invalid(Errors);
        public Unit Match(Action<IEnumerable<Error>> invalid, Action<T> valid) =>
            Match(invalid.ToFunc(), valid.ToFunc());
        public IEnumerable<T> AsEnumerable()
        {
            if(this.IsValid)
            {
                yield return this.Value;
            }
        }

    }
    public static class Validation
    {
        public struct Invalid
        {
            internal IEnumerable<Error> Errors { get; }
            public Invalid(IEnumerable<Error> errors)
            {
                Errors = errors;
            }
        }
        public static Validation<T> FromNull<T>(T val) => val == null ? (Error)("null value") : (Validation<T>)val;
        public static Validation<T> Valid<T>(T val) => new Validation<T>(val);
        public static Validation<R> Map<T, R>(this Validation<T> @this, Func<T, R> f) => 
            @this.Match<Validation<R>>(l => new Invalid(l), t => Valid(f(t)));
//        public static Validation<Func<T1,T2,R>>Map<T1,T2,R>(this Validation<T1> @this,Func<T1,T2,R>f)=>
 //           @this.Match()
        public static Validation<R> Bind<T, R>(this Validation<T> @this, Func<T, Validation<R>> f) =>
            @this.Match(e => new Invalid(e), t => f(t));
        public static Validation<Unit> ForEach<T>(this Validation<T> @this, Action<T> a) =>
            @this.Map(a.ToFunc());

        public static Validation<R> Select<T, R>(this Validation<T> @this, Func<T, Validation<R>> f) =>
            @this.Match(e => new Invalid(e), t => f(t));
        public static Validation<RR> SelectMany<T, R, RR>(this Validation<T> @this, Func<T, Validation<R>> bind, Func<T, R, RR> proj) =>
            @this.Match(
                e => new Invalid(e),
                t => bind(t)
                   .Match(
                        e1 => new Invalid(e1),
                        r => Valid(proj(t, r))
                    )
                );
        public static Validation<R> Apply<T, R>(this Validation<Func<T, R>> VF, Validation<T> VT) =>
            VF.Match(
                e => VT.Match(
                    e1 => new Invalid(e.Concat(e1)),
                    _ => new Invalid(e))
                ,
                f => VT.Match(
                    e2 => new Invalid(e2),
                    t => Valid(f(t))
                )
           );
        public static Validation<Func<T2, R>> Apply<T, T2, R>(this Validation<Func<T, T2, R>> VF, Validation<T> VT) =>
            Apply(VF.Map(F.Curry), VT);
        //public static Validation<Func<T2, R>> Apply<T, T2, R>(this Validation<Func<T, T2, R>> VF, Validation<T> VT) =>
        //    VF.Match<Validation<Func<T2, R>>>(
        //            e => VT.Match(
        //                e1 => new Invalid(e.Concat(e1)),
        //                _ => new Invalid(e)
        //                ),
        //            v => VT.Match(
        //                e2 => new Invalid(e2),
        //                t => new Validation<Func<T2, R>>(t2 => v(t, t2))
        //                )
        //        );
        public static Validation<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Validation<Func<T1, T2, T3, R>> VF, Validation<T1> VT) =>
            Apply(VF.Map(F.CurryFirst), VT);
        //VF.Match<Validation<Func<T2, T3, R>>>(
        //    e=>VT.Match(
        //            e1=>new Invalid( e.Concat(e1)),
        //            _=>new Invalid(e)
        //        ),
        //    v=>VT.Match(
        //            e2=>new Invalid(e2),
        //            t1=>new Validation<Func<T2, T3, R>>((t2,t3)=>v(t1,t2,t3))
        //        )
        //    );
        public static Validation<Func< T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Validation<Func<T1, T2, T3, T4, R>> VF, Validation<T1> VT) =>
            Apply(VF.Map(F.CurryFirst), VT);
    }
}
