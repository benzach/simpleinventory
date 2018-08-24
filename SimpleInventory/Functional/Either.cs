using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SimpleInventory.Functional.EitherExt;
using static SimpleInventory.Functional.F;
using Unit = System.ValueTuple;
namespace SimpleInventory.Functional
{
    public struct Either<L, R>
    {
        internal L Left { get; }
        internal R Right { get; }
        public bool IsRight { get; }
        public bool IsLeft => !IsRight;
        private Either(L left)
        {
            this.Left = left;
            IsRight = false;
            this.Right = default(R);
        }
        private Either(R right)
        {
            this.Right = right;
            this.IsRight = true;
            this.Left = default(L);
        }
        public static implicit operator Either<L,R>(Either.Left<L> left)=>new Either<L, R>(left.Value);
        public static implicit operator Either<L,R>(Either.Right<R> right)=>new Either<L, R>(right.Value);
        public static implicit operator Either<L,R>(L val)=>new Either<L, R>(val);
        public static implicit operator Either<L,R>(R val)=>new Either<L, R>(val);

        public TR Match<TR>(Func<L, TR> left, Func<R, TR> right) => this.IsRight ? right(this.Right) : left(this.Left);
        public Unit Match(Action<L> left, Action<R> right) => Match(left.ToFunc(), right.ToFunc());

        public IEnumerable<R> AsEnumerable()
        {
            if (this.IsRight)
                yield return this.Right;
        }
    }
    public class Either
    {
        public struct Left<T>
        {
            internal T Value { get; }
            public Left(T val)
            {
                this.Value = val;
            }
        }
        public struct Right<T>
        {
            internal T Value { get; }
            public Right(T val)
            {
                this.Value = val;
            }
            public Right<RR> Map<RR>(Func<T, RR> f) => Right(f(Value));
            public Right<RR> Bind<RR>(Func<T, Right<RR>> f) => f(Value);
        }
    }
    public static class EitherExt
    {
        public static Either.Left<T> Left<T>(T val) => new Either.Left<T>(val);
        public static Either.Right<T> Right<T>(T val) => new Either.Right<T>(val);

        public static Either<L, RR> Map<L, R, RR>(this Either<L, R> @this, Func<R, RR> f) => 
            @this.Match<Either<L,RR>>(l => l, r => Right(f(r)));
        public static Either<LL, R> Map<L, LL, R>(this Either<L, R> @this, Func<L, LL> f) =>
            @this.Match<Either<LL, R>>(l => f(l), r => r);
        public static Either<L, Unit> ForEach<L, R>(this Either<L, R> @this, Action<R> a) => 
            @this.Map(a.ToFunc());
        public static Either<Unit, R> ForEach<L, R>(this Either<L, R> @this, Action<L> a) => 
            @this.Map(a.ToFunc());
        public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> @this, Func<R, Either<L, RR>> f) => 
            @this.Match(l => l, r => f(r));
        //linq
        public static Either<L, R> Select<L, T, R>(this Either<L, T> @this, Func<T, R> f) =>
            @this.Map(f);
        public static Either<L, RR> SelectMany<L, T, R, RR>(this Either<L, T> @this, Func<T, Either<L, R>> bind, Func<T, R, RR> proj) =>
            @this.Match<Either<L,RR>>(
                l => l,
                r =>bind(r)
                    .Match<Either<L,RR>>(
                        l1 => l1, 
                        r1 => proj(r, r1)
                     )
                );
        //public static Either<LL, R> SelectMany<L, T, LL, R>(this Either<T, R> @this, Func<T, Either<L, R>> bind, Func<T, L, LL> proj) =>
        //    @this.Match<Either<LL, R>>(
        //            l => bind(l)
        //                    .Match<Either<LL, R>>(
        //                        l1 => proj(l, l1),
        //                        r => r
        //                     ),
        //            r1 => r1
        //    );
        public static Either<L, R> Flip<L, R>(this Either<R, L> @this) =>
            @this.Match<Either<L,R>>(l => l,
                        r => r);
        public static Option<L> LeftIfPresent<L, R>(this Either<L, R> @this) =>
            @this.Match(l => Some(l),
                    _ => None);
        public static Option<R> RightIfPresent<L, R>(this Either<L, R> @this) =>
            @this.Match(_ => None,
                r => Some(r));
    }
}
