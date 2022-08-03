namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public interface IStandardTypeCalculator
{
    List<UkStudentLoanProjection> Run(StandardTypeCalculatorRequest request);
}