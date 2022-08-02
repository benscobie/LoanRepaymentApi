namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.Threshold;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
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
    public void Execute_WhenCalledWithFact_ShouldReturnExpectedResult(ThresholdOperationFact fact,
        int expectedResult)
    {
        // Arrange

        // Act
        var result = new ThresholdOperation().Execute(fact);

        // Assert
        result.Should().Be(expectedResult);
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
                },
                20195
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    PeriodDate = new DateTimeOffset(2050, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                20195
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                27295
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    PeriodDate = new DateTimeOffset(2050, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                27295
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                25375
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    PeriodDate = new DateTimeOffset(2050, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                25375
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                21000
            },
            new object[]
            {
                new ThresholdOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    PeriodDate = new DateTimeOffset(2050, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                21000
            },
        };
    }
}