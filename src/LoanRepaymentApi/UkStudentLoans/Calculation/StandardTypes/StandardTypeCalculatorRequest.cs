namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public class StandardTypeCalculatorRequest
{
    public StandardTypeCalculatorRequest(PersonDetails personDetails)
    {
        PersonDetails = personDetails;
    }
    
    public int Period { get; init; }

    public DateTimeOffset PeriodDate { get; init; }

    public PersonDetails PersonDetails { get; init; }

    public List<UkStudentLoan> Loans { get; init; } = new();

    public IList<UkStudentLoanTypeResult> PreviousPeriods { get; init; } = new List<UkStudentLoanTypeResult>();
}