namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.InterestRate;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.Common;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
using Moq;
using Xunit;

public class InterestRateOperationTests
{
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithFactWithInvalidLoanType_ShouldThrowException(InterestRateOperation sut,
        InterestRateOperationFact fact)
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
    public void Execute_WhenCalledWithFact_ShouldReturnExpectedResult(
        InterestRateOperationFact fact,
        decimal plan2And4InterestRateCap,
        decimal expectedResult)
    {
        // Arrange & Act
        var prevailingMarketRateCapMock = new Mock<IPrevailingMarketRateCap>();
        prevailingMarketRateCapMock.Setup(x => x.Get()).Returns(plan2And4InterestRateCap);
        var plan1InterestRateMock = new Mock<IPlan1InterestRate>();
        plan1InterestRateMock.Setup(x => x.Get(fact.PeriodDate)).Returns(0.015m);
        var plan4InterestRateMock = new Mock<IPlan4InterestRate>();
        plan4InterestRateMock.Setup(x => x.Get(fact.PeriodDate)).Returns(0.015m);
        var retailPriceIndexMock = new Mock<IRetailPriceIndex>();
        retailPriceIndexMock.Setup(x => x.Get(fact.PeriodDate)).Returns(0.015m);
        var plan2LowerAndUpperThresholdsMock = new Mock<IPlan2LowerAndUpperThresholds>();
        plan2LowerAndUpperThresholdsMock.Setup(x => x.Get(fact.PeriodDate)).Returns(new Plan2LowerAndUpperThreshold
        {
            LowerThreshold = 27295,
            UpperThreshold = 49130
        });

        var result = new InterestRateOperation(prevailingMarketRateCapMock.Object, plan1InterestRateMock.Object, plan4InterestRateMock.Object, retailPriceIndexMock.Object, plan2LowerAndUpperThresholdsMock.Object).Execute(fact);

        // Assert
        result.Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> TestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type1
                },
                1m, // Irrelevant
                0.015m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type4
                },
                1m,
                0.015m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate
                },
                1m, // Irrelevant
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 27294
                },
                1m,
                0.015m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 27294
                },
                0.01m,
                0.01m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 49129,
                    StudyingPartTime = true,
                    PeriodDate = new DateTime(2023, 12, 01, 0, 0, 0),
                    CourseStartDate = new DateTime(2020, 01, 01, 0, 0, 0),
                },
                1m,
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 38212,
                    StudyingPartTime = true,
                    PeriodDate = new DateTime(2025, 01, 01, 0, 0, 0),
                    CourseStartDate = new DateTime(2020, 01, 01, 0, 0, 0),
                },
                1m,
                0.030m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 38212,
                    StudyingPartTime = false,
                    PeriodDate = new DateTime(2020, 03, 01, 0, 0, 0),
                    CourseEndDate = new DateTime(2019, 07, 01, 0, 0, 0),
                },
                1m,
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 38212,
                    StudyingPartTime = false,
                    PeriodDate = new DateTime(2020, 04, 01, 0, 0, 0),
                    CourseEndDate = new DateTime(2019, 07, 01, 0, 0, 0),
                },
                1m,
                0.030m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 60000,
                },
                1m,
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    Salary = 60000,
                },
                0.01m,
                0.01m
            },
        };
    }
}
