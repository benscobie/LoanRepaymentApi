namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;

public class VoluntaryRepaymentOperationFact
{
    public DateTime PeriodDate { get; set; }

    public List<UkStudentLoan> Loans { get; set; } = new();

    public List<Calculation.VoluntaryRepayment> VoluntaryRepayments { get; set; } = new();

    public Dictionary<UkStudentLoanType, decimal> LoanInterestRates { get; set; } = new();

    public Dictionary<UkStudentLoanType, decimal> LoanBalancesRemaining { get; set; } = new();
}