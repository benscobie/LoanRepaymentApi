namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public class Plan4InterestRate : IPlan4InterestRate
    {
        public decimal Get(DateTime periodDate)
        {
            if (periodDate >= new DateTime(2025, 09, 01, 0, 0, 0))
            {
                return 0.032m;
            }

            return 0.043m;
        }
    }
}
