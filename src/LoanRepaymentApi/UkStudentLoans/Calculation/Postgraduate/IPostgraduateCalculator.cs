namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public interface IPostgraduateCalculator
{
    UkStudentLoanTypeResult? Run(PostgraduateCalculatorRequest request);
}