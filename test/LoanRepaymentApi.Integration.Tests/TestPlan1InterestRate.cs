using System;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestPlan1InterestRate : IPlan1InterestRate
    {
        public decimal Get(DateTime periodDate)
        {
            return 0.015m;
        }
    }
}
