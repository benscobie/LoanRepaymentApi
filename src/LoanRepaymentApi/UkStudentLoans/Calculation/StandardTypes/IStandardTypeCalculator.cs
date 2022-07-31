namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public interface IStandardTypeCalculator
{
    List<UkStudentLoanTypeResult> Run(StandardTypeCalculatorRequest request);
}