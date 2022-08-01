namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public class PostgraduateCalculatorRequest
{
    public PostgraduateCalculatorRequest(PersonDetails personDetails, UkStudentLoan loan)
    {
        PersonDetails = personDetails;
        Loan = loan;
    }
    
    public int Period { get; init; }

    public DateTimeOffset PeriodDate { get; init; }

    public PersonDetails PersonDetails { get; }

    public UkStudentLoan Loan { get; }

    public IList<UkStudentLoanTypeResult> PreviousPeriods { get; init; } = new List<UkStudentLoanTypeResult>();
}