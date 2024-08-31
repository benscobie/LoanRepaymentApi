namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public interface IPlan1InterestRate
    {
        decimal Get(DateTime periodDate);
    }
}
