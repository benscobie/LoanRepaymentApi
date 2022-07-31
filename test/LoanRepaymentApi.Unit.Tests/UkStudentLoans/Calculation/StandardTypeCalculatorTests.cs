namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using Xunit;

public class StandardTypeCalculatorTests
{
    private readonly StandardTypeCalculator _calculator = new();

    [Fact]
    public void Execute_WithSingleLoanNotEndOfPeriod_ShouldPayOffSomeOfTheBalance()
    {
        // Arrange
        var income = new Income
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                InterestRate = 0.01m,
                RepaymentThreshold = 20_000m
            }
        };

        var request = new StandardTypeCalculatorRequest
        {
            Income = income,
            Loans = loans,
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>()
        };

        var expected = new List<UkStudentLoanTypeResult>
        {
            new()
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
        };

        // Act
        var results = _calculator.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }

    [Fact]
    public void Execute_WithSingleLoanLastPeriod_ShouldPayOffRemainingBalance()
    {
        // Arrange
        var income = new Income
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                InterestRate = 0.01m,
                RepaymentThreshold = 20_000m
            }
        };

        var request = new StandardTypeCalculatorRequest
        {
            Income = income,
            Loans = loans,
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>
            {
                new()
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
        };

        var expected = new List<UkStudentLoanTypeResult>
        {
            new()
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
        };

        // Act
        var results = _calculator.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }

    [Fact]
    public void Execute_WithType1AndType2Loans_ShouldPayOffType2AndCarryExcessToType2()
    {
        // Arrange
        var income = new Income
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 600m,
                InterestRate = 0.01m,
                RepaymentThreshold = 20_000m
            },
            new()
            {
                Type = UkStudentLoanType.Type2,
                BalanceRemaining = 1_200,
                InterestRate = 0.01m,
                RepaymentThreshold = 30_000
            }
        };

        var request = new StandardTypeCalculatorRequest
        {
            Income = income,
            Loans = loans,
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>
            {
                new()
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
                new()
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
        };

        var expected = new List<UkStudentLoanTypeResult>
        {
            new()
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
            new()
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 03, 01),
                LoanType = UkStudentLoanType.Type2,
                InterestRate = 0.01m,
                PaidInPeriod = 526.4383m,
                InterestAppliedInPeriod = 0.4383m,
                TotalPaid = 1201.4383m,
                TotalInterestPaid = 1.4383m,
                DebtRemaining = 0,
            }
        };

        // Act
        var results = _calculator.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
}