namespace LoanRepaymentApi.UkStudentLoans;

public class UkStudentLoanResultDto
{
    public int Period { get; set; }

    public DateTimeOffset PeriodDate { get; set; }

    public int Salary { get; set; }

    public List<UkStudentLoanProjectionDto> Projections { get; set; } = new();

    public decimal AggregatedDebtRemaining { get; set; }

    public decimal AggregatedPaidInPeriod { get; set; }

    public decimal AggregatedInterestAppliedInPeriod { get; set; }

    public decimal AggregatedTotalInterestApplied { get; set; }

    public decimal AggregatedTotalPaid { get; set; }
}