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
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using Moq;
using Xunit;

public class StandardTypeCalculatorTests
{
    [Theory, AutoMoqData]
    public void Execute_WithSingleLoanNotEndOfPeriod_ShouldPayOffSomeOfTheBalance(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
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

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 1,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanType = UkStudentLoanType.Type1,
                InterestRate = 0.01m,
                DebtRemaining = 451m,
                TotalPaid = 750m,
                Paid = 750m,
                TotalInterestPaid = 1m,
                InterestApplied = 1m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.Paying,
                Threshold = 20_000
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
    public void Execute_WithALoanPaidOffAndPeriodInFuture_ShouldNotBePaidAgain(
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
            Period = 50,
            PeriodDate = new DateTime(2022, 02, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>
            {
                new()
                {
                    Period = 49,
                    Paid = 1000,
                    TotalPaid = 5000,
                    TotalInterestPaid = 100,
                    LoanType = UkStudentLoanType.Type1,
                    RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff,
                }
            }
        };

        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 50,
                PeriodDate = new DateTime(2022, 02, 01),
                TotalPaid = 5000,
                TotalInterestPaid = 100,
                LoanType = UkStudentLoanType.Type1,
                RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff,
                Threshold = 20_000
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
    public void Execute_WithSinglePostgraduateLoanNotEndOfPeriod_ShouldPayOffSomeOfTheBalance(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Postgraduate,
                BalanceRemaining = 1_200m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
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

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 1,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanType = UkStudentLoanType.Postgraduate,
                InterestRate = 0.01m,
                DebtRemaining = 701.00M,
                TotalPaid = 500m,
                Paid = 500m,
                TotalInterestPaid = 1m,
                InterestApplied = 1m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.Paying,
                Threshold = 20_000
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
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 02, 01),
                    LoanType = UkStudentLoanType.Type1,
                    InterestRate = 0.01m,
                    DebtRemaining = 451m,
                    TotalPaid = 750m,
                    Paid = 750m,
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

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 03, 01),
                LoanType = UkStudentLoanType.Type1,
                InterestRate = 0.01m,
                DebtRemaining = 0,
                TotalPaid = 1201.3758m,
                Paid = 451.3758m,
                TotalInterestPaid = 1.3758m,
                InterestApplied = 0.3758m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff,
                Threshold = 20_000
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
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 600m
            },
            new()
            {
                Type = UkStudentLoanType.Type2,
                BalanceRemaining = 1_200
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
            Period = 2,
            PeriodDate = new DateTime(2022, 03, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 02, 01),
                    LoanType = UkStudentLoanType.Type1,
                    InterestRate = 0.01m,
                    Paid = 75m,
                    InterestApplied = 0.50m,
                    TotalPaid = 75m,
                    TotalInterestPaid = 0.50m,
                    DebtRemaining = 525.50m
                },
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 02, 01),
                    LoanType = UkStudentLoanType.Type2,
                    InterestRate = 0.01m,
                    Paid = 675m,
                    InterestApplied = 1m,
                    TotalPaid = 675m,
                    TotalInterestPaid = 1m,
                    DebtRemaining = 526m
                }
            }
        };

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(false);
        thresholdOperation.Setup(x => x.Execute(It.Is<ThresholdOperationFact>(x => x.LoanType == UkStudentLoanType.Type1)))
            .Returns(20_000);
        thresholdOperation.Setup(x => x.Execute(It.Is<ThresholdOperationFact>(x => x.LoanType == UkStudentLoanType.Type2)))
            .Returns(30_000);
        interestRateOperation.Setup(x => x.Execute(It.IsAny<InterestRateOperationFact>()))
            .Returns(0.01m);

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 03, 01),
                LoanType = UkStudentLoanType.Type1,
                InterestRate = 0.01m,
                Paid = 223.5616m,
                InterestApplied = 0.4379m,
                TotalPaid = 298.5616m,
                TotalInterestPaid = 0.9379m,
                DebtRemaining = 302.3762m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.Paying,
                Threshold = 20_000
            },
            new()
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 03, 01),
                LoanType = UkStudentLoanType.Type2,
                InterestRate = 0.01m,
                Paid = 526.4383m,
                InterestApplied = 0.4383m,
                TotalPaid = 1201.4383m,
                TotalInterestPaid = 1.4383m,
                DebtRemaining = 0,
                RepaymentStatus = UkStudentLoanRepaymentStatus.PaidOff,
                Threshold = 30_000
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
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
            Period = 2,
            PeriodDate = new DateTime(2022, 02, 01),
            Salary = 120_000,
            PreviousProjections = new List<UkStudentLoanProjection>
            {
                new()
                {
                    Period = 1,
                    PeriodDate = new DateTime(2022, 01, 01),
                    LoanType = UkStudentLoanType.Type1,
                    InterestRate = 0.01m,
                    DebtRemaining = 451m,
                    TotalPaid = 750m,
                    Paid = 750m,
                    TotalInterestPaid = 1m,
                    InterestApplied = 1m,
                }
            }
        };

        canLoanBeWrittenOffOperationMock.Setup(x => x.Execute(It.IsAny<CanLoanBeWrittenOffOperationFact>()))
            .Returns(true);
        thresholdOperation.Setup(x => x.Execute(It.IsAny<ThresholdOperationFact>()))
            .Returns(20_000);
        interestRateOperation.Setup(x => x.Execute(It.IsAny<InterestRateOperationFact>()))
            .Returns(0.01m);

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 2,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanType = UkStudentLoanType.Type1,
                InterestRate = 0,
                DebtRemaining = 0,
                TotalPaid = 750m,
                Paid = 0m,
                TotalInterestPaid = 1m,
                InterestApplied = 0m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff,
                Threshold = 20_000
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
    public void Execute_WithSingleLoanWithFuturePaymentDate_ShouldNotPayLoan(
        [Frozen] Mock<ICanLoanBeWrittenOffOperation> canLoanBeWrittenOffOperationMock,
        [Frozen] Mock<IThresholdOperation> thresholdOperation,
        [Frozen] Mock<IInterestRateOperation> interestRateOperation,
        StandardTypeCalculator sut)
    {
        // Arrange
        var income = new PersonDetails
        {
            AnnualSalaryBeforeTax = 120_000
        };

        var loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1,
                BalanceRemaining = 1_200m,
                FirstRepaymentDate = new DateTime(2022, 03, 01)
            }
        };

        var request = new StandardTypeCalculatorRequest(income)
        {
            Loans = loans,
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

        var expected = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 1,
                PeriodDate = new DateTime(2022, 02, 01),
                LoanType = UkStudentLoanType.Type1,
                InterestRate = 0.01m,
                DebtRemaining = 1201,
                TotalPaid = 0,
                Paid = 0,
                TotalInterestPaid = 0,
                InterestApplied = 1m,
                RepaymentStatus = UkStudentLoanRepaymentStatus.NotPaying,
                Threshold = 20_000
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