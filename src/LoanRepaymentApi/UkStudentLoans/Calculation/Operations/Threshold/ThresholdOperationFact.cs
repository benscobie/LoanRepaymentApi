namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class ThresholdOperationFact
{
    public DateTime PeriodDate { get; set; }

    public int Period { get; set; }

    public UkStudentLoanType LoanType { get; set; }

    public List<UkStudentLoanProjection> PreviousProjections { get; set; } = new();

    public decimal AnnualEarningsGrowth { get; set; }
}