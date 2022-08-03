namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;

public class SalaryOperation : ISalaryOperation
{
    public int Execute(SalaryOperationFact fact)
    {
        var salaryAdjustment = fact.SalaryAdjustments.SingleOrDefault(x => x.Date.Year == fact.PeriodDate.Year && x.Date.Month == fact.PeriodDate.Month);

        if (salaryAdjustment != null)
        {
            return (int)(fact.CurrentSalary + (fact.CurrentSalary * salaryAdjustment.Value));
        }

        return fact.CurrentSalary;
    }
}