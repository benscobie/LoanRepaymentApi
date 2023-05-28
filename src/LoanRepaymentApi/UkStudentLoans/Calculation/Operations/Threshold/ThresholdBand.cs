namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

using LoanRepaymentApi.UkStudentLoans;

public class ThresholdBand
{
    public DateTime DateFrom { get; init; }

    public DateTime? DateTo { get; init; }

    public UkStudentLoanType LoanType { get; init; }

    public int Threshold { get; init; }
}