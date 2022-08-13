namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.Threshold;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using Xunit;

public class ThresholdOperationTests
{
    [Theory, AutoMoqData]
    public void Execute_WhenAThresholdCannotBeFound_ShouldThrowException(ThresholdOperation sut,
        ThresholdOperationFact fact)
    {
        // Arrange
        fact.LoanType = UkStudentLoanType.NotSet;

        // Act
        Action act = () => sut.Execute(fact);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void Execute_WhenCalledWithPeriodInBetweenBand_ShouldReturnExpectedResult(ThresholdOperationFact fact,
        int expectedResult)
    {
        // Arrange & Act
        var result = new ThresholdOperation().Execute(fact);

        // Assert
        result.Should().Be(expectedResult);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenLastThresholdGrowthWithin12Months_ShouldNotGrow(ThresholdOperation sut, ThresholdOperationFact fact)
    {
        // Arrange
        fact.Period = 24;
        fact.PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0));
        fact.AnnualEarningsGrowth = 0.10m;
        fact.LoanType = UkStudentLoanType.Type1;
        fact.PreviousProjections = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 23,
                PeriodDate = new DateTimeOffset(2022, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 33000,
                LoanType = fact.LoanType,
            },
            new()
            {
                Period = 13,
                PeriodDate = new DateTimeOffset(2021, 05, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 33000,
                LoanType = fact.LoanType,
            },
            new()
            {
                Period = 12,
                PeriodDate = new DateTimeOffset(2021, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 32000,
                LoanType = fact.LoanType,
            },
        };

        // Act
        var result = new ThresholdOperation().Execute(fact);

        // Assert
        result.Should().Be(33000);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenLastThresholdGrowthOutside12MonthsWithPreviousGrowth_ShouldGrow(ThresholdOperation sut, ThresholdOperationFact fact)
    {
        // Arrange
        fact.Period = 24;
        fact.PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0));
        fact.AnnualEarningsGrowth = 0.10m;
        fact.LoanType = UkStudentLoanType.Type1;
        fact.PreviousProjections = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 23,
                PeriodDate = new DateTimeOffset(2021, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 33000,
                LoanType = fact.LoanType,
            },
            new()
            {
                Period = 12,
                PeriodDate = new DateTimeOffset(2021, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 33000,
                LoanType = fact.LoanType,
            },
            new()
            {
                Period = 11,
                PeriodDate = new DateTimeOffset(2021, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 31000,
                LoanType = fact.LoanType,
            },
        };

        // Act
        var result = new ThresholdOperation().Execute(fact);

        // Assert
        result.Should().Be(36300);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenLastThresholdGrowthOutside12MonthsWithNoPreviousGrowth_ShouldGrow(ThresholdOperation sut, ThresholdOperationFact fact)
    {
        // Arrange
        fact.Period = 13;
        fact.PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0));
        fact.AnnualEarningsGrowth = 0.10m;
        fact.LoanType = UkStudentLoanType.Type1;
        fact.PreviousProjections = new List<UkStudentLoanProjection>
        {
            new()
            {
                Period = 12,
                PeriodDate = new DateTimeOffset(2022, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 33000,
                LoanType = fact.LoanType,
            },
            new()
            {
                Period = 1,
                PeriodDate = new DateTimeOffset(2021, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Threshold = 33000,
                LoanType = fact.LoanType,
            },
        };

        // Act
        var result = new ThresholdOperation().Execute(fact);

        // Assert
        result.Should().Be(36300);
    }
    
    public static IEnumerable<object[]> TestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    AnnualEarningsGrowth = 0.10m,
                    Period = 1
                },
                20195
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    AnnualEarningsGrowth = 0.10m,
                    Period = 1
                },
                27295
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    AnnualEarningsGrowth = 0.10m,
                    Period = 1
                },
                25375
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    AnnualEarningsGrowth = 0.10m,
                    Period = 1
                },
                21000
            },
        };
    }
}