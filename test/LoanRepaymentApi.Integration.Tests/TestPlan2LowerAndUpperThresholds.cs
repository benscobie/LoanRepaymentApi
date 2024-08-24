using System;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestPlan2LowerAndUpperThresholds : IPlan2LowerAndUpperThresholds
    {
        public Plan2LowerAndUpperThreshold Get(DateTime periodDate)
        {
            return new Plan2LowerAndUpperThreshold { LowerThreshold = 27295, UpperThreshold = 49130 };
        }
    }
}
