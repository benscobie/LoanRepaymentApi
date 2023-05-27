namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;

public class SalaryOperation : ISalaryOperation
{
    public int Execute(SalaryOperationFact fact)
    {
        var salaryAdjustment = fact.SalaryAdjustments.SingleOrDefault(x => x.Date.Year == fact.PeriodDate.Year && x.Date.Month == fact.PeriodDate.Month);
        if (salaryAdjustment != null)
        {
            return (int)Math.Round(salaryAdjustment.Value);
        }
        else
        {
            var lastSalaryChange =
                fact.Results.OrderByDescending(x => x.Period).FirstOrDefault(x => x.Salary != fact.PreviousPeriodSalary && x.Period < fact.Period)?.Period + 1 ?? 1;

            var growthPercentage = 0m;
            if (lastSalaryChange <= fact.Period - 12)
            {
                growthPercentage = fact.SalaryGrowth;
            }

            return (int)Math.Round(fact.PreviousPeriodSalary + (fact.PreviousPeriodSalary * growthPercentage));
        }
    }
}