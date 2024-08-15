using System;
using LoanRepaymentApi.Common;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestRetailPriceIndex : IRetailPriceIndex
    {
        public decimal Get(DateTime periodDate)
        {
            return 0.015m;
        }
    }
}
