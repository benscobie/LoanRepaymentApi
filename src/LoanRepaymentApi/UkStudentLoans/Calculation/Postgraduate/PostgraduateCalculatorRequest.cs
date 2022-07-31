namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public class PostgraduateCalculatorRequest
{
    public PostgraduateCalculatorRequest(Income income)
    {
        Income = income;
    }
    
    public int Period { get; init; }

    public DateTimeOffset PeriodDate { get; init; }

    public Income Income { get; init; }

    public UkStudentLoan Loan { get; init; }

    public IList<UkStudentLoanTypeResult> PreviousPeriods { get; init; } = new List<UkStudentLoanTypeResult>();
}