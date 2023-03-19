using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestPlan1And4InterestRate : IPlan1And4InterestRate
    {
        public decimal Get()
        {
            return 0.015m;
        }
    }
}
