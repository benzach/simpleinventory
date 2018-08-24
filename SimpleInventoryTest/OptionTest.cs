using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Functional.Lib.Functional;
using static Functional.Lib.Functional.F;

namespace SimpleInventoryTest
{
    public class OptionTest
    {
        [Fact]
        public void Option_Of_None_Matches_None()
        {
            Option<int> ONone = None;
            ONone.Match(() => Assert.True(true), val => Assert.False(true));
        }
        [Fact]
        public void Option_Of_T_Implict_Create_O_T_By_Assign_T()
        {
            Option<string> os = "this is a string";
            os.Match(()=> Assert.False(true), va => Assert.True(va is string));
        }
        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void Option_Of_T_Return_Some_if_Valid_None_If_invalid(int? num)
        {
            var val = OptionExt.FromNullable(num);
            val.Match(() => Assert.True(num == null), v1 => Assert.Equal(num.Value, v1.Value));
        }
        [Theory(DisplayName ="Bind Takes OT and trasnform Func T change T to OR")]
        [InlineData(null)]
        [InlineData(4)]
        public void Option_Bind_takes_OT_Xform_OR(int? num)
        {
            var oval = OptionExt.FromNullable(num);
            var xform = oval.Bind(val => val == null ? None : Some($"the val is {val.Value}"));
            xform.Match(() => Assert.True(num == null), x => Assert.True(x is string));
        }
        [Theory(DisplayName ="Map takes OT and transform func change T to R")]
        [InlineData(null)]
        [InlineData(5)]
        public void Option_Map_OT_F_T_to_R(int? num)
        {
            var opv = OptionExt.FromNullable(num);
            var res = opv.Map(val => $"the val is {val}");
            res.Match(() => Assert.True(num == null),
                s => Assert.True(s is string));
        }
        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void Apply_accept_OT_To_A_Func_Returns_New_Func_With_OT_Applied(int? num)
        {
            var ot = OptionExt.FromNullable(num);
            Option<Func<int?, string>> OFunc =Some<Func<int?,string>>(val => $"this is the val {val}");

            var res=OFunc.Apply(ot);
            res.Match(
                () => Assert.True(num == null),
                val => Assert.Equal(val, $"this is the val {num.Value}"));
        }
        [Theory]
        [InlineData(null,null)]
        [InlineData(null,5)]
        [InlineData(6,null)]
        [InlineData(6,5)]
        public void Apply_2_param_To_Func_one_At_a_time(int? num1,int? num2)
        {
            var oval1 = OptionExt.FromNullable(num1);
            var oval2 = OptionExt.FromNullable(num2);
            var isnull = oval1 == null || oval2 == null;
            var OptionSum = Some<Func<int?, int?, int?>>((n1, n2) => n1 == null || n2 == null ?(int?) null : n1.Value + n2.Value);
            OptionSum.Apply(oval1)
                      .Apply(oval2)
                      .Match(() => Assert.True(isnull),
                            val => Assert.Equal(num1.Value + num2.Value, val.Value));
        }
        [Theory]
        [InlineData(null,null,null)]
        [InlineData(1,null,null)]
        [InlineData(1,2,null)]
        [InlineData(1,2,3)]
        public void Apply_3_parameters_one_at_a_time(int? num1,int? num2,int? num3)
        {
            var ov1 = OptionExt.FromNullable(num1);
            var ov2 = OptionExt.FromNullable(num2);
            var ov3 = OptionExt.FromNullable(num3);
            var OFunc = Some<Func<int?, int?, int?, int?>>((n1, n2, n3) => n1 == null || n2 == null || n3 == null ?(int?)null : n1.Value + n2.Value + n3.Value);
            var isnull = ov1 == null || ov2 == null || ov3 == null;
            OFunc.Apply(ov1)
                .Apply(ov2)
                .Apply(ov3)
                .Match(() => Assert.True(isnull),
                val => Assert.Equal(num1.Value + num2.Value + num3.Value, val.Value));
        }
        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void Select_Transform_T_into_OR(int? num)
        {
            var oval = OptionExt.FromNullable(num);
            var res=oval.Select<int?,string>(v => v == null ? null :$"this val={v}");
            res.Match(() => Assert.True(num == null),
                v => Assert.Equal($"this val={num.Value}", v));
        }
        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void Select_many_flatten_option(int? num)
        {
            var nestedobj = Some<Validation<int>>(num==null?(Error)("not a valid number"):(Validation<int>)num.Value );
            var res = from a in nestedobj
                      select a.Match(er=>None,val=>Some($"val is {val}"));
            res.Match(() => Assert.True(num == null),
                v => Assert.Equal($"val is 5", v));

        }
        [Theory]
        [InlineData(null,null,null)]
        [InlineData(1,null,null)]
        [InlineData(1,2,null)]
        [InlineData(1,2,3)]
        public void SelectMany_combine_many_Option_into_single_scope(int? num1,int? num2,int?num3)
        {
            var o1 = OptionExt.FromNullable(num1);
            var o2 = OptionExt.FromNullable(num2);
            var o3 = OptionExt.FromNullable(num3);
            var res = from a in o1
                      from b in o2
                      from c in o3
                      select a + b + c;
            var isNone = num1 == null || num2 == null || num3 == null;
            res.Match(() => Assert.True(isNone),
                v => Assert.Equal(num1+ num2 + num3, v));
        }

    }
}
