namespace LoanRepaymentApi.UkStudentLoans;

public class UkStudentLoan
{
    public UkStudentLoanType Type { get; set; }
    
    public decimal BalanceRemaining { get; set; }
    
    public decimal InterestRate { get; set; }
    
    public decimal RepaymentThreshold { get; set; }
}