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
        var plan2And4InterestRateCapMock = new Mock<IPlan2AndPostgraduateInterestRateCap>();
        plan2And4InterestRateCapMock.Setup(x => x.Get()).Returns(plan2And4InterestRateCap);
        var plan1And4InterestRateMock = new Mock<IPlan1And4InterestRate>();
        plan1And4InterestRateMock.Setup(x => x.Get()).Returns(0.015m);
        var retailPriceIndexMock = new Mock<IRetailPriceIndex>();
        retailPriceIndexMock.Setup(x => x.GetForPreviousMarch()).Returns(0.015m);

        var result = new InterestRateOperation(plan2And4InterestRateCapMock.Object, plan1And4InterestRateMock.Object, retailPriceIndexMock.Object).Execute(fact);

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
                    PeriodDate = new DateTimeOffset(2023, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseStartDate = new DateTimeOffset(2020, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
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
                    PeriodDate = new DateTimeOffset(2025, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseStartDate = new DateTimeOffset(2020, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
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
                    PeriodDate = new DateTimeOffset(2020, 03, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseEndDate = new DateTimeOffset(2019, 07, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
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
                    PeriodDate = new DateTimeOffset(2020, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseEndDate = new DateTimeOffset(2019, 07, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
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
