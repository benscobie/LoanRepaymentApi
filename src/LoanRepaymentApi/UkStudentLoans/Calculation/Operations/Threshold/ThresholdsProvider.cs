namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class ThresholdsProvider : IThresholdsProvider
{
    public List<ThresholdBand> Get()
    {
        return new List<ThresholdBand>
        {
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                DateFrom = new DateTimeOffset(2023, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 22015
            },
            new()
            {
                LoanType = UkStudentLoanType.Type2,
                DateFrom = new DateTimeOffset(2023, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 27295
            },
            new()
            {
                LoanType = UkStudentLoanType.Type4,
                DateFrom = new DateTimeOffset(2023, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 27660
            },
            new()
            {
                LoanType = UkStudentLoanType.Postgraduate,
                DateFrom = new DateTimeOffset(2023, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 21000
            },
            new()
            {
                LoanType = UkStudentLoanType.Type5,
                DateFrom = new DateTimeOffset(2023, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                DateTo = null,
                Threshold = 25000
            }
        };
    }
}