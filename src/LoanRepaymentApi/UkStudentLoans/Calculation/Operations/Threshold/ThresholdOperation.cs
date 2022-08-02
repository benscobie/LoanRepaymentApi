namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class ThresholdOperation : IThresholdOperation
{
    public int Execute(ThresholdOperationFact fact)
    {
        return Thresholds.Single(x =>
            x.LoanType == fact.LoanType && fact.PeriodDate >= x.DateFrom &&
            (x.DateTo == null || fact.PeriodDate < x.DateTo)).Threshold;
    }
    
    private List<ThresholdBand> Thresholds => new()
    {
        new ThresholdBand
        {
            LoanType = UkStudentLoanType.Type1,
            DateFrom = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            DateTo = null,
            Threshold = 20195
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
            Threshold = 25375
        },
        new ThresholdBand
        {
            LoanType = UkStudentLoanType.Postgraduate,
            DateFrom = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            DateTo = null,
            Threshold = 21000
        }
    };
    

    private class ThresholdBand
    {
        public DateTimeOffset DateFrom { get; init; }
        
        public DateTimeOffset? DateTo { get; init; }
        
        public UkStudentLoanType LoanType { get; init; }
        
        public int Threshold { get; init; }
    }
}