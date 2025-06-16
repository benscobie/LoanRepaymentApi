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
            return
            [
                new() {
                    LoanType = UkStudentLoanType.Type1,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 20195
                },
                new() {
                    LoanType = UkStudentLoanType.Type2,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 27295
                },
                new() {
                    LoanType = UkStudentLoanType.Type4,
                    DateFrom = new DateTime(2022, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 25375
                },
                new() {
                    LoanType = UkStudentLoanType.Type5,
                    DateFrom = new DateTime(2023, 04, 06, 0, 0, 0),
                    DateTo = null,
                    Threshold = 25000
                }
            ];
        }
    }
}
