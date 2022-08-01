namespace LoanRepaymentApi.UkStudentLoans;

using System.Net.Mime;
using Hellang.Middleware.ProblemDetails;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UkStudentLoansController : ControllerBase
{
    private readonly ILogger<UkStudentLoansController> _logger;
    private readonly IUkStudentLoanCalculator _ukStudentLoanCalculator;
    private readonly UkStudentLoanCalculationDtoValidator _calculationDtoValidator;

    public UkStudentLoansController(
        ILogger<UkStudentLoansController> logger,
        IUkStudentLoanCalculator ukStudentLoanCalculator,
        UkStudentLoanCalculationDtoValidator calculationDtoValidator)
    {
        _logger = logger;
        _ukStudentLoanCalculator = ukStudentLoanCalculator;
        _calculationDtoValidator = calculationDtoValidator;
    }

    [HttpPost("calculate")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<UkStudentLoanResult>>> Calculate(UkStudentLoanCalculationDto request)
    {
        var validationResult = await _calculationDtoValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            var validation = new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };

            throw new ProblemDetailsException(validation);
        }
        
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