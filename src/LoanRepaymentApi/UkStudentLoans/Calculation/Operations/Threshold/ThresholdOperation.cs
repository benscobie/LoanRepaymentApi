namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class ThresholdOperation : IThresholdOperation
{
    private readonly List<ThresholdBand> _thresholds;

    public ThresholdOperation(IThresholdsProvider thresholdsProvider)
    {
        _thresholds = thresholdsProvider.Get();
    }

    public int Execute(ThresholdOperationFact fact)
    {
        var thresholdBandWithinPeriod = _thresholds.SingleOrDefault(x =>
            x.LoanType == fact.LoanType && fact.PeriodDate >= x.DateFrom && x.DateTo != null &&
            fact.PeriodDate < x.DateTo);

        if (thresholdBandWithinPeriod != null)
        {
            return thresholdBandWithinPeriod.Threshold;
        }

        var mostRecentThresholdBand = _thresholds.Single(x =>
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
}