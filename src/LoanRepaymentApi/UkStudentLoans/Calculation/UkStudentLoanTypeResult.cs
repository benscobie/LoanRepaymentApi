namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanTypeResult
{
    public UkStudentLoanType LoanType { get; set; }

    public int Period { get; set; }

    public DateTime PeriodDate { get; set; }

    public decimal DebtRemaining { get; set; }

    public decimal InterestRate { get; set; }

    public decimal PaidInPeriod { get; set; }

    public decimal InterestAppliedInPeriod { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal TotalInterestPaid { get; set; }
}