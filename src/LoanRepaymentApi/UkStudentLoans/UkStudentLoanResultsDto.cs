namespace LoanRepaymentApi.UkStudentLoans;

public class UkStudentLoanResultsDto
{
    public List<UkStudentLoanResultDto> Results { get; set; } = new();

    public DateTimeOffset DebtClearedDate => Results.Max(x => x.PeriodDate);

    public int DebtClearedNumberOfPeriods => Results.Max(x => x.Period);

    public decimal TotalPaid => Results.Last().AggregatedTotalPaid;

    public decimal TotalInterestPaid => Results.Last().AggregatedTotalInterestPaid;
}