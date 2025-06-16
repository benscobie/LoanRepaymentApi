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
            SalaryAdjustments = request.SalaryAdjustments,
        }, loans)
        {
            SalaryGrowth = request.SalaryGrowth,
            AnnualEarningsGrowth = request.AnnualEarningsGrowth,
            VoluntaryRepayments = request.VoluntaryRepayments
        };

        foreach (var loan in request.Loans)
        {
            calculatorRequest.Loans.Add(new UkStudentLoan
            {
                Type = loan.LoanType,
                BalanceRemaining = loan.BalanceRemaining,
                AcademicYearLoanTakenOut = loan.AcademicYearLoanTakenOut,
                CourseStartDate = loan.CourseStartDate,
                CourseEndDate = loan.CourseEndDate,
                StudyingPartTime = loan.StudyingPartTime
            });
        }

        try
        {
            var calculationResults = _ukStudentLoanCalculator.Run(calculatorRequest);

            var result = new UkStudentLoanResultsDto
            {
                Results = _mapper.Map<List<UkStudentLoanResultDto>>(calculationResults)
            };

            return result;
        }
        catch (OverflowException)
        {
            var validation = new ValidationProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Number Too Large",
                Detail = "One or more of the numbers you entered, or values calculated from them, is too large for the system to handle. Please review your entries and try again."
            };

            throw new ProblemDetailsException(validation);
        }
    }

    [HttpGet("assumptions")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<UkStudentLoanAssumptionsDto> GetAssumptions()
    {
        return new UkStudentLoanAssumptionsDto
        {
            SalaryGrowth = 0.05m,
            AnnualEarningsGrowth = 0.042m
        };
    }
}