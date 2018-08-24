using Functional.Lib.Functional;
using SimpleInventory.BL;
using SimpleInventory.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
//using static Functional.Lib.Functional.Option;
using static Functional.Lib.Functional.OptionExt;
using static Functional.Lib.Functional.F;
using static SimpleInventory.BL.BusinessRepoExt;
//using static SimpleInventory.BL.BusinessRepoExt;
namespace SimpleInventoryTest
{
    public class BusinessRepoTest
    {
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int CategoryType { get; set; }
            public int? ClassificationId { get; set; }
        }
        public class Student
        {
            public string StudentID { get; set; }
            public string Name { get; set; }
            public Code_Value Classification { get; set; }
        }
        public class StudentBL
        {
            public string StudentID { get; set; }
            public string Name { get; set; }
            public string classification { get; set; }
        }
        [Fact]
        public void Given_A_BusinessRepo_Of_T_Bind_To_Get_BusinessRepo_Of_R()
        {
            var p = new Person { Id = 1, Name = "Tom", Description = "Tall" };
            Repository<Person, int> personRepo = (val:p ,key:p.Id);
            var StudentRepo = personRepo.Bind<Student, string>(x 
                => x.Select(b => (
                                       val:new Student
                                       {
                                           StudentID = $"{b.Item1.Id}-{b.Item1.Name}",
                                           Name = b.Item1.Name
                                       }, 
                                       key:$"{b.Item1.Id}-{b.Item1.Name}")
                            ).ToList());
            //Assert.Contains($"{p.Id}-{p.Name}", StudentRepo.GetAll().Select(x=>x.Key));
            Assert.Contains($"{p.Id}-{p.Name}", StudentRepo.Data.Select(x => x.Key));
        }
        List<Person> lst = new List<Person>(new[] {
                new Person{Id=1,Name="Tom",Description="tall",CategoryType=1,ClassificationId=6},
                new Person{Id=1,Name="Tom",Description="tall",CategoryType=1,ClassificationId=6},
                new Person{Id=2,Name="bob",Description="big",CategoryType=1,ClassificationId=7},
                new Person{Id=3,Name="bill",Description="short",CategoryType=1,ClassificationId=8},
                new Person{Id=4,Name="Tom hank",Description="tall",CategoryType=2},
            });
        List<Code_Set> codesets = new List<Code_Set>(new[] {
            new Code_Set{Id=1, Name="PersonType",Description="PersonType"},
            new Code_Set{Id=2,Name="State",Description="State name"},
            new Code_Set{Id=3,Name="HighschoolClassification", Description="Highschool classification"},
            new Code_Set{Id=4,Name="HighschoolClass",Description="Highschool classes"}
        });
        List<Code_Value> codevalues = new List<Code_Value>(new[] {
            new Code_Value{Id=1, CodeSetId=1, Name="Student", Description="Student"},
            new Code_Value{Id=2,CodeSetId=1,Name="Teacher",Description="Teacher"},
            new Code_Value{Id=3,CodeSetId=1,Name="Administrator",Description="School Administrator"},
            new Code_Value{Id=4,CodeSetId=2,Name="AL",Description="Alabama"},
            new Code_Value{Id=5,CodeSetId=2,Name="AZ",Description="Arizon"},
            new Code_Value{Id=6,CodeSetId=3,Name="Fr",Description="Freshman"},
            new Code_Value{Id=7,CodeSetId=3,Name="So",Description="Sophmore"},
            new Code_Value{Id=8,CodeSetId=3,Name="Jr",Description="Junior"},
            new Code_Value{Id=9,CodeSetId=3,Name="Sr",Description="Senior"},
            new Code_Value{Id=10,CodeSetId=4,Name="Algebra"},
            new Code_Value{Id=11,CodeSetId=4,Name="Physics"},
            new Code_Value{Id=12,CodeSetId=4,Name="History"}
        });
        List<ClassEnrolled> ClassesEnrolled = new List<ClassEnrolled>(new[] {
            new ClassEnrolled{id=1, ClassEnroll_cv=10, studentId=1,room=101},
            new ClassEnrolled{id=2,ClassEnroll_cv=11,studentId=1,room=202},
            new ClassEnrolled{id=3,ClassEnroll_cv=12,studentId=2,room=303},
            new ClassEnrolled{id=4,ClassEnroll_cv=10,studentId=2,room=101}
        });
        List<ClassEnrolled> ModifiedClassEnrolled= new List<ClassEnrolled>(new[] {
            new ClassEnrolled{id=1, ClassEnroll_cv=11, studentId=1,room=101},
            new ClassEnrolled{id=2,ClassEnroll_cv=12,studentId=1,room=202},
            new ClassEnrolled{id=3,ClassEnroll_cv=12,studentId=2,room=303},
            new ClassEnrolled{id=4,ClassEnroll_cv=11,studentId=2,room=101}
        });
        internal class ClassEnrolled
        {
            public int id { get; set; }
            public int studentId { get; set; }
            public int ClassEnroll_cv { get; set; }
            public int room { get; set; }

        }
        [Fact]
        public void Given_A_Repo_Of_T_Return_Only_Repo_Of_R_Based_on_D()
        {
            Repository<Person, int> peopleRepo = lst.Select(x=>(val:x,key:x.Id)).ToList();
            var StudentRepo=peopleRepo.Bind<Student,string>(lstpeop => lstpeop
                                        .Where(x => x.Item1.CategoryType == 1)
                                        .Select(y => (
                                            val: new Student
                                            {
                                                StudentID = $"{y.Item1.Id}-{y.Item1.Name}",
                                                Name = y.Item1.Name
                                            },
                                            key: $"{y.Item1.Id}-{y.Item1.Name}"
                                            )
                                        ).ToList());
            var a = StudentRepo.Data.Select(x=>x.Value().Name).ToList();
            Assert.True(lst.Where(x => x.CategoryType == 1).Count()> StudentRepo.Data.Count());
            Assert.Contains(new[] { "Tom", "Bob", "Bill" }, t => a.Contains(t));
            Assert.DoesNotContain(new[] { "Tom hank" }, t => a.Contains(t));
        }
        internal class StudentSchedule
        {
            public string Id { get; set; }
            public int studentId { get; set; }
            public string studentName { get; set; }
            public string classification { get; set; }
            public List<Option<ClassEnrolledIn>> ClassesIn { get; set; }
        }
        internal class ClassEnrolledIn
        {
            public string Subject { get; set; }
            public int room { get; set; }
            //public string Teacher { get; set; }
        }

