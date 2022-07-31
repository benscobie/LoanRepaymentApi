namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public interface IUkStudentLoanCalculator
{
    List<UkStudentLoanResult> Run(UkStudentLoanCalculatorRequest request);
}