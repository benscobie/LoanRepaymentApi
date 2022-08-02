namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.InterestRate;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
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
    public void Execute_WhenCalledWithFact_ShouldReturnExpectedResult(InterestRateOperationFact fact,
        decimal expectedResult)
    {
        // Arrange & Act
        var result = new InterestRateOperation().Execute(fact);

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
                0.015m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type4
                },
                0.015m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate
                },
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    AnnualSalaryBeforeTax = 27294
                },
                0.015m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    AnnualSalaryBeforeTax = 49129,
                    StudyingPartTime = true,
                    PeriodDate = new DateTimeOffset(2023, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseStartDate = new DateTimeOffset(2020, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    AnnualSalaryBeforeTax = 38212,
                    StudyingPartTime = true,
                    PeriodDate = new DateTimeOffset(2025, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseStartDate = new DateTimeOffset(2020, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                0.030m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    AnnualSalaryBeforeTax = 38212,
                    StudyingPartTime = false,
                    PeriodDate = new DateTimeOffset(2020, 03, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseEndDate = new DateTimeOffset(2019, 07, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                0.045m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    AnnualSalaryBeforeTax = 38212,
                    StudyingPartTime = false,
                    PeriodDate = new DateTimeOffset(2020, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseEndDate = new DateTimeOffset(2019, 07, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                0.030m
            },
            new object[]
            {
                new InterestRateOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    AnnualSalaryBeforeTax = 60000,
                },
                0.045m
            },
        };
    }
}