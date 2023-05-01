namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

using LoanRepaymentApi.UkStudentLoans;

public class ThresholdBand
{
    public DateTimeOffset DateFrom { get; init; }

    public DateTimeOffset? DateTo { get; init; }

    public UkStudentLoanType LoanType { get; init; }

    public int Threshold { get; init; }
}