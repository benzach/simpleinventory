using System;
using Xunit;
using Functional.Lib.Functional;
using static Functional.Lib.Functional.F;

namespace SimpleInventoryTest
{
    public class EitherTest
    {
        [Fact]
        public void Initialize_Either_From_Left_Should_Return_Left()
        {
            int num = 1;
            Either<int, string> e = EitherExt.Left(num);
            e.Match(i => Assert.Equal(num, i),
                s => Assert.True(false));
        }
        [Fact]
        public void Initialize_Either_From_right_Should_REturn_Right()
        {
            string val = "test";
            Either<int, string> e = EitherExt.Right(val);
            e.Match(i => Assert.False(true),
                s => Assert.Equal(val, s));
        }
        [Fact]
        public void Map_Right_To_Transform_modifies_Right_only()
        {
            int num = 10;
            Either<string, int> Either = EitherExt.Right(num);
            var e=Either.Map(x => (double)x * 2);
            e.Match(s => Assert.True(s is string),
                v => Assert.Equal((double)num * 2,v));

        }
        public Func<string, int> xfrm => s => int.Parse(s) * 2;
        [Fact]
        public void Map_Left_To_Trasnform_Modifies_Right_only()
        {
            string s = "10";
            Either<string, int> Either = EitherExt.Left(s);
            
            var e = Either.Map(xfrm);

            e.Match(i => Assert.Equal(int.Parse(s) * 2, i),
                i2=>Assert.False(true));
        }
        [Fact]
        public void SelectMany_combine_many_either_into_single_scope()
        {
            Either<string, int> e1 = EitherExt.Right(1);
            Either<string, int> e2 = EitherExt.Right(2);
            Either<string, int> e3 = EitherExt.Right(3);
            var res = from n1 in e1
                      from n2 in e2
                      from n3 in e3
                      select n1 + n2 + n3;
            res.Match(s => Assert.False(true),
                v => Assert.Equal(1+2+3, v));
        }
        [Fact]
        public void SelectMany_work_on_nested_either()
        {
            Either<string,int> inE=EitherExt.Right(5);
            Either<string, Either<string, int>> e = EitherExt.Right(inE);
            var res = from a in e
                      from b in a
                      select (double)b / 2.0;

            res.Match(s => Assert.False(true),
                d => Assert.Equal(5.0 / 2.0, d));
        }
        [Fact(DisplayName = "once either goes left, it stays left")]
        public void SelectMany_combine_many_either_L_R_into_single_scope()
        {
            Either<string, int> e1 = EitherExt.Right(1);
            Either<string, int> e2 = EitherExt.Left("2");
            Either<string, int> e3 = EitherExt.Right(3);
            Either<string, int> e4 = EitherExt.Left("4");
            var res = from n1 in e1
                      from n2 in e2
                      from n3 in e3
                      from n4 in e4
                      select n1 + n2 + n3 + n4;
            res.Match(s => Assert.Equal("2", s),
                v => Assert.Equal(1 + 2 + 3, v));
        }
        [Fact(DisplayName = "If right is not present then the result of selectmany will be none")]
        public void SelectMany_combine_many_either_L_R_into_single_scope_it_stop_as_soon_see_left()
        {
            Either<string, int> e1 = EitherExt.Right(1);
            Either<string, int> e2 = EitherExt.Left("2");
            Either<string, int> e3 = EitherExt.Right(3);
            Either<string, int> e4 = EitherExt.Left("4");
            var res = from n1 in e1.RightIfPresent()
                      from n2 in e2.RightIfPresent()
                      from n3 in e3.RightIfPresent()
                      from n4 in e4.RightIfPresent()
                      select n1 + n2 + n3 + n4;
            //var res1 = res.Flip();
            res.Match(() => Assert.True(true),
                i => Assert.False(true));
        }

    }
}
