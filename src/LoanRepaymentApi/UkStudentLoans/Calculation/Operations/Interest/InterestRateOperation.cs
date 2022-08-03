namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

public class InterestRateOperation : IInterestRateOperation
{
    private const decimal RPI = 0.015m;

    public decimal Execute(InterestRateOperationFact fact)
    {
        if (fact.LoanType == UkStudentLoanType.Type1 || fact.LoanType == UkStudentLoanType.Type4)
        {
            return 0.015m;
        }

        if (fact.LoanType == UkStudentLoanType.Postgraduate)
        {
            return 0.045m;
        }

        if (fact.LoanType == UkStudentLoanType.Type2)
        {
            if (fact.Salary < 27295)
            {
                return RPI;
            }
            else if (fact.Salary < 49130)
            {
                if ((fact.StudyingPartTime!.Value && fact.PeriodDate < fact.CourseStartDate!.Value.AddYears(4)) ||
                    !fact.StudyingPartTime!.Value && fact.PeriodDate < new DateTimeOffset(
                        fact.CourseEndDate!.Value.Year + (fact.CourseEndDate!.Value.Month < 4 ? 0 : 1), 04, 01, 0, 0, 0,
                        TimeSpan.Zero))
                {
                    return 0.045m;
                }

                decimal percentagePortionOfIncome = (fact.Salary - 27295m) / (49129m - 27295m);
                var rate = Math.Round(percentagePortionOfIncome * 0.03m, 3);

                return RPI + rate;
            }
            else
            {
                return RPI + 0.03m;
            }
        }

        throw new InvalidOperationException();
    }
}