        //[Fact]
        //public void Given_Different_Repo_Use_Partial_Function_to_apply_different_repo_finally_return_BusObj()
        //{
        //    BusinessRepo<ClassEnrolled, int> classesEnrolled = ClassesEnrolled.Select(x=>(val:x,key:x.id)).ToList();
        //    BusinessRepo<Code_Value, int> ClassificationRepo = codevalues.Where(y=>y.CodeSetId==3).Select(x => (val: x, key: x.Id)).ToList();
        //    BusinessRepo<Code_Value, int> SubjectRepo = codevalues.Where(x => x.CodeSetId == 4).Select(x => (val: x, key: x.Id)).ToList();
        //    BusinessRepo<Person, int> peopleRepo = lst.Select(x => (val: x, key: x.Id)).ToList();
        //    //Func<Person, List<Code_Value>, List<ClassEnrolled>, List<Code_Value>, StudentSchedule> createStudent =
        //    //    (p, class_cvs, classes_enroll, subjs_cvs)
        //    //    => new StudentSchedule {
        //    //        Id = $"{p.Id}-{p.Name}",
        //    //        classification = class_cvs.SingleOrDefault(x => x.Id == p.ClassificationId).Name,
        //    //        studentId = p.Id,
        //    //        studentName = p.Name,
        //    //        ClassesIn = classes_enroll.Select(y => new ClassEnrolledIn {
        //    //            Subject = subjs_cvs.SingleOrDefault(z => z.Id == y.ClassEnroll_cv).Name,
        //    //            room = y.room
        //    //        }).ToList()
        //    //    };
        //    Func<Person, Code_Value, ClassEnrolled, Code_Value, StudentSchedule> createStudent =
        //        (p, class_cvs, classes_enroll, subjs_cvs)
        //        => new StudentSchedule
        //        {
        //            Id = $"{p.Id}-{p.Name}",
        //            classification = class_cvs.Name,
        //            studentId = p.Id,
        //            studentName = p.Name,
        //            ClassesIn = classes_enroll.Select(y => new ClassEnrolledIn
        //            {
        //                Subject = subjs_cvs.SingleOrDefault(z => z.Id == y.ClassEnroll_cv).Name,
        //                room = y.room
        //            }).ToList()
        //        };

