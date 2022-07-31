namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public interface IUkStudentLoanCalculator
{
    List<UkStudentLoanResult> Execute(UkStudentLoanCalculatorRequest request);
}