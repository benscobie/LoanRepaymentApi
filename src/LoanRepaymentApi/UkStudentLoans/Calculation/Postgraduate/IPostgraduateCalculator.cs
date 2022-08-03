namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public interface IPostgraduateCalculator
{
    UkStudentLoanProjection? Run(PostgraduateCalculatorRequest request);
}