        //    Func<int, int, int, int, string> getKey =
        //        (personid, classid, classesid, subjid)
        //        => $"{personid}-{classid}-{classesid}-{subjid}";
        //    BusinessRepo<
        //        Func<Person, List<Code_Value>, List<ClassEnrolled>, List<Code_Value>, StudentSchedule>,
        //        Func<int, int, int, int, string>> ComposeStudentRepo =
        //        (val: createStudent, key: getKey);

        //    var res = ComposeStudentRepo
        //            .Apply(peopleRepo);
        //    //res.Apply(ClassificationRepo);
        //    //BusinessRepo<Func<int, int, string, int, string>,
        //    //    Func<int, int, string, int, string>> ComposeStudentRepo =
        //    //(BusinessRepo<Func<int, int, string, int, string>,
        //    //Func<int, int, string, int, string>>)(val: getKey, key: getKey);

        //}
        [Fact]
        public void Given_Different_Repo_Use_Partial_Function_to_apply_different_repo_finally_return_BusObj()
        {
            Repository<ClassEnrolled, int> classesEnrolled = ClassesEnrolled.Select(x => (val: x, key: x.id)).ToList();
            Repository<Code_Value, int> ClassificationRepo = codevalues.Where(y => y.CodeSetId == 3).Select(x => (val: x, key: x.Id)).ToList();
            Repository<Code_Value, int> SubjectRepo = codevalues.Where(x => x.CodeSetId == 4).Select(x => (val: x, key: x.Id)).ToList();
            Repository<Person, int> peopleRepo = lst.Select(x => (val: x, key: x.Id)).ToList();
            Func<List<Code_Value>, List<Person>, List<StudentBL>> CreateStudent =
                (lstClassifications, people)
                => people.Select(x => new StudentBL
                {
                    StudentID = $"{x.Id}-{x.Name}",
                    Name = x.Name,
                    classification = lstClassifications.SingleOrDefault(y => y.Id == x.ClassificationId).Name
                }).ToList();

            Func<Person, Student> createStd = (p) => new Student { StudentID = $"{p.Id}-{p.Name}", Name = p.Name };
            Func<int, string> createID = i => i.ToString();
            Repository<Func<Person, Student>, Func<int, string>> CreateStudentRepo = (val: createStd, key: createID);
            var res=CreateStudentRepo.Apply(peopleRepo);

            Assert.Equal(lst.Where(x => x.CategoryType == 1).Count() ,res.Data.Count());
            Assert.Contains(new[] { "Tom", "Bob", "Bill" }, t => res.Data.Select(x=>x.Value().Name).Contains(t));

        }
        [Fact]
        public void Given_Different_Repo_Use_Partial_Function_to_apply_different_repo_finally_return_BusObj1()
        {
            Repository<ClassEnrolled, int> classesEnrolled = ClassesEnrolled.Select(x => (val: x, key: x.id)).ToList();
            Repository<Code_Value, int> ClassificationRepo = codevalues.Where(y => y.CodeSetId == 3).Select(x => (val: x, key: x.Id)).ToList();
            Repository<Code_Value, int> SubjectRepo = codevalues.Where(x => x.CodeSetId == 4).Select(x => (val: x, key: x.Id)).ToList();
            Repository<Person, int> peopleRepo = lst.Select(x => (val: x, key: x.Id)).ToList();
            Func<Person,Code_Value,Option<StudentBL>> CreateStudent =
                (p, Classification_cv)
                =>  p.ClassificationId==Classification_cv.Id?Some( new StudentBL
                {
                    StudentID = $"{p.Id}-{p.Name}",
                    Name = p.Name,
                    classification =Classification_cv.Name 
                }):None;
            Func<int,int, string> createkeyy =
                (pid, cv)
                => $"{pid}={cv}";
            Repository<Func< Person, Code_Value, Option<StudentBL>>, Func<int,int, string>> CreateStudentRepo1 =
                (val: CreateStudent, key: createkeyy);
            var a1 = CreateStudentRepo1
                .Apply(peopleRepo)
                .Apply(ClassificationRepo)
                .Data
                //.GetAll()
                .Select(x=>x.Value)
                .SelectMany(x=>x().AsEnumberable());
         }
        [Fact]
        public void Given_Different_Repo_Use_Partial_Function_to_apply_different_repo_finally_return_BusObj2()
        {
            var aaa = ClassesEnrolled.Select(x => x.id).Distinct();
            Repository<ClassEnrolled, int> classesEnrolledRepo = ClassesEnrolled.Select(x => (val: x, key: x.id)).ToBusinessRepoIII();            
            Repository<Code_Value, int> ClassificationRepo = codevalues.Where(y => y.CodeSetId == 3).Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Repository<Code_Value, int> SubjectRepo = codevalues.Where(x => x.CodeSetId == 4).Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Repository<Person, int> peopleRepo = lst.Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Func<Person, Code_Value, ClassEnrolled, Option<StudentSchedule>> CreateStudent =
                (p, Classification_cv, classEnroll)
                => p.ClassificationId == Classification_cv.Id && p.Id== classEnroll.studentId ? Some(new StudentSchedule
                {
                    Id = $"{p.Id}-{p.Name}",
                    studentName = p.Name,
                    classification = Classification_cv.Name,
                    studentId = p.Id,
                    ClassesIn =new List<Option<ClassEnrolledIn>>(new[] { classEnroll.studentId == p.Id ? Some(new ClassEnrolledIn { Subject = classEnroll.ClassEnroll_cv.ToString(), room = classEnroll.room }) : None })

                }) : None;
            Func<int, int,int, string> createkeyy =
                (pid, cv,cls)
                => $"{pid}-{cv}-{cls}";
            Repository<Func<Person, Code_Value, ClassEnrolled, Option<StudentSchedule>>, Func<int,int, int, string>> CreateStudentRepo1 =
                (val: CreateStudent, key: createkeyy);
            var x1 = CreateStudentRepo1.Apply(peopleRepo);
            var x2 = x1.Apply(ClassificationRepo);
            var x3 = x2.Apply(classesEnrolledRepo);
            var a1 = CreateStudentRepo1
                .Apply(peopleRepo)
                .Apply(ClassificationRepo)
                .Apply(classesEnrolledRepo)
                .Data
                //.GetAll()
                .Select(x=>x.Value)
                .SelectMany(x => x().AsEnumberable())
                .GroupBy(y=>y.Id);
            var test = CreateStudentRepo1
                .Apply(peopleRepo);
                //.Apply(ClassificationRepo)
                //.Apply(classesEnrolledRepo);
                //.Data;
               // .Values;

            //var b1 = a1.Select(b => b.SelectMany(y => y.ClassesIn));
            //group by id then sum up by adding all classes enrolled per student
            var c1 = a1.Select(y => new StudentSchedule
            {
                Id = y.First().Id,
                classification = y.First().classification,
                studentId = y.First().studentId,
                studentName = y.First().studentName,
                ClassesIn = y.SelectMany(z => z.ClassesIn).ToList()
            } );
            Assert.True(true);
        }

