namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanCalculatorRequest
{
    public UkStudentLoanCalculatorRequest(Income income, List<UkStudentLoan> loans)
    {
        Income = income;
        Loans = loans;
    }
    
    public Income Income { get; }

    public List<UkStudentLoan> Loans { get; }
}