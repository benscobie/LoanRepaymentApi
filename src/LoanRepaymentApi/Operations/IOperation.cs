namespace LoanRepaymentApi.Operations;

public interface IOperation<TReturn, TFactType>
    where TFactType : new()
    where TReturn : new()
{
    TReturn Execute(TFactType fact);
}