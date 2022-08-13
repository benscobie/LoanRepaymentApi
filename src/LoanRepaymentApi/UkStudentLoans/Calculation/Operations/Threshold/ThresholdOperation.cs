namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class ThresholdOperation : IThresholdOperation
{
    public int Execute(ThresholdOperationFact fact)
    {
        var thresholdBandWithinPeriod = Thresholds.SingleOrDefault(x =>
            x.LoanType == fact.LoanType && fact.PeriodDate >= x.DateFrom && x.DateTo != null &&
            fact.PeriodDate < x.DateTo);

        if (thresholdBandWithinPeriod != null)
        {
            return thresholdBandWithinPeriod.Threshold;
        }

        var mostRecentThresholdBand = Thresholds.Single(x =>
            x.LoanType == fact.LoanType && fact.PeriodDate >= x.DateFrom &&
            x.DateTo == null);

        if (fact.Period == 1)
        {
            return mostRecentThresholdBand.Threshold;
        }

        var previousPeriodsThreshold =
            fact.PreviousProjections.Single(x => x.LoanType == fact.LoanType && x.Period == fact.Period - 1).Threshold;

        var lastThresholdChange = fact.PreviousProjections.OrderByDescending(x => x.Period)
            .FirstOrDefault(x =>
                x.LoanType == fact.LoanType && x.Period < fact.Period && x.Threshold != previousPeriodsThreshold)
            ?.Period + 1 ?? 1;

        if (lastThresholdChange <= fact.Period - 12)
        {
            return (int)Math.Round(previousPeriodsThreshold + (previousPeriodsThreshold * fact.AnnualEarningsGrowth));
        }

        return previousPeriodsThreshold;
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