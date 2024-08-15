namespace LoanRepaymentApi.Common
{
    public class RetailPriceIndex : IRetailPriceIndex
    {
        public decimal Get(DateTime periodDate)
        {
            if (periodDate >= new DateTime(2024, 09, 01, 0, 0, 0))
            {
                return 0.043m;
            }

            return 0.135m;
        }
    }
}