        [Fact]
        public void Given_Different_Repo_Use_Partial_Function_to_apply_different_repo_finally_return_BusObj3()
        {
            var aaa = ClassesEnrolled.Select(x => x.id).Distinct();
            Repository<ClassEnrolled, int> classesEnrolledRepo = ClassesEnrolled.Select(x => (val: x, key: x.id)).ToBusinessRepoIII();
            Repository<Code_Value, int> ClassificationRepo = codevalues.Where(y => y.CodeSetId == 3).Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Repository<Code_Value, int> SubjectRepo = codevalues.Where(x => x.CodeSetId == 4).Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Repository<Person, int> peopleRepo = lst.Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Func<Person, Code_Value, ClassEnrolled, Option<StudentSchedule>> CreateStudent =
                (p, Classification_cv, classEnroll)
                => p.ClassificationId == Classification_cv.Id && p.Id == classEnroll.studentId ? Some(new StudentSchedule
                {
                    Id = $"{p.Id}-{p.Name}",
                    studentName = p.Name,
                    classification = Classification_cv.Name,
                    studentId = p.Id,
                    ClassesIn = new List<Option<ClassEnrolledIn>>(new[] { classEnroll.studentId == p.Id ? Some(new ClassEnrolledIn { Subject = classEnroll.ClassEnroll_cv.ToString(), room = classEnroll.room }) : None })

                }) : None;
            Func<int, int, int, string> createkeyy =
                (pid, cv, cls)
                => $"{pid}-{cv}-{cls}";
            Repository<Func<Person, Code_Value, ClassEnrolled, Option<StudentSchedule>>, Func<int, int, int, string>> CreateStudentRepo1 =
                (val: CreateStudent, key: createkeyy);


            var test1 = from y1 in peopleRepo
                        from y2 in ClassificationRepo
                        //from yy3 in classesEnrolledRepo
                        select (y1.value.ClassificationId == y2.value.Id ? Some(new Student { Name = y1.value.Name, StudentID = $"{y1.value.Id}-{y2.value.Id}", Classification = y2.value }) : None, $"{y1.value.Id}-{y2.value.Id}");

            var test2 = from y1 in test1
                        from y2 in classesEnrolledRepo
                        select y2;
                        //y1.Item1.ClassificationId== y2.Item1.Id?                         
                        //Some((val:new Student { Name = y1.Item1.Name, StudentID = y1.Item1.Id.ToString(), Classification = y2.Item1 },key: y1.Item1.Id)):None;


               var x1 = CreateStudentRepo1.Apply(peopleRepo);
            var x2 = x1.Apply(ClassificationRepo);
            var x3 = x2.Apply(classesEnrolledRepo);
            var a1 = CreateStudentRepo1
                .Apply(peopleRepo)
                .Apply(ClassificationRepo)
                .Apply(classesEnrolledRepo)
                .Data
                //.GetAll()
                .Select(x=>x.Value)
                .SelectMany(x => x().AsEnumberable())
                .GroupBy(y => y.Id);
            var test = CreateStudentRepo1
                .Apply(peopleRepo);
                //.Apply(ClassificationRepo)
                //.Apply(classesEnrolledRepo);
            //.Data;
            // .Values;

            //var b1 = a1.Select(b => b.SelectMany(y => y.ClassesIn));
            //group by id then sum up by adding all classes enrolled per student
            var c1 = a1.Select(y => new StudentSchedule
            {
                Id = y.First().Id,
                classification = y.First().classification,
                studentId = y.First().studentId,
                studentName = y.First().studentName,
                ClassesIn = y.SelectMany(z => z.ClassesIn).ToList()
            });
            Assert.True(true);
        }
        [Fact]
        public void Given_Different_Repo_Use_Partial_Function_to_apply_different_repo_finally_return_BusObj4()
        {
            Repository<ClassEnrolled, int> modifiedClassRepo = ModifiedClassEnrolled.Select(x => (val: x, key: x.id)).ToBusinessRepoIII();
            Repository<ClassEnrolled, int> classesEnrolledRepo = ClassesEnrolled.Select(x => (val: x, key: x.id)).ToBusinessRepoIII();
            Repository<Code_Value, int> ClassificationRepo = codevalues.Where(y => y.CodeSetId == 3).Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Repository<Code_Value, int> SubjectRepo = codevalues.Where(x => x.CodeSetId == 4).Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Repository<Person, int> peopleRepo = lst.Select(x => (val: x, key: x.Id)).ToBusinessRepoIII();
            Func<Person, Code_Value, ClassEnrolled, Option<StudentSchedule>> CreateStudent =
                (p, Classification_cv, classEnroll)
                => p.ClassificationId == Classification_cv.Id && p.Id == classEnroll.studentId ? Some(new StudentSchedule
                {
                    Id = $"{p.Id}-{p.Name}",
                    studentName = p.Name,
                    classification = Classification_cv.Name,
                    studentId = p.Id,
                    ClassesIn = new List<Option<ClassEnrolledIn>>(new[] { classEnroll.studentId == p.Id ? Some(new ClassEnrolledIn { Subject = classEnroll.ClassEnroll_cv.ToString(), room = classEnroll.room }) : None })

                }) : None;
            Func<int, int, int, string> createkeyy =
                (pid, cv, cls)
                => $"{pid}-{cv}-{cls}";
            Repository<Func<Person, Code_Value, ClassEnrolled, Option<StudentSchedule>>, Func<int, int, int, string>> CreateStudentRepo1 =
                (val: CreateStudent, key: createkeyy);

            var xyz = from p in peopleRepo.Data.Select(x => x.Value)
                      from clsf in ClassificationRepo.Data.Select(x => x.Value)
                      from clsEn in classesEnrolledRepo.Data.Select(x => x.Value)
                      select p().ClassificationId == clsf().Id && p().Id == clsEn().studentId ?
                              Some(new StudentSchedule {
                                  Id = $"{p().Id}-{p().Name}",
                                  studentName = p().Name,
                                  classification = clsf().Name,
                                  studentId = p().Id,
                                  ClassesIn = new List<Option<ClassEnrolledIn>>(
                                      new[] { clsEn().studentId==p().Id?
                                            Some(new ClassEnrolledIn{
                                                Subject=clsEn().ClassEnroll_cv.ToString(),
                                                room=clsEn().room
                                            }):
                                            None
                                      })
                              }) :
                              None;

            //var yyy = peopleRepo.Select(x => x.Select(y => (
            //                                                val: new Student { StudentID = $"{y.Item1.Id}-{y.Item1.Name}", Name = y.Item1.Name },
            //                                                key: y.Item1.Id)
            //                                                )
            //                                                .ToList().ToBusinessRepoIII()
            //                           );

            var aa1 = CreateStudentRepo1
                .Apply(peopleRepo)
                .Apply(ClassificationRepo);

            var a1=aa1.Apply(classesEnrolledRepo)
                //.GetAll()
                .Data
                .Select(x=>x.Value)
                .SelectMany(x => x().AsEnumberable())
                .GroupBy(y => y.Id);

            //var b1 = a1.Select(b => b.SelectMany(y => y.ClassesIn));
            //group by id then sum up by adding all classes enrolled per student
            var c1 = a1.Select(y => new StudentSchedule
            {
                Id = y.First().Id,
                classification = y.First().classification,
                studentId = y.First().studentId,
                studentName = y.First().studentName,
                ClassesIn = y.SelectMany(z => z.ClassesIn).ToList()
            });
            Assert.Equal( 2,c1.Count());
            //Assert.Contains(new[] { "Tom", "Bob" }, c1.Select(x => x.studentName).ToList());
            c1.Select(x => x.studentName).ToList().ForEach(y => Assert.Contains(y.ToUpper(), new[] { "TOM", "BOB" }));
            c1.Where(x => x.studentName.ToUpper() == "TOM").SelectMany(x => x.ClassesIn).ToList().ForEach(x => x.Match(
                ()=>Assert.False(true),
                val => Assert.Contains(val.Subject, new[] {"10","11"})
                ));
            a1 = aa1.Apply(modifiedClassRepo)
                    //.GetAll()
                    .Data
                    .Select(x=>x.Value)
                    .SelectMany(x => x().AsEnumberable())
                    .GroupBy(y => y.Id);

            //var b1 = a1.Select(b => b.SelectMany(y => y.ClassesIn));
            //group by id then sum up by adding all classes enrolled per student
            c1 = a1.Select(y => new StudentSchedule
            {
                Id = y.First().Id,
                classification = y.First().classification,
                studentId = y.First().studentId,
                studentName = y.First().studentName,
                ClassesIn = y.SelectMany(z => z.ClassesIn).ToList()
            });
            Assert.Equal(2, c1.Count());
            //Assert.Contains(new[] { "Tom", "Bob" }, c1.Select(x => x.studentName).ToList());
            c1.Select(x => x.studentName).ToList().ForEach(y => Assert.Contains(y.ToUpper(), new[] { "TOM", "BOB" }));
            c1.Where(x => x.studentName.ToUpper() == "TOM").SelectMany(x => x.ClassesIn).ToList().ForEach(x => x.Match(
                () => Assert.False(true),
                val => Assert.Contains(val.Subject, new[] { "11", "12"})
                ));

        }
    }
}
