namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;
using Xunit;

public class PostgraduateCalculatorTests
{
    private readonly PostgraduateCalculator _calculator = new();

    [Fact]
    public void Execute_WithSingleLoanNotEndOfPeriod_ShouldPayOffSomeOfTheBalance()
    {
        // Arrange
        var income = new Income
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m,
            InterestRate = 0.01m,
            RepaymentThreshold = 20_000m
        };

        var request = new PostgraduateCalculatorRequest(income)
        {
            Loan = loan,
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>()
        };

        var expected = new UkStudentLoanTypeResult
        {
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0.01m,
            DebtRemaining = 701m,
            TotalPaid = 500.00m,
            PaidInPeriod = 500.00m,
            TotalInterestPaid = 1m,
            InterestAppliedInPeriod = 1m
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

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m,
            InterestRate = 0.01m,
            RepaymentThreshold = 20_000m
        };

        var request = new PostgraduateCalculatorRequest(income)
        {
            Loan = loan,
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 02, 01),
                    LoanType = UkStudentLoanType.Postgraduate,
                    InterestRate = 0.01m,
                    DebtRemaining = 10m,
                    TotalPaid = 500.00m,
                    PaidInPeriod = 500.00m,
                    TotalInterestPaid = 1m,
                    InterestAppliedInPeriod = 1m
                }
            }
        };

        var expected = new UkStudentLoanTypeResult
        {
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0.01m,
            DebtRemaining = 0,
            TotalPaid = 510.0083m,
            PaidInPeriod = 10.0083m,
            TotalInterestPaid = 1.0083m,
            InterestAppliedInPeriod = 0.0083m
        };

        // Act
        var results = _calculator.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
}