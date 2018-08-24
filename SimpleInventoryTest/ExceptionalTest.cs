using Functional.Lib.Functional;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SimpleInventoryTest
{
    public class ExceptionalTest
    {
        [Fact]
        public void Given_Value_Return_Success_Exceptional()
        {
            Exceptional<int> validInt = 5;
            validInt.Match(err => Assert.False(true),
                i => Assert.Equal(5, i));
        }
        [Fact]
        public void Given_ExceptionError_REturns_Fail_Exceptional()
        {
            Exceptional<int> invalid = (ErrorException)"this is an error";
            invalid.Match(err => Assert.Equal("this is an error", err.Message),
                i => Assert.False(true));
        }
        [Theory]
        [InlineData(5)]
        [InlineData(null)]
        public void Assert_Int_Val_REturn_Success_and_null_retuns_Failure(int? num)
        {
            Exceptional<int> validInt = num.HasValue? (Exceptional<int>)num.Value:(ErrorException)("Number is null") ;
            bool isValid = num.HasValue;
            validInt.Match(
                er => Assert.True(!isValid && "Number is null" == er.Message),
                i => Assert.Equal(num.Value, i)
                );
        }
        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void Bind_Transform_ET_To_ER(int? num)
        {
            Exceptional<int> valid = num.HasValue ? (Exceptional<int>)num : (ErrorException)("num is null");
            bool isValid = num.HasValue;
            var res = valid.Bind(i => (Exceptional<string>)$"val is {i}");
            res.Match(err => Assert.Equal("num is null", err.Message),
                msg => Assert.Equal($"val is {num}", msg)
                );
        }
        [Theory]
        [InlineData(null)]
        [InlineData(2.5)]
        public void Map_TransFrom_ET_To_Er(double? num)
        {
            Exceptional<double> valid = num.HasValue ? (Exceptional<double>)num : (ErrorException)("num is null");
            bool isValid = num.HasValue;
            var res = valid.Map(d => $"num is {d}");
            res.Match(err => Assert.True(!isValid && err.Message == "num is null"),
                s => Assert.Equal($"num is {num}", s));
        }
        [Theory]
        [InlineData(null,null,null)]
        [InlineData(1,null,null)]
        [InlineData(1,2,null)]
        [InlineData(1,2,3)]
        public void Assert_apply_one_param_at_a_time_and_fail_fast(int? n1,int? n2,int? n3)
        {
            Exceptional<int> v1 = n1.HasValue ? (Exceptional<int>)n1.Value : (ErrorException)("n1 is null");
            Exceptional<int> v2 = n2.HasValue ? (Exceptional<int>)n2.Value : (ErrorException)("n2 is null");
            Exceptional<int> v3 = n3.HasValue ? (Exceptional<int>)n3.Value : (ErrorException)("n1 is null");
            var isValid = n1.HasValue && n2.HasValue && n3.HasValue;
            var validF =(Exceptional<Func<int, int, int, string>>)((i1, i2, i3) => $"val is {i1 + i2 + i3}");
            validF.Apply(v1)
                .Apply(v2)
                .Apply(v3)
                .Match(err => Assert.True(!isValid),
                    s => Assert.Equal($"val is {n1 + n2 + n3}", s));
        }
        [Theory]
        [InlineData(null)]
        [InlineData(6)]
        public void Assert_Select_ET_Transfrom_ER(int? num)
        {
            Exceptional<int> E1 = num.HasValue ? (Exceptional<int>)num : (ErrorException)("num is null");
            var res = E1.Select<int,double>(i => (double)i / 2.0);
            res.Match(
                err => Assert.Equal("num is null", err.Message),
                d => Assert.Equal((double)num / 2.0,d));
        }
        [Theory]
        [InlineData(4)]
        [InlineData(null)]
        public void Assert_SelectMany_evaluate_inner_nested_Exceptional(int? num)
        {
            var ex1 = num.HasValue?(Exceptional<int>)num:(ErrorException)("num is null");
            Exceptional<Exceptional<int>> nested = (Exceptional<Exceptional<int>>)ex1;
            var res = from a in nested
                      from b in a
                      select b + 5;
            res.Match(err => Assert.True(num == null),
                v => Assert.Equal(num + 5, v));
        }
    }
}
