namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;

public class SalaryOperationFact
{
    public int Period { get; set; }

    public DateTimeOffset PeriodDate { get; set; }

    public int PreviousPeriodSalary { get; init; }

    public decimal SalaryGrowth { get; set; }

    public List<UkStudentLoanResult> Results { get; set; } = new();

    public List<Adjustment> SalaryAdjustments { get; init; } = new();
}