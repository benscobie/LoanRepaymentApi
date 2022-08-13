namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;

public class SalaryOperationFact
{
    public int Period { get; set; }
    
    public DateTimeOffset PeriodDate { get; set; }

    public int CurrentSalary { get; init; }
    
    public decimal AnnualEarningsGrowth { get; set; }
    
    public List<UkStudentLoanResult> Results { get; set; } = new();

    public List<Adjustment> SalaryAdjustments { get; init; } = new();
}