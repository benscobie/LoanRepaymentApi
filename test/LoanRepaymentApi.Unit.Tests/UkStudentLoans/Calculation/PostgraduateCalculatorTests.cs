namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
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
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            PreviousProjections = new List<UkStudentLoanProjection>(),
            Salary = 120_000
        };
        
        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);
        interestRateOperation.Setup(x => x.Execute(It.IsAny<InterestRateOperationFact>()))
            .Returns(0.01m);

        var expected = new UkStudentLoanProjection
        {
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0.01m,
            DebtRemaining = 701m,
            TotalPaid = 500.00m,
            Paid = 500.00m,
            TotalInterestPaid = 1m,
            InterestApplied = 1m,
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
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 02, 01),
                    LoanType = UkStudentLoanType.Postgraduate,
                    InterestRate = 0.01m,
                    DebtRemaining = 10m,
                    TotalPaid = 500.00m,
                    Paid = 500.00m,
                    TotalInterestPaid = 1m,
                    InterestApplied = 1m
                }
            }
        };
        
        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);
        interestRateOperation.Setup(x => x.Execute(It.IsAny<InterestRateOperationFact>()))
            .Returns(0.01m);

        var expected = new UkStudentLoanProjection
        {
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0.01m,
            DebtRemaining = 0,
            TotalPaid = 510.0083m,
            Paid = 10.0083m,
            TotalInterestPaid = 1.0083m,
            InterestApplied = 0.0083m,
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
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 02, 01),
                    LoanType = UkStudentLoanType.Postgraduate,
                    InterestRate = 0.01m,
                    DebtRemaining = 10m,
                    TotalPaid = 500.00m,
                    Paid = 500.00m,
                    TotalInterestPaid = 1m,
                    InterestApplied = 1m
                }
            }
        };

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(true);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);
        interestRateOperation.Setup(x => x.Execute(It.IsAny<InterestRateOperationFact>()))
            .Returns(0.01m);

        var expected = new UkStudentLoanProjection
        {
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0,
            DebtRemaining = 0,
            TotalPaid = 500.00m,
            Paid = 0,
            TotalInterestPaid = 1m,
            InterestApplied = 0,
            RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
    
    [Theory, AutoMoqData]
    public void Execute_WithSingleLoanWithFuturePaymentDate_ShouldNotPayLoan(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        PostgraduateCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loan = new UkStudentLoan
        {
            Type = UkStudentLoanType.Postgraduate,
            BalanceRemaining = 1_200m,
            FirstRepaymentDate = new DateTime(2022, 03, 01)
        };

        var request = new PostgraduateCalculatorRequest(income, loan)
        {
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>()
        };

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);
        interestRateOperation.Setup(x => x.Execute(It.IsAny<InterestRateOperationFact>()))
            .Returns(0.01m);

        var expected = new UkStudentLoanProjection
        {
            Period = 1,
            PeriodDate = new DateTime(2022, 02, 01),
            LoanType = UkStudentLoanType.Postgraduate,
            InterestRate = 0.01m,
            DebtRemaining = 1201,
            TotalPaid = 0,
            Paid = 0,
            TotalInterestPaid = 0,
            InterestApplied = 1m,
            RepaymentStatus = UkStudentLoanRepaymentStatus.NotPaying
        };

        // Act
        var results = sut.Run(request);

        // Assert
        results.Should().BeEquivalentTo(expected, options => options
            .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.0001m))
            .WhenTypeIs<decimal>());
    }
}