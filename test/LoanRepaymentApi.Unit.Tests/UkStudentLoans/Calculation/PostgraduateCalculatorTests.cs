namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;
using Moq;
using Xunit;

public class PostgraduateCalculatorTests
{
    [Theory, AutoMoqData]
    public void Execute_WithSingleLoanNotEndOfPeriod_ShouldPayOffSomeOfTheBalance(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m,
            InterestRate = 0.01m
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            PreviousPeriods = new List<UkStudentLoanTypeResult>()
        };
        
        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

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
            InterestAppliedInPeriod = 1m,
            RepaymentStatus = UkStudentLoanRepaymentStatus.Paying
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
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m,
            InterestRate = 0.01m,
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
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
        
        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

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
            InterestAppliedInPeriod = 0.0083m,
            RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff
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
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000m
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m,
            InterestRate = 0.01m
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
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

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(true);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

        var expected = new UkStudentLoanTypeResult
        {
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0.01m,
            DebtRemaining = 0,
            TotalPaid = 500.00m,
            PaidInPeriod = 0,
            TotalInterestPaid = 1m,
            InterestAppliedInPeriod = 0,
            RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
}