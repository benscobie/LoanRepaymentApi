namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;

public class SalaryOperationFact
{
    public DateTimeOffset PeriodDate { get; set; }

    public int CurrentSalary { get; init; }

    public List<Adjustment> SalaryAdjustments { get; init; } = new();
}