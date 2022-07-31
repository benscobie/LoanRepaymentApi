namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public interface IPostgraduateCalculator
{
    List<UkStudentLoanTypeResult> Run(PostgraduateCalculatorRequest request);
}