using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using System;
using System.Collections.Generic;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestThresholdsProvider : IThresholdsProvider
    {
        public List<ThresholdBand> Get()
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
