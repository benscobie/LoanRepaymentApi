using LoanRepaymentApi.Common;

namespace LoanRepaymentApi.Integration.Tests
{
    public class TestRetailPriceIndex : IRetailPriceIndex
    {
        public decimal GetForPreviousMarch()
        {
            return 0.015m;
        }
    }
}
