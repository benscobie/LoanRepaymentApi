using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using LoanRepaymentApi;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails(ConfigureProblemDetails);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUkStudentLoanCalculator, UkStudentLoanCalculator>();
builder.Services.AddScoped<IStandardTypeCalculator, StandardTypeCalculator>();
builder.Services.AddScoped<IPostgraduateCalculator, PostgraduateCalculator>();
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddValidatorsFromAssemblyContaining<UkStudentLoanCalculationDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseProblemDetails();

app.UseHttpsRedirection();

app.MapControllers();

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
public partial class Program { }