namespace LoanRepaymentApi.UkStudentLoans;

using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Hellang.Middleware.ProblemDetails;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class UkStudentLoansController : ControllerBase
{
    private readonly ILogger<UkStudentLoansController> _logger;
    private readonly IUkStudentLoanCalculator _ukStudentLoanCalculator;
    private readonly UkStudentLoanCalculationDtoValidator _calculationDtoValidator;
    private readonly IMapper _mapper;

    public UkStudentLoansController(
        ILogger<UkStudentLoansController> logger,
        IUkStudentLoanCalculator ukStudentLoanCalculator,
        UkStudentLoanCalculationDtoValidator calculationDtoValidator,
        IMapper mapper)
    {
        _logger = logger;
        _ukStudentLoanCalculator = ukStudentLoanCalculator;
        _calculationDtoValidator = calculationDtoValidator;
        _mapper = mapper;
    }

    [HttpPost("calculate")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UkStudentLoanResultsDto>> Calculate(UkStudentLoanCalculationDto request)
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
        var calculatorRequest = new UkStudentLoanCalculatorRequest(new PersonDetails
        {
            AnnualSalaryBeforeTax = request.AnnualSalaryBeforeTax,
            BirthDate = request.BirthDate,
            SalaryAdjustments = request.SalaryAdjustments
        }, loans);

        foreach (var loan in request.Loans)
        {
            calculatorRequest.Loans.Add(new UkStudentLoan
            {
                Type = loan.LoanType,
                BalanceRemaining = loan.BalanceRemaining,
                FirstRepaymentDate = loan.FirstRepaymentDate,
                AcademicYearLoanTakenOut = loan.AcademicYearLoanTakenOut,
                CourseStartDate = loan.CourseStartDate,
                CourseEndDate = loan.CourseEndDate,
                StudyingPartTime = loan.StudyingPartTime
            });
        }
        
        var calculationResults = _ukStudentLoanCalculator.Run(calculatorRequest);

        var result = new UkStudentLoanResultsDto
        {
            Results = _mapper.Map<List<UkStudentLoanResultDto>>(calculationResults)
        };
        
        return result;
    }

    [HttpGet("assumptions")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UkStudentLoanAssumptionsDto>> GetAssumptions()
    {
        return new UkStudentLoanAssumptionsDto
        {
            SalaryGrowth = 0.05m,
            AnnualEarningsGrowth = 0.042m
        };
    }
}