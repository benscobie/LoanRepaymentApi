namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public class Plan1InterestRate : IPlan1InterestRate
    {
        public decimal Get(DateTime periodDate)
        {
            return 0.043m;
        }
    }
}
