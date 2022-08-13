namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;

public class SalaryOperation : ISalaryOperation
{
    public int Execute(SalaryOperationFact fact)
    {
        var growthPercentage = 0m;

        var salaryAdjustment = fact.SalaryAdjustments.SingleOrDefault(x => x.Date.Year == fact.PeriodDate.Year && x.Date.Month == fact.PeriodDate.Month);
        if (salaryAdjustment != null)
        {
            growthPercentage = salaryAdjustment.Value;
        }
        else
        {
            var lastSalaryChange =
                fact.Results.SingleOrDefault(x => x.Salary != fact.CurrentSalary && x.Period < fact.Period);
            if (lastSalaryChange != null && lastSalaryChange.Period + 1 <= fact.Period - 12)
            {
                growthPercentage = fact.AnnualEarningsGrowth;
            }
        }

        return (int)Math.Round(fact.CurrentSalary + (fact.CurrentSalary * growthPercentage));
    }
}