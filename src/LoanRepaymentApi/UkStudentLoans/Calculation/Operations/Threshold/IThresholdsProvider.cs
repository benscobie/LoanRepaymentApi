namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public interface IThresholdsProvider
{
    List<ThresholdBand> Get();
}