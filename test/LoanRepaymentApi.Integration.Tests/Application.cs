﻿namespace LoanRepaymentApi.Integration.Tests;

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
            services.AddSingleton<IClock>(new FakeClock(Instant.FromUtc(2022, 1, 1, 0, 0, 0)));
        });

        return base.CreateHost(builder);
    }
}