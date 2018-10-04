using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace SimpleInventoryTest
{
    public interface IRate<TClientRequest, TClientResponse>
    {
        TClientResponse GetQuote(TClientRequest request);
    }
    public interface ITime<TClientRequest, TClientResponse>
    {
        TClientResponse GetTime(TClientRequest request);
    }
    public class EstesClientRateRequest { }
    public class EstesClientRateResponse { }
    public class EstesClientTimeRequest { }
    public class EstesClientTimeResponse { }
    public interface IEstes : IRate<EstesClientRateRequest, EstesClientRateResponse>, ITime<EstesClientTimeRequest, EstesClientTimeResponse>
    {

    }
    public class EstesAPI : IEstes
    {
        public EstesClientRateResponse GetQuote(EstesClientRateRequest request)
        { 
            throw new NotImplementedException();
        }

        public EstesClientTimeResponse GetTime(EstesClientTimeRequest request)
        {
            throw new NotImplementedException();
        }
    }
    //public class DoSomething<T>
    //{
    //    public TResult doit<TResult,T1>(Func<T1,T,TResult> getresult,T1 t)
    //    {
    //        return getresult(t,)
    //    }
    //}
    public class InheritenceTest
    {

    }
}
