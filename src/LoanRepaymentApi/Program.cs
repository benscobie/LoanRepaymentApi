using AspNetCoreRateLimit;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using LoanRepaymentApi;
using LoanRepaymentApi.Common;
using LoanRepaymentApi.Infrastructure;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using NodaTime;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSentry();

// Add services to the container.

builder.Services.AddOptions();
builder.Services.AddMemoryCache();

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddProblemDetails(ConfigureProblemDetails);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsAllowedOrigins = "AllowedOrigins";
var appSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsAllowedOrigins,
        policy =>
        {
            policy.WithOrigins(appSettings.CORSOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddScoped<IUkStudentLoanCalculator, UkStudentLoanCalculator>();
builder.Services.AddScoped<IStandardTypeCalculator, StandardTypeCalculator>();
builder.Services.AddScoped<IRetailPriceIndex, RetailPriceIndex>();
builder.Services.AddScoped<IPrevailingMarketRateCap, PrevailingMarketRateCap>();
builder.Services.AddScoped<IPlan1InterestRate, Plan1InterestRate>();
builder.Services.AddScoped<IPlan4InterestRate, Plan4InterestRate>();
builder.Services.AddScoped<IThresholdsProvider, ThresholdsProvider>();
builder.Services.AddScoped<IPlan2LowerAndUpperThresholds, Plan2LowerAndUpperThresholds>();

// TODO Bulk register IOperations
builder.Services.AddScoped<ICanLoanBeWrittenOffOperation, CanLoanBeWrittenOffOperation>();
builder.Services.AddScoped<IThresholdOperation, ThresholdOperation>();
builder.Services.AddScoped<IInterestRateOperation, InterestRateOperation>();
builder.Services.AddScoped<ISalaryOperation, SalaryOperation>();
builder.Services.AddScoped<IFirstPossibleRepaymentDateOperation, FirstPossibleRepaymentDateOperation>();
builder.Services.AddScoped<IVoluntaryRepaymentOperation, VoluntaryRepaymentOperation>();

builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddValidatorsFromAssemblyContaining<UkStudentLoanCalculationDtoValidator>();

builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseIpRateLimiting();
}

app.UseProblemDetails();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(corsAllowedOrigins);

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

void ConfigureProblemDetails(ProblemDetailsOptions options)
{
    // Only include exception details in a development environment. There's really no nee
    // to set this as it's the default behavior. It's just included here for completeness :)
    options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();

    // Custom mapping function for FluentValidation's ValidationException.
    options.MapFluentValidationException();

    // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
    // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
    options.Rethrow<NotSupportedException>();

    // This will map NotImplementedException to the 501 Not Implemented status code.
    options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

    // This will map HttpRequestException to the 503 Service Unavailable status code.
    options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

    // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
    // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
}

// Make the implicit Program class public so test projects can access it
public partial class Program
{
}
