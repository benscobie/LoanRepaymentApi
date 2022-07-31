namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanResult
{
    public int Period { get; set; }
    
    public DateTimeOffset PeriodDate { get; set; }

    public List<UkStudentLoanTypeResult> LoanResults { get; set; }

    public decimal AggregatedPaidInPeriod => LoanResults.Sum(x => x.PaidInPeriod);

    public decimal AggregatedInterestAppliedInPeriod => LoanResults.Sum(x => x.InterestAppliedInPeriod);
}