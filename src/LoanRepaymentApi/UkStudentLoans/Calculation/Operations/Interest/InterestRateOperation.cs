using LoanRepaymentApi.Common;

namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

public class InterestRateOperation : IInterestRateOperation
{
    private readonly decimal _prevailingMarketRateCap;
    private readonly IRetailPriceIndex _retailPriceIndex;
    private readonly IPlan2LowerAndUpperThresholds _plan2LowerAndUpperThresholds;
    private readonly IPlan1InterestRate _plan1InterestRate;
    private readonly IPlan4InterestRate _plan4InterestRate;

    public InterestRateOperation(
        IPrevailingMarketRateCap prevailingMarketRateCap,
        IPlan1InterestRate plan1InterestRate,
        IPlan4InterestRate plan4InterestRate,
        IRetailPriceIndex retailPriceIndex,
        IPlan2LowerAndUpperThresholds plan2LowerAndUpperThresholds)
    {
        _prevailingMarketRateCap = prevailingMarketRateCap.Get();
        _retailPriceIndex = retailPriceIndex;
        _plan1InterestRate = plan1InterestRate;
        _plan4InterestRate = plan4InterestRate;
        _plan2LowerAndUpperThresholds = plan2LowerAndUpperThresholds;
    }

    public decimal Execute(InterestRateOperationFact fact)
    {
        var rpi = _retailPriceIndex.Get(fact.PeriodDate);

        if (fact.LoanType == UkStudentLoanType.Type1)
        {
            return _plan1InterestRate.Get(fact.PeriodDate);
        }
        else if (fact.LoanType == UkStudentLoanType.Type4)
        {
            return _plan4InterestRate.Get(fact.PeriodDate);
        }
        else if (fact.LoanType == UkStudentLoanType.Type5)
        {
            return decimal.Min(rpi, _prevailingMarketRateCap);
        }
        else if (fact.LoanType == UkStudentLoanType.Postgraduate)
        {
            return decimal.Min(rpi + 0.03m, _prevailingMarketRateCap);
        }
        else if (fact.LoanType == UkStudentLoanType.Type2)
        {
            var thresholds = _plan2LowerAndUpperThresholds.Get(fact.PeriodDate);

            if (fact.Salary <= thresholds.LowerThreshold)
            {
                return decimal.Min(rpi, _prevailingMarketRateCap);
            }
            else if (fact.Salary < thresholds.UpperThreshold)
            {
                if ((fact.StudyingPartTime && fact.PeriodDate < fact.CourseStartDate!.Value.AddYears(4)) ||
                    !fact.StudyingPartTime && fact.PeriodDate < new DateTime(
                        fact.CourseEndDate!.Value.Year + (fact.CourseEndDate!.Value.Month < 4 ? 0 : 1), 04, 01, 0, 0, 0))
                {
                    return decimal.Min(rpi + 0.03m, _prevailingMarketRateCap);
                }

                decimal percentagePortionOfIncome = (fact.Salary - thresholds.LowerThreshold) / ((thresholds.UpperThreshold - 1) - thresholds.LowerThreshold);
                var rate = Math.Round(percentagePortionOfIncome * 0.03m, 3);

                return decimal.Min(rpi + rate, _prevailingMarketRateCap);
            }
            else
            {
                return decimal.Min(rpi + 0.03m, _prevailingMarketRateCap);
            }
        }

        throw new InvalidOperationException();
    }
}
