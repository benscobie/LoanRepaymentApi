namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public interface IPlan4InterestRate
    {
        decimal Get(DateTime periodDate);
    }
}
