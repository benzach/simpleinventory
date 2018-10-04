using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Functional.Lib.Functional.F;

namespace SimpleInventoryTest
{
      
    public class MyList
    {
        private List<(int val,DateTime date)> lst = new List<(int val, DateTime date)>();
        public List<(int val,DateTime date)> List { get { return this.lst; } }
        public void Add(int i)
        {
            Thread.Sleep(5000);
            lst.Add((val:i,date:DateTime.Now));
            var res = lst.Select(x => $"{x.val}-{x.date}");
            Debug.WriteLine(string.Join('-', res) );
            
        }
    }
    public class LockTest
    {
        [Fact]
        public void LockShouldOnlyAllowOneOperationAtATime()
        {
            var lst = new MyList();
            List<Task> tal = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                var val = i;
                var ta= new Task(() => WithLock(()=>{ lst.Add(val); }));
                ta.Start();
                
                tal.Add(ta);
                Thread.Sleep(500);
            }
            Task.WaitAll(tal.ToArray());
            Debug.Write("complete!!!");
            Assert.True(tal.Count == 5);
            Assert.Equal(5, lst.List.Count);
            var validint= new List<int>(new int[]{ 0,1,2,3,4});
            var distinct = lst.List.Select(x=>x.val).Distinct();

            Assert.Contains(lst.List.Select(x=>x.val), i => validint.Contains(i));
            Assert.Equal(lst.List.Count, distinct.Count());
            Debug.WriteLine(string.Join('-', lst.List.Select(x=>$"{x.val}-{x.date}")));
        }

        [Fact]
        public void LockShouldOnlyAllowOneOperationAtATime1()
        {
            var lst = new MyList();
            List<Thread> ts = new List<Thread>();
            for (int i = 0; i < 5; i++)
            {
                var val = i;
                var t = new Thread(() =>
                    {
                        WithLock(() => { lst.Add(val);  });
                        //Debug.WriteLine(string.Join(',', res));
                    });
                t.Start();
                ts.Add(t);
                Thread.Sleep(500);
                //t.Start();
                //ts.Add(t);

                //var val2 = val * 2;
                //new Thread(() => {
                //    WithLock(() => { lst.Add(val2); return Unit; });
                //    //Debug.WriteLine(string.Join(',', res));
                //}).Start();

                //Thread.Sleep(1000);
            }
            foreach (var myt in ts)
            {
                myt.Join();
            }
            Debug.Write("complete!!!");
            Assert.True(ts.Count == 5);
            Assert.Equal(5, lst.List.Count);
            var validint = new List<int>(new int[] { 0, 1, 2, 3, 4 });
            var distinct = lst.List.Select(x => x.val).Distinct();

            Assert.Contains(lst.List.Select(x => x.val), i => validint.Contains(i));
            Assert.Equal(lst.List.Count, distinct.Count());
            Debug.WriteLine(string.Join('-', lst.List.Select(x => $"{x.val}-{x.date}")));
        }

    }
}
