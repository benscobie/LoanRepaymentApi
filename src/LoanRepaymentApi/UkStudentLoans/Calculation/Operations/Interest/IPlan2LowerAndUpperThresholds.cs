namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public interface IPlan2LowerAndUpperThresholds
    {
        Plan2LowerAndUpperThreshold Get(DateTime periodDate);
    }
}
