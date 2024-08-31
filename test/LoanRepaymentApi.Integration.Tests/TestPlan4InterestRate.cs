using System;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestPlan4InterestRate : IPlan4InterestRate
    {
        public decimal Get(DateTime periodDate)
        {
            return 0.015m;
        }
    }
}
