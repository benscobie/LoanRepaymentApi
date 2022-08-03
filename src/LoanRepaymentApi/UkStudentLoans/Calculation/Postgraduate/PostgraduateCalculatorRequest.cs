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
    
    public int Salary { get; set; }

    public IList<UkStudentLoanProjection> PreviousProjections { get; init; } = new List<UkStudentLoanProjection>();
}