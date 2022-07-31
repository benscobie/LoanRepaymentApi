namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public class StandardTypeCalculatorRequest
{
    public StandardTypeCalculatorRequest(Income income)
    {
        Income = income;
    }
    
    public int Period { get; init; }

    public DateTimeOffset PeriodDate { get; init; }

    public Income Income { get; init; }

    public List<UkStudentLoan> Loans { get; init; } = new();

    public IList<UkStudentLoanTypeResult> PreviousPeriods { get; init; } = new List<UkStudentLoanTypeResult>();
}