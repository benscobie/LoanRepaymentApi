namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public class StandardTypeCalculatorRequest
{
    public StandardTypeCalculatorRequest(PersonDetails personDetails)
    {
        PersonDetails = personDetails;
    }

    public int Period { get; init; }

    public DateTime PeriodDate { get; init; }

    public PersonDetails PersonDetails { get; }

    public List<UkStudentLoan> Loans { get; init; } = new();

    public int Salary { get; set; }

    public List<UkStudentLoanProjection> PreviousProjections { get; init; } = new List<UkStudentLoanProjection>();

    public decimal AnnualEarningsGrowth { get; set; }
}