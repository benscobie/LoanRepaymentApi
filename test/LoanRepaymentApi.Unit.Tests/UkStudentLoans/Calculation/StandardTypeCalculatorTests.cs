﻿namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using Moq;
using Xunit;

public class StandardTypeCalculatorTests
{
    [Theory, AutoMoqData]
    public void Execute_WithSingleLoanNotEndOfPeriod_ShouldPayOffSomeOfTheBalance(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                InterestRate = 0.01m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>()
        };

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

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
                InterestAppliedInPeriod = 1m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.Paying
            }
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }

    [Theory, AutoMoqData]
    public void Execute_WithSingleLoanLastPeriod_ShouldPayOffRemainingBalance(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                InterestRate = 0.01m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
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

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

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
                InterestAppliedInPeriod = 0.3758m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff
            }
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }

    [Theory, AutoMoqData]
    public void Execute_WithType1AndType2Loans_ShouldPayOffType2AndCarryExcessToType2(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 600m,
                InterestRate = 0.01m
            },
            new()
            {
                Type = UkStudentLoanType.Type2,
                BalanceRemaining = 1_200,
                InterestRate = 0.01m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
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

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.Is<ThresholdOperationFact>(x => x.LoanType == UkStudentLoanType.Type1)))
            .Returns(20_000);
        thresholdOperation.Setup(x => x.Execute(It.Is<ThresholdOperationFact>(x => x.LoanType == UkStudentLoanType.Type2)))
            .Returns(30_000);

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
                RepaymentStatus = UkStudentLoanRepaymentStatus.Paying
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
                RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff
            }
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
    
    [Theory, AutoMoqData]
    public void Execute_WithSingleLoanThatIsBeingWrittenOff_ShouldReturnWrittenOffResult(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                InterestRate = 0.01m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
            Period = 2,
            PeriodDate = new DateTime(2022, 02, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 01, 01),
                    LoanType = UkStudentLoanType.Type1,
                    InterestRate = 0.01m,
                    DebtRemaining = 451m,
                    TotalPaid = 750m,
                    PaidInPeriod = 750m,
                    TotalInterestPaid = 1m,
                    InterestAppliedInPeriod = 1m,
                }
            }
        };

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(true);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

        var expected = new List<UkStudentLoanTypeResult>
        {
            new()
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanType = UkStudentLoanType.Type1,
                InterestRate = 0.01m,
                DebtRemaining = 0,
                TotalPaid = 750m,
                PaidInPeriod = 0m,
                TotalInterestPaid = 1m,
                InterestAppliedInPeriod = 0m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff
            }
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
}