namespace LoanRepaymentApi.Common
{
    public interface IRetailPriceIndex
    {
        decimal Get(DateTime periodDate);
    }
}
