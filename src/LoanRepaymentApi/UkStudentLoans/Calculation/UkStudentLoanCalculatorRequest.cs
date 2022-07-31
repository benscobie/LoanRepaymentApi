namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanCalculatorRequest
{
    public Income Income { get; set; }

    public List<UkStudentLoan> Loans { get; set; } = new List<UkStudentLoan>();
}