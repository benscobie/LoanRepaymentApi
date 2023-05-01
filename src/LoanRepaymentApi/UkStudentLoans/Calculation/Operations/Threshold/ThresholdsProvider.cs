namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

using LoanRepaymentApi.UkStudentLoans;

public class ThresholdsProvider : IThresholdsProvider
{
    public List<ThresholdBand> Get()
    {
        return new List<ThresholdBand>
        {
            new ThresholdBand
            {
                LoanType = UkStudentLoanType.Type1,
                DateFrom = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 22015
            },
            new ThresholdBand
            {
                LoanType = UkStudentLoanType.Type2,
                DateFrom = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 27295
            },
            new ThresholdBand
            {
                LoanType = UkStudentLoanType.Type4,
                DateFrom = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 27660
            },
            new ThresholdBand
            {
                LoanType = UkStudentLoanType.Postgraduate,
                DateFrom = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 21000
            }
        };
    }
}