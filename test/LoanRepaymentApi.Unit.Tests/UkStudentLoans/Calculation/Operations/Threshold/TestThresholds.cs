using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using LoanRepaymentApi.UkStudentLoans;
using System;
using System.Collections.Generic;

namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.Threshold
{
    internal static class TestThresholds
    {
        public static List<ThresholdBand> Get()
        {
            return new List<ThresholdBand>
            {
                new ThresholdBand
                {
                    LoanType = UkStudentLoanType.Type1,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 20195
                },
                new ThresholdBand
                {
                    LoanType = UkStudentLoanType.Type2,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 27295
                },
                new ThresholdBand
                {
                    LoanType = UkStudentLoanType.Type4,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 25375
                },
                new ThresholdBand
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 21000
                }
            };
        }
    }
}
