namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public class Plan4InterestRate : IPlan4InterestRate
    {
        public decimal Get(DateTime periodDate)
        {
            return 0.043m;
        }
    }
}
