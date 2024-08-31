namespace LoanRepaymentApi.Integration.Tests;

using LoanRepaymentApi.Common;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Testing;

public class Application : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public Application(string environment = "Development")
    {
        _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IClock>(new FakeClock(Instant.FromUtc(2022, 04, 1, 0, 0, 0)));
            services.AddScoped<IRetailPriceIndex, TestRetailPriceIndex>();
            services.AddScoped<IPrevailingMarketRateCap, TestPrevailingMarketRateCap>();
            services.AddScoped<IPlan1InterestRate, TestPlan1InterestRate>();
            services.AddScoped<IPlan4InterestRate, TestPlan4InterestRate>();
            services.AddScoped<IPlan2LowerAndUpperThresholds, TestPlan2LowerAndUpperThresholds>();
            services.AddScoped<IThresholdsProvider, TestThresholdsProvider>();
        });

        return base.CreateHost(builder);
    }
}
