namespace LoanRepaymentApi.UkStudentLoans;

using LoanRepaymentApi.UkStudentLoans.Calculation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UkStudentLoansController : ControllerBase
{
    private readonly ILogger<UkStudentLoansController> _logger;
    private readonly IUkStudentLoanCalculator _ukStudentLoanCalculator;

    public UkStudentLoansController(ILogger<UkStudentLoansController> logger, IUkStudentLoanCalculator ukStudentLoanCalculator)
    {
        _logger = logger;
        _ukStudentLoanCalculator = ukStudentLoanCalculator;
    }

    [HttpPost("calculate")]
    public IEnumerable<UkStudentLoanResult> Calculate(UkStudentLoanCalculationDto request)
    {
        var loans = new List<UkStudentLoan>();
        var calculatorRequest = new UkStudentLoanCalculatorRequest(new Income
        {
            AnnualSalaryBeforeTax = request.AnnualSalaryBeforeTax,
        }, loans);

        foreach (var loan in request.Loans)
        {
            calculatorRequest.Loans.Add(new UkStudentLoan
            {
                Type = loan.LoanType,
                BalanceRemaining = loan.BalanceRemaining,
                InterestRate = loan.InterestRate,
                RepaymentThreshold = loan.RepaymentThreshold
            });
        }
        
        var results = _ukStudentLoanCalculator.Run(calculatorRequest);
        
        // TODO Map response to DTO + loan type enum should be string
        return results;
    }
}