namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public class StandardTypeCalculatorRequest
{
    public int Period { get; set; }

    public DateTimeOffset PeriodDate { get; set; }

    public Income Income { get; set; }

    public List<UkStudentLoan> Loans { get; set; } = new();

    public IList<UkStudentLoanTypeResult> PreviousPeriods { get; set; } = new List<UkStudentLoanTypeResult>();
}