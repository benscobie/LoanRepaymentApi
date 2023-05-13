using LoanRepaymentApi.Common;

namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

public class InterestRateOperation : IInterestRateOperation
{
    private readonly decimal _plan2AndPostgraduateInterestRateCap;
    private readonly decimal _retailPriceIndex;
    private readonly decimal _plan1And4InterestRate;

    public InterestRateOperation(IPlan2AndPostgraduateInterestRateCap plan2AndPostgraduateInterestRateCap, IPlan1And4InterestRate plan1And4InterestRate, IRetailPriceIndex retailPriceIndex)
    {
        _plan2AndPostgraduateInterestRateCap = plan2AndPostgraduateInterestRateCap.Get();
        _retailPriceIndex = retailPriceIndex.GetForPreviousMarch();
        _plan1And4InterestRate = plan1And4InterestRate.Get();
    }

    public decimal Execute(InterestRateOperationFact fact)
    {
        if (fact.LoanType == UkStudentLoanType.Type1 || fact.LoanType == UkStudentLoanType.Type4)
        {
            return _plan1And4InterestRate;
        }

        if (fact.LoanType == UkStudentLoanType.Type5)
        {
            return _retailPriceIndex;
        }

        if (fact.LoanType == UkStudentLoanType.Postgraduate)
        {
            return decimal.Min(_retailPriceIndex + 0.03m, _plan2AndPostgraduateInterestRateCap);
        }

        if (fact.LoanType == UkStudentLoanType.Type2)
        {
            if (fact.Salary < 27295)
            {
                return decimal.Min(_retailPriceIndex, _plan2AndPostgraduateInterestRateCap);
            }
            else if (fact.Salary < 49130)
            {
                if ((fact.StudyingPartTime!.Value && fact.PeriodDate < fact.CourseStartDate!.Value.AddYears(4)) ||
                    !fact.StudyingPartTime!.Value && fact.PeriodDate < new DateTimeOffset(
                        fact.CourseEndDate!.Value.Year + (fact.CourseEndDate!.Value.Month < 4 ? 0 : 1), 04, 01, 0, 0, 0,
                        TimeSpan.Zero))
                {
                    return decimal.Min(_retailPriceIndex + 0.03m, _plan2AndPostgraduateInterestRateCap);
                }

                decimal percentagePortionOfIncome = (fact.Salary - 27295m) / (49129m - 27295m);
                var rate = Math.Round(percentagePortionOfIncome * 0.03m, 3);

                return decimal.Min(_retailPriceIndex + rate, _plan2AndPostgraduateInterestRateCap);
            }
            else
            {
                return decimal.Min(_retailPriceIndex + 0.03m, _plan2AndPostgraduateInterestRateCap);
            }
        }

        throw new InvalidOperationException();
    }
}
