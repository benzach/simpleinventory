using Functional.Lib.Functional;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using System.Diagnostics;
using static Functional.Lib.Functional.F;
using static Functional.Lib.Functional.Validation;
namespace SimpleInventoryTest
{
    public class ValidationTest
    {
        //public static string[] e = new[] { "this is an error" };

        [Fact]
        public void InitializeInvalidationWithErrorShouldReturnInvalid()
        {
            Error e = "this is an error";
            //var err = new Validation.Invalid((new[] { e }));
            List<Error> lst = new List<Error>(new Error[] { e });
            Validation<int> validNumber = new Validation.Invalid(lst);
            validNumber.Match(
                er => Assert.True(er.FirstOrDefault().Messsage == "this is an error"),
                _ => Assert.False(true)
                );
        }
        [Fact]
        public void InitValidationWithErrorShouldReturnInvalid()
        {
            Error e = "this is an error";
            Validation<int> InvalidNmber = e;
            InvalidNmber.Match(
                _ => Assert.True(true),
                _ => Assert.False(true));
        }
        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void initValidationWithTReturValid(int num)
        {

            Validation<int> valid = num;
            valid.Match(
                _ => Assert.False(true),
                val => Assert.Equal(num,val));
        }
        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        public void Bind_TransForm_ValT_To_ValR(int num)
        {
            Validation<int> validNum = num;
            var oval=validNum.Bind(val =>Valid( $"this is num={val}"));
            oval.Match(
                _ => Assert.False(true),
                val => Assert.Equal($"this is num={num}", val));
            
        }
        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        public void map_transform_inner_data_From_ValT_To_ValR(int num)
        {
            Validation<int> validnum = num;
            var sqr=validnum.Map(x => x * x);
            sqr.Match(
                _ => Assert.False(true),
                val => Assert.Equal(num * num, val));
        }
        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(null)]
        public void Apply_first_Arg_To_func(int? num)
        {
            Func<int,int> doub=n => n * n;
            Validation<Func<int, int>> validFunc = doub;
            Validation<int> validNum = num.HasValue ? Valid(num.Value) : (Error)("this is null");
            var a=validFunc.Apply(validNum);
            a.Match(
                err => Assert.Equal( "this is null",err.SingleOrDefault().Messsage),
                val => Assert.Equal(num*num, val));            
        }
        [Theory(DisplayName ="aggregate errors")]
        [InlineData(null,null)]
        [InlineData(null,1)]
        [InlineData(1,null)]
        [InlineData(3,4)]
        public void Apply_two_arg_one_at_a_time(int? val1,int? val2)
        {

            Func<int,int,int> multiply=(num1, num2) => num1 * num2;
            Validation<Func<int, int, int>> valF =val1.HasValue && val2.HasValue ?multiply:(Validation<Func<int, int, int>>) (Error)("Multiply Operation failed");
            Validation<int> Val1 = val1.HasValue?(Validation<int>)val1.Value:(Error)("Val1 is null");

            Validation<int> Val2 = val2.HasValue?(Validation<int>)val2.Value:(Error)("Val2 is null");
            var one = valF.Apply(Val1);
            int nonNull = 0;
            nonNull += val1.HasValue ? 0 : 1;
            nonNull += val2.HasValue ? 0 : 1;
            one.Apply(Val2).Match(
                er => Assert.Equal(nonNull>0?nonNull+1:0, er.Count()),
                val => Assert.Equal(val1.Value * val2.Value, val)
                );
        }
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? Age { get; set; }
            public bool? IsMarried { get; set; }
        }
        public static IEnumerable<object[]> GetData()
        {
            var alldata = new List<Object[]>
            {
                new object[]{1,null,null,null },
                new object[]{2,"Tom",null,null},
                new object[]{3,"Bob",3,null},
                new object[]{4,"Marry",4,true}
            };
            return alldata;
        }
        [Theory(DisplayName ="Validate Person Record")]
        [MemberData(nameof(GetData))]
        public void Validate_Person_aggregate_Error_on_Errors(int id,string name,int? age, bool? isMarried)
        {
            var Person = new Person { Id = id, Name = name, Age = age, IsMarried = isMarried };
            Func<int,string,int,bool, Person> Validate = (mid,mname,mage,ismarried)=>new Person { Id = mid, Name = mname, Age = mage, IsMarried = isMarried };
            Validation<Func<int, string, int, bool, Person>> ValF = Validate;
            Validation<int> idVal = id > 0 ? (Validation<int>)id : (Error)("Id has to be positive integer");
            Validation<string> NameVal = string.IsNullOrEmpty(Person.Name) ? (Error)("Name is empty") :(Validation<string>) Person.Name;
            Validation<int> AgeVal = Person.Age.HasValue?(Validation<int>)Person.Age: (Error)("Age is null") ;
            Validation<bool> isMarriedVal = Person.IsMarried.HasValue?(Validation<bool>) Person.IsMarried: (Error)("IsMarried is null") ;
            int numError = 0;
            numError += id < 0 ? 1 : 0;
            numError += string.IsNullOrEmpty(name) ? 1 : 0;
            numError += !age.HasValue ? 1 : 0;
            numError += !isMarried.HasValue ? 1 : 0;
            var a = ValF
                    .Apply(id > 0 ? (Validation<int>)id : (Error)("Id has to be positive integer"))
                    .Apply(string.IsNullOrEmpty(Person.Name)?(Error)("Name is empty"):(Validation<string>)Person.Name)//NameVal
                    .Apply(Person.Age.HasValue?(Validation<int>)Person.Age:(Error)("Age is null"))//AgeVal
                    .Apply(Person.IsMarried.HasValue?(Validation<bool>)Person.IsMarried:(Error)("isMarried is null"))//isMarriedval
                    .Match(
                        er=>Assert.Equal(numError,er.Count()),
                        val=>Assert.Equal(Person.Id,val.Id)
                    );
        }
        [Theory]
        [InlineData(null,0)]
        [InlineData(5,0)]
        [InlineData(5,2)]
        public void Select_Takes_a_Func_To_TransformT_To_R(int? val1,int val2)
        {
            Validation<int> valInt = val1.HasValue ? (Validation<int>)val1.Value : (Error)("Not valid number");
            //var valIntX=valInt2.Bind(num => num > 0 ? (Validation<int>)num : (Error)("Num has to be greater than 0"));
            var res = valInt.Select(ival => val2 > 0 ? (Validation<double>)((double)ival / (double)val2) : (Error)("cant divide by 0"));
            res.Match(
                _ => Assert.True(!val1.HasValue || val2 <= 0),
                v => Assert.Equal((double)val1.Value / (double)val2, v)
                );
        }
        [Fact]
        public void SelectMany_Flatten_array_within_array()
        {
            Validation<int> valList = 5;
            //var lst = from a in valList


            var res=valList.SelectMany(val => (Validation<string>)$"this val={val}", (i, s) => $"{i}=={s}");
            res.Match(
                _ => Assert.False(true),
                r => Assert.Equal("5==this val=5", r));
        }
        [Theory]
        [InlineData(5)]
        [InlineData(null)]
        public void SelectMany_Validation_Option_To_Validation(int? val)
        {
            Option<int> optInt = val.HasValue?Some(val.Value):None;
            Validation<Option<int>> validOpInt = optInt;
            var res = from a in validOpInt                      
                      select a.Match(()=>(Error)("null val"),v=>(Validation<int>)v);

            res.Match(
                _ => Assert.True(!val.HasValue),
                vo => Assert.Equal(vo, val.Value)
                );
            
        }
        [Theory]
        [InlineData(null,null,null)]
        [InlineData(1,null,null)]
        [InlineData(1,2,null)]
        [InlineData(1,2,3)]
        public void SelectMany_combine_many_Validation_into_single_scope_And_err_does_not_aggregate(int? num1,int?num2,int?num3)
        {
            var v1 = Validation.FromNull(num1);
            var v2 = Validation.FromNull(num2);
            var v3 = Validation.FromNull(num3);
            var res = from n1 in v1
                      from n2 in v2
                      from n3 in v3
                      select n1 + n2 + n3;
            var Invalids = 0;
            Invalids += num1.HasValue ? 1 : 0;
            Invalids += num2.HasValue ? 1 : 0;
            Invalids += num3.HasValue ? 1 : 0;
            res.Match(er => Assert.True(1== er.Count()), //since errs do not aggregate as soon as there's error it evaluates to invalid
                v => Assert.Equal(num1 + num2 + num3, v));
        }
    }
}
