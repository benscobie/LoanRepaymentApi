using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestPrevailingMarketRateCap : IPrevailingMarketRateCap
    {
        public decimal Get()
        {
            return 1;
        }
    }
}
