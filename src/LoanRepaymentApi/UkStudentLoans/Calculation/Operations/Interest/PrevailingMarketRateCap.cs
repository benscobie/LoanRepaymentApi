namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

public class PrevailingMarketRateCap : IPrevailingMarketRateCap
{
    public decimal Get()
    {
        return 0.073m;
    }
}
