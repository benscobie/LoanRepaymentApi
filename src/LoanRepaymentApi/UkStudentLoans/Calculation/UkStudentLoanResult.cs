namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanResult
{
    public int Period { get; set; }

    public DateTimeOffset PeriodDate { get; set; }
    
    public int Salary { get; set; }

    public List<UkStudentLoanProjection> Projections { get; set; } = new List<UkStudentLoanProjection>();

    public decimal AggregatedPaidInPeriod => Projections.Sum(x => x.Paid);

    public decimal AggregatedInterestAppliedInPeriod => Projections.Sum(x => x.InterestApplied);
}