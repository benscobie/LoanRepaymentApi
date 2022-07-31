namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using NodaTime;
using NodaTime.Testing;
using Xunit;

public class UkStudentLoanCalculatorTests
{
    private readonly UkStudentLoanCalculator _calculator = new(new FakeClock(Instant.FromUtc(2022, 1, 1, 0, 0, 0)));
    
    [Fact]
    public void Execute_WithSingleLoan_ShouldReturnExpectedResults()
    {
        // Arrange
        var income = new Income
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new UkStudentLoan
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                InterestRate = 0.01m,
                RepaymentThreshold = 20_000m
            }
        };

        var request = new UkStudentLoanCalculatorRequest
        {
            Income = income,
            Loans = loans
        };

        var expected = new List<UkStudentLoanResult>
        {
            new UkStudentLoanResult
            {
                Period = 1,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanResults = new List<UkStudentLoanTypeResult>
                {
                    new UkStudentLoanTypeResult
                    {
                        Period = 1,
                        PeriodDate = new DateTime(2022, 02, 01),
                        LoanType = UkStudentLoanType.Type1,
                        InterestRate = 0.01m,
                        DebtRemaining = 451m,
                        TotalPaid = 750m,
                        PaidInPeriod = 750m,
                        TotalInterestPaid = 1m,
                        InterestAppliedInPeriod = 1m
                    }
                }
            },
            new UkStudentLoanResult
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 03, 01),
                LoanResults = new List<UkStudentLoanTypeResult>
                {
                    new UkStudentLoanTypeResult
                    {
                        Period = 2,
                        PeriodDate = new DateTime(2022, 03, 01),
                        LoanType = UkStudentLoanType.Type1,
                        InterestRate = 0.01m,
                        DebtRemaining = 0,
                        TotalPaid = 1201.3758m,
                        PaidInPeriod = 451.3758m,
                        TotalInterestPaid = 1.3758m,
                        InterestAppliedInPeriod = 0.3758m
                    }
                }
            }
        };
        
        // Act
        var results = _calculator.Execute(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
    
    [Fact]
    public void Execute_WithType1AndType2Loans_ShouldReturnExpectedResults()
    {
        // Arrange
        var income = new Income
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new UkStudentLoan
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 600m,
                InterestRate = 0.01m,
                RepaymentThreshold = 20_000m
            },
            new UkStudentLoan
            {
                Type = UkStudentLoanType.Type2,
                BalanceRemaining = 1_200,
                InterestRate = 0.01m,
                RepaymentThreshold = 30_000
            }
        };

        var request = new UkStudentLoanCalculatorRequest
        {
            Income = income,
            Loans = loans
        };

        var expected = new List<UkStudentLoanResult>
        {
            new UkStudentLoanResult
            {
                Period = 1,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanResults = new List<UkStudentLoanTypeResult>
                {
                    new UkStudentLoanTypeResult
                    {
                        Period = 1,
                        PeriodDate = new DateTime(2022, 02, 01),
                        LoanType = UkStudentLoanType.Type1,
                        InterestRate = 0.01m,
                        PaidInPeriod = 75m,
                        InterestAppliedInPeriod = 0.50m,
                        TotalPaid = 75m,
                        TotalInterestPaid = 0.50m,
                        DebtRemaining = 525.50m,
                    },
                    new UkStudentLoanTypeResult
                    {
                        Period = 1,
                        PeriodDate = new DateTime(2022, 02, 01),
                        LoanType = UkStudentLoanType.Type2,
                        InterestRate = 0.01m,
                        PaidInPeriod = 675m,
                        InterestAppliedInPeriod = 1m,
                        TotalPaid = 675m,
                        TotalInterestPaid = 1m,
                        DebtRemaining = 526m,
                    }
                }
            },
            new UkStudentLoanResult
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 03, 01),
                LoanResults = new List<UkStudentLoanTypeResult>
                {
                    
                    new UkStudentLoanTypeResult
                    {
                        Period = 2,
                        PeriodDate = new DateTime(2022, 03, 01),
                        LoanType = UkStudentLoanType.Type1,
                        InterestRate = 0.01m,
                        PaidInPeriod = 223.5616m,
                        InterestAppliedInPeriod = 0.4379m,
                        TotalPaid = 298.5616m,
                        TotalInterestPaid = 0.9379m,
                        DebtRemaining = 302.3762m,
                    },
                    new UkStudentLoanTypeResult
                    {
                        Period = 2,
                        PeriodDate = new DateTime(2022, 03, 01),
                        LoanType = UkStudentLoanType.Type2,
                        InterestRate = 0.01m,
                        PaidInPeriod =  526.4383m,
                        InterestAppliedInPeriod = 0.4383m,
                        TotalPaid = 1201.4383m,
                        TotalInterestPaid = 1.4383m,
                        DebtRemaining = 0,
                    }
                }
            },
            new UkStudentLoanResult
            {
                Period = 3,
                PeriodDate = new DateTime(2022, 04, 01),
                LoanResults = new List<UkStudentLoanTypeResult>
                {
                    new UkStudentLoanTypeResult
                    {
                        Period = 3,
                        PeriodDate = new DateTime(2022, 04, 01),
                        LoanType = UkStudentLoanType.Type1,
                        InterestRate = 0.01m,
                        PaidInPeriod = 302.6282m,
                        InterestAppliedInPeriod = 0.2519m,
                        TotalPaid = 601.1898m,
                        TotalInterestPaid = 1.1898m,
                        DebtRemaining = 0m,
                    }
                }
            }
        };
        
        // Act
        var results = _calculator.Execute(request).ToList();

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